using Abilities;
using Abilities.Attacks;
using Items;
using Stats;
using System.Collections;
using System.Collections.Generic;
using Entities.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Entity
    {
        private static readonly int IS_MOVING = Animator.StringToHash("isMoving");
        private static readonly int DASH      = Animator.StringToHash("dash");
        private static readonly int ATTACK    = Animator.StringToHash("attack");
        private static readonly int SPECIAL   = Animator.StringToHash("special");
        private static readonly int SWITCH    = Animator.StringToHash("switch");

        public UnityEvent<int, int> OnHealthUpdate;
        
        public bool GobletReady;

        public enum Character { Fenrir, Hel }
        
        [SerializeField] private Transform _characterContainer;
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Collision")]
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _frontCollisionPoint;

        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private LayerMask _holeLayer;

        [Header("Interaction")]
        [SerializeField] private float _lookWeight;
        [SerializeField] private float _distanceWeight;

        [Header("Healing")]
        [SerializeField] private int _gobletHealAmount;
        public int _gobletCost;
        
        
        [Header("Dash")]
        [SerializeField] private Ability _dashAbility;
        
        [SerializeField] private Transform _dashPoint;

        [Range(0.01f, 0.5f)]
        [SerializeField] private float _dashDuration;
        [Range(0f, 1f), Tooltip("Fraction of dash duration to fade back to normal speed.")]
        [SerializeField] private float _dashFade;

        [Tooltip("The fraction cutoff for dashing OVER holes")]
        [Range(0.5f, 1f)]
        [SerializeField] private float _dashHoleSnapFraction;

        [SerializeField] private LayerMask _dashingPlayerLayer;
        
        [Header("Fenrir")]
        [SerializeField] private CharacterAbilitySet _fenrirAbilities;
        [SerializeField] private Animator  _fenrirAnimator;
        [SerializeField] private Transform _fenrirAttackPoint;
        [SerializeField] private Transform _fenrirSpecialPoint;
        [SerializeField] private Transform _fenrirAttackDashPoint;
        
        [Header("Hel")]
        [SerializeField] private CharacterAbilitySet _helAbilities;
        [SerializeField] private Animator  _helAnimator;
        [SerializeField] private Transform _helAttackPoint;
        [SerializeField] private Transform _helSpecialPoint;
        
        private Animator[] _animators;
        private Animator CurrentAnimator => _animators[(int) ActiveCharacter];
        
        public Character ActiveCharacter;
        
        private AttackAbilityTracker[] _attackAbilityTrackers;
        private AttackAbilityTracker[] _specialAbilityTrackers;
        private AbilityTracker _dashAbilityTracker;
        private AttackAbilityTracker AttackAbilityTracker => _attackAbilityTrackers[(int) ActiveCharacter];
        private AttackAbilityTracker SpecialAbilityTracker => _specialAbilityTrackers[(int) ActiveCharacter];

        private Transform[] _attackPoints;
        private Transform[] _specialPoint;

        private Vector3 AttackPosition => _attackPoints[(int) ActiveCharacter].position;
        private Vector3 SpecialPosition => _specialPoint[(int) ActiveCharacter].position;
        
        private AbilityStats _currentAttackStats;
        private int          _currentAttackUseTimes;
        
        private PlayerBaseStats _playerBaseStats;
        
        private Camera _camera;
        
        private Vector2 _moveInput;
        private Vector2 _lastMoveInput;
        private Vector2 _dashInputSnapshot;
        
        private float _critChance;
        private float _critDamage;
        
        protected float _damageReduction = 0f;
        
        private readonly List<IItem> _items = new();

        private float _originalDashDistance;
        private float _originalMoveSpeed;
        private Coroutine _dashCoroutine;

        private bool _charging;
        private bool _hasControl = true;
        private bool _isDashing;
        
        private List<IInteractable> _interactables = new();
        private IInteractable _currentInteractable;

        protected override void Start()
        {
            _playerInput.SwitchCurrentActionMap("Dialogue");
            _playerInput.SwitchCurrentActionMap("UI");
            _playerInput.SwitchCurrentActionMap("Player");
            
            _originalDashDistance = Vector3.Distance(transform.position, _dashPoint.position);

            _animators = new[]
            {
                _fenrirAnimator,
                _helAnimator
            };
            
            _attackAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK)),
                new(_helAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK))
            };
            
            _specialAbilityTrackers = new AttackAbilityTracker[]
            {
                // new(_fenrirAbilities.Special, (ability, action) =>
                //     StartAttack(ability, action, SPECIAL)),
                // new(_helAbilities.Special, (ability, action) =>
                //     StartAttack(ability, action, SPECIAL))
            };

            _dashAbilityTracker = new(_dashAbility, () => PerformDash(_dashPoint.position, true));

            _attackPoints = new []
            {
                _fenrirAttackPoint,
                _helAttackPoint
            };

            _specialPoint = new []
            {
                _fenrirSpecialPoint,
                _helSpecialPoint
            };
            
            _playerBaseStats = (PlayerBaseStats) EntityBaseStats;
            
            InitializeBaseStats();
            _camera = Camera.main!;
            
            CharacterIndexChanged();

            //Sync the health UI at the start
            OnHealthUpdate?.Invoke(_currentHealth, _maxHealth);
            CharacterIndexChanged(false);
        }

        protected override void InitializeBaseStats()
        {
            base.InitializeBaseStats();

            _critChance = _playerBaseStats.CritChance;
            _critDamage = _playerBaseStats.CritDamage;
        }
        
        public void LoseControl() => _hasControl = false;
        public void GainControl() => _hasControl = true;
        
        public void SetDashing(bool isDashing) => _isDashing = isDashing;
        
        protected override void Update()
        {
            base.Update();
            
            foreach (var abilityTracker in _attackAbilityTrackers)
                abilityTracker.Update();
            foreach (var abilityTracker in _specialAbilityTrackers)
                abilityTracker.Update();
            
            _dashAbilityTracker.Update();
            
            MoveAndRotate();
            
            _rigidbody.angularVelocity = Vector3.zero;
            
            if(_interactables.Count > 0)
                FindMainInteractable();
        }

        private void MoveAndRotate()
        {
            var cameraForward = _camera.transform.forward;
            var downProjection = Vector3.Project(cameraForward, Vector3.up);
            
            var forwardDirection = (cameraForward - downProjection).normalized;
            var rightDirection = _camera.transform.right.normalized;
            
            Vector2 moveVector = _isDashing ? _dashInputSnapshot : _hasControl ? _moveInput : Vector2.zero;
            
            Vector3 movementX = moveVector.x * rightDirection;
            Vector3 movementZ = moveVector.y * forwardDirection;
            
            Vector3 movement = _moveSpeed * (movementX + movementZ).normalized;
            
            _rigidbody.linearVelocity = _charging ? Vector3.zero : movement;
            
            transform.LookAt(transform.position + movement);
        }

        private void StartAttack(AbilityStats stats, int useTimes, int animatorHash)
        {
            _currentAttackStats = stats;
            _currentAttackUseTimes = useTimes;
            
            CurrentAnimator.SetTrigger(animatorHash);
        }
        
        public void PerformAttack()
        {
            var stats = _currentAttackStats;
            
            var attackStats = new AttackStats(
                stats.AttackPrefab, 
                _damage, 
                _critChance, 
                _critDamage, 
                _areaSizeMultiplier);

            var position = AttackPosition;

            if (stats.Burst)
                StartCoroutine(AttackCoroutine(attackStats, _currentAttackUseTimes, 
                    stats.BurstDelay, stats.SpreadAngle, position));
            else
                Attack.Create(this, position, transform.rotation, attackStats);
        }

        public void PerformSpecial()
        {
            var stats = _currentAttackStats;
            
            var specialStats = new AttackStats(
                stats.AttackPrefab, 
                _damage, 
                _critChance, 
                _critDamage, 
                _areaSizeMultiplier);

            var position = _specialPoint[(int) ActiveCharacter].position;

            if (stats.Burst)
                StartCoroutine(AttackCoroutine(specialStats, _currentAttackUseTimes, 
                    stats.BurstDelay, stats.SpreadAngle, position));
            else
                Attack.Create(this, position, transform.rotation, specialStats);
        }

        private IEnumerator AttackCoroutine(AttackStats stats, int times, float delay, float spreadAngle, Vector3 position)
        {
            float halfAngle = spreadAngle * (times - 1) * 0.5f;
            
            for (int i = 0; i < times; i++)
            {
                float angle = spreadAngle * i - halfAngle;
                Quaternion rotation = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
                
                Attack.Create(this, position, rotation, stats);

                if (i != times - 1)
                    yield return new WaitForSeconds(delay);
            }
        }

        public void PerformAttackDash() => PerformDash(_fenrirAttackDashPoint.position, false);
        
        private void PerformDash(Vector3 dashPoint, bool animate)
        {
            // Projected dash vector using the calculated offset from player center to front
            Vector3 dashVector = dashPoint - _rigidbody.position;
            float distance = dashVector.magnitude;
            
            // Distance from center of player to the front collision point
            Vector3 collisionPointOffset =
                dashVector.normalized * 0.02f + _frontCollisionPoint.position - _rigidbody.position;
            
            Ray ray = new(_frontCollisionPoint.position, dashVector);
            bool hitWall = Physics.Raycast(ray, out var hit, distance, _wallLayer);
            if (hitWall) // Interpret holes as walls
            {
                dashPoint = hit.point - collisionPointOffset; // Subtract to get the center of the player after dash

                // Do new raycast for holes
                var holeColliders = Physics.OverlapSphere(dashPoint, _collider.radius, _holeLayer);
                if (holeColliders.Length > 0)
                {
                    // Calculate new dash vector
                    dashVector = dashPoint - _rigidbody.position;
                    distance = dashVector.magnitude;

                    ray = new(_rigidbody.position, dashVector);
                    bool hitHole = holeColliders[0].Raycast(ray, out hit, distance);

                    if (hitHole)
                        dashPoint = hit.point - collisionPointOffset;
                }
            }
            else // May dash over holes
            {
                print("didn't hit wall");
                
                var holeColliders = Physics.OverlapSphere(dashPoint, _collider.radius, _holeLayer);
                if (holeColliders.Length > 0)
                {
                    var holeCollider = holeColliders[0];
                    
                    Vector3 forwardRayOrigin = dashPoint - dashVector.normalized * 100f;
                    Ray forwardRay = new(forwardRayOrigin, dashVector);
                    holeCollider.Raycast(forwardRay, out hit, 10000);
                    
                    var forwardHitPoint = hit.point;
                    
                    Vector3 backwardRayOrigin = dashPoint + dashVector.normalized * 100f;
                    Ray backwardRay = new(backwardRayOrigin, -dashVector);
                    holeCollider.Raycast(backwardRay, out hit, 10000);

                    var backwardHitPoint = hit.point;
                    
                    float holeDiameter = Vector3.Distance(forwardHitPoint, backwardHitPoint);
                    if (holeDiameter > 200)
                        goto coroutine;
                    
                    float holeDashDistance = Vector3.Distance(forwardHitPoint, dashPoint);

                    float fraction = holeDashDistance / holeDiameter;

                    bool snapToOtherSide = fraction >= _dashHoleSnapFraction;
                    
                    if (snapToOtherSide)
                    {
                        ray = new(forwardHitPoint, dashVector);
                        distance = holeDiameter + 2 * _collider.radius + 0.02f;
                        
                        hitWall = Physics.Raycast(ray, out hit, distance, _wallLayer);
                        
                        if (hitWall)
                            snapToOtherSide = false;
                    }
                    
                    if (snapToOtherSide)
                        dashPoint = backwardHitPoint + collisionPointOffset;
                    else
                        dashPoint = forwardHitPoint - collisionPointOffset;
                }
            }

            coroutine:
            if (_dashCoroutine != null)
            {
                StopCoroutine(_dashCoroutine);
                _moveSpeed = _originalMoveSpeed;
            }
            _dashCoroutine = StartCoroutine(DashCoroutine(dashPoint, animate));
        }
        
        private IEnumerator DashCoroutine(Vector3 dashPoint, bool animate)
        {
            if (animate)
                CurrentAnimator.SetTrigger(DASH);
            
            _isDashing = true;
            _hasControl = false;
            _dashInputSnapshot = _lastMoveInput;

            int defaultPlayerLayer = gameObject.layer;
            
            int dashingPlayerLayer = _dashingPlayerLayer;
            int dashLayer = 0;
            while ((dashingPlayerLayer >>= 1) > 0)
                dashLayer++;
            
            gameObject.layer = dashLayer;

            float actualDashDistance = Vector3.Distance(transform.position, dashPoint);
            float dashDistanceFraction = actualDashDistance / _originalDashDistance;
            
            float dashDuration = _dashDuration * dashDistanceFraction;
            
            float dashSpeed = actualDashDistance / dashDuration;
            _originalMoveSpeed = _moveSpeed;
            _moveSpeed = dashSpeed;

            yield return new WaitForSeconds(dashDuration);
            
            gameObject.layer = defaultPlayerLayer;
            
            _isDashing = false;
            _hasControl = true;
            _dashInputSnapshot = Vector2.zero;

            float dashFadeDuration = dashDuration * _dashFade;
            
            if (dashFadeDuration <= 0)
            {
                _moveSpeed = _originalMoveSpeed;
                yield break;
            }
            
            float elapsedTime = 0;
            while (elapsedTime < dashFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / dashFadeDuration);
                
                _moveSpeed = Mathf.Lerp(dashSpeed, _originalMoveSpeed, t);
                
                yield return null;
            }
        }
        
        public override void TakeDamage(int amount, Entity attacker)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            base.TakeDamage(reducedDamage, attacker);
            OnHealthUpdate?.Invoke(_currentHealth, _maxHealth);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            OnHealthUpdate?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddItem(IItem item)
        {
            _items.Add(item);
            
            item.Apply(this);
        }

        private void CharacterIndexChanged(bool triggerSwitch = true)
        {
            for (int i = 0; i < _characterContainer.childCount; i++)
            {
                bool activeState = i == (int) ActiveCharacter;
                
                var character = _characterContainer.GetChild(i);
                character.gameObject.SetActive(activeState);
            }

            if (!triggerSwitch)
                return;
            
            CurrentAnimator.SetTrigger(SWITCH);
        }

        #region Stat Modification
        public void AddBaseMaxHealth(int amount)
        {
            _baseMaxHealth += amount;
            UpdateStats();
        }

        public void AddBaseDamage(int amount)
        {
            _baseDamage += amount;
            UpdateStats();
        }

        public void AddBaseMoveSpeed(float amount)
        {
            _baseMoveSpeed += amount;
            UpdateStats();
        }
        
        public void AddMaxHealthMultiplier(float amount)
        {
            _maxHealthMultiplier += amount;
            UpdateStats();
        }

        public void AddDamageMultiplier(float amount)
        {
            _damageMultiplier += amount;
            UpdateStats();
        }
        
        public void AddMoveSpeedMultiplier(float amount)
        {
            _moveSpeedMultiplier += amount;
            UpdateStats();
        }

        public void AddAreaSizeMultiplier(float amount)
        {
            _areaSizeMultiplier += amount;
        }
        
        public void AddCritChanceMultiplier(float amount)
        {
            _critChance += amount;
        }

        public void AddCritDamageMultiplier(float amount)
        {
            _critDamage += amount;
        }

        public void AddDamageReductionMultiplier(float amount)
        {
            _damageReduction += amount;
        }

        private void UpdateStats()
        {
            _maxHealth = Mathf.CeilToInt(_baseMaxHealth * _maxHealthMultiplier);
            _damage = Mathf.CeilToInt(_baseDamage * _damageMultiplier);
            _moveSpeed = _baseMoveSpeed * _moveSpeedMultiplier;
        }
        #endregion

        #region Collision

        private void FindMainInteractable()
        {
            if (_currentInteractable != null) 
                _currentInteractable.Highlighted = false;
            
            int lowestIndex = 0;
            float highestScore = 0;
            for (int i = 0; i < _interactables.Count; i++)
            {
                
                var between = (_interactables[i].Position - _rigidbody.position);
                var distance = between.magnitude;
                var direction = between/distance;

                float distScore = 1 - Mathf.Clamp01(distance/ 10f);
                var dot = Vector3.Dot(transform.forward, direction);

                float score = dot * _lookWeight + distScore * _distanceWeight;
                if (score > highestScore)
                {
                     lowestIndex = i;
                     highestScore = score;
                }
                   
            }
            
            _currentInteractable = _interactables[lowestIndex];
            _currentInteractable.Highlighted = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                _interactables.Add(other.GetComponent<IInteractable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                IInteractable interactable = other.GetComponent<IInteractable>();
                
                if(interactable == _currentInteractable) 
                    _currentInteractable.Highlighted = false;
                
                _interactables.Remove(interactable);
            }
        }
        #endregion
        
        #region Input
        public void MoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            
            bool isMoving = _moveInput.sqrMagnitude > 0.5f;
            if (isMoving)
                _lastMoveInput = _moveInput;
            
            CurrentAnimator.SetBool(IS_MOVING, isMoving);
        }

        public void InteractInput(InputAction.CallbackContext context)
        {
            if(!context.performed || !_hasControl) 
                return;

            if (_interactables.Count == 0 || _currentInteractable == null)
                return;
            
            _currentInteractable.Interacted();
            _currentInteractable.Highlighted = false;
            
            _interactables.Remove(_currentInteractable);
            _currentInteractable = null;
        }

        public void AttackInput(InputAction.CallbackContext context)
        {
            if (!_hasControl) 
                return;
            
            _charging = AttackAbilityTracker.RegisterInput(context);
            print(_charging);
        }

        public void SpecialInput(InputAction.CallbackContext context)
        {
            if (!_hasControl) 
                return;
            
            _charging = SpecialAbilityTracker.RegisterInput(context);
        }

        public void HealInput(InputAction.CallbackContext context)
        {
            if (!_hasControl || !GobletReady) return;
            
            Heal(_gobletHealAmount);
            GobletReady = false;
        }

        public void DashInput(InputAction.CallbackContext context)
        {
            if (!_hasControl) 
                return;
            
            _dashAbilityTracker.RegisterInput(context);
        }

        public void SwitchInput(InputAction.CallbackContext context)
        {
            if (!_hasControl) 
                return;
            
            int characterIndex = (int) ++ActiveCharacter % 2;
            ActiveCharacter = (Character) characterIndex;
            
            CharacterIndexChanged();
        }
        #endregion
    }
}
