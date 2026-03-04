using Abilities;
using Abilities.Attacks;
using Items;
using Stats;
using System.Collections;
using System.Collections.Generic;
using Entities.Stats;
using Gameplay.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Entity
    {
        public enum Character { Fenrir, Hel }
        
        private static readonly int IS_MOVING = Animator.StringToHash("isMoving");
        private static readonly int DASH      = Animator.StringToHash("dash");
        private static readonly int ATTACK    = Animator.StringToHash("attack");
        private static readonly int SPECIAL   = Animator.StringToHash("special");
        private static readonly int SWITCH    = Animator.StringToHash("switch");

        public UnityEvent<int, int> OnHealthChanged;
        public UnityEvent<int, int> OnPotionChargesChanged;
        
        [SerializeField] private Transform _characterContainer;
        [SerializeField] private PlayerInput _playerInput;

        [Tooltip("Amount of time (in seconds) in advance the player can press an input for it to count.")]
        [SerializeField] private float _inputBufferMargin;
        
        [Header("Movement")]
        [SerializeField] private float _animationLockMoveSpeedFadeDuration;
        
        [Header("Collision")]
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Transform _frontCollisionPoint;

        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private LayerMask _holeLayer;

        [Header("Interaction")]
        [SerializeField] private float _lookWeight;
        [SerializeField] private float _distanceWeight;

        [Header("Healing")]
        [SerializeField] private int _potionHealAmount;
        [SerializeField] private int _potionCost;
        [SerializeField] private int _maxPotionCharges;
        
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
        [SerializeField] private float     _fenrirLungeForce;
        [SerializeField] private float     _fenrirLungeDuration;
        
        [Header("Hel")]
        [SerializeField] private CharacterAbilitySet _helAbilities;
        [SerializeField] private Animator  _helAnimator;
        [SerializeField] private Transform _helAttackPoint;
        [SerializeField] private Transform _helSpecialPoint;
        [SerializeField] private float     _helLungeForce;
        [SerializeField] private float     _helLungeDuration;
            
        public UnityEvent OnHelAttackStageChanged;
        public UnityEvent OnHelAttackFullyCharged;
        public UnityEvent OnHelAttackReleased;
        
        private Animator[] _animators;
        private Animator CurrentAnimator => _animators[(int) ActiveCharacter];
        
        public Character ActiveCharacter;
        
        private AttackAbilityTracker[] _attackAbilityTrackers;
        private AttackAbilityTracker[] _specialAbilityTrackers;
        private AttackAbilityTracker[] _switchAbilityTrackers;
        private AbilityTracker _dashAbilityTracker;
        private AttackAbilityTracker AttackAbilityTracker => _attackAbilityTrackers[(int) ActiveCharacter];
        private AttackAbilityTracker SpecialAbilityTracker => _specialAbilityTrackers[(int) ActiveCharacter];
        private AttackAbilityTracker SwitchAbilityTracker => _switchAbilityTrackers[(int) ActiveCharacter];

        private Transform[] _attackPoints;
        private Transform[] _specialPoint;

        private Vector3 AttackPosition => _attackPoints[(int) ActiveCharacter].position;
        private Vector3 SpecialPosition => _specialPoint[(int) ActiveCharacter].position;
        
        private Ability _currentAbility;
        private int     _currentAbilityUseTimes;
        
        private PlayerBaseStats _playerBaseStats;
        
        private Camera _camera;
        
        private Vector2 _moveInput;
        private Vector2 _lastMoveInput;
        private Vector2 _dashInputSnapshot;
        
        private float _critChance;
        private float _critDamage;
        
        protected float _damageReduction = 0f;
        
        private int _potionCharges;
        private bool PotionReady => _potionCharges >= _potionCost;

        private float _originalDashDistance;
        private float _originalMoveSpeed;
        private Coroutine _dashCoroutine;

        private bool  _isDashing;
        private bool  _hasControl = true;
        private float _controlLossDuration;
        
        private InputBuffer _inputBuffer;
        
        private List<IInteractable> _interactables = new();
        private IInteractable _currentInteractable;

        protected override void Start()
        {
            _playerInput.SwitchCurrentActionMap("Dialogue");
            _playerInput.SwitchCurrentActionMap("UI");
            _playerInput.SwitchCurrentActionMap("Player");
            
            _camera = Camera.main!;
            
            _inputBuffer = new(_inputBufferMargin);
            
            _playerBaseStats = (PlayerBaseStats) EntityBaseStats;
            InitializeBaseStats();
            
            _originalMoveSpeed = _moveSpeed;
            _originalDashDistance = Vector3.Distance(transform.position, _dashPoint.position);
            
            InitializeAbilityTrackers();
            InitializeAttackPoints();
            InitializeAnimators();
            
            OnDamageDealt.AddListener(AddPotionCharges);
            
            //Sync the health UI at the start
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            CharacterIndexChanged();
        }

        private void InitializeAbilityTrackers()
        {
            _attackAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK)),
                new(_helAbilities.Attack, (ability, action) =>
                    StartAttack(ability, action, ATTACK))
            };

            _switchAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Switch, (ability, action) =>
                    StartAttack(ability, action, SWITCH)),
                new(_helAbilities.Switch, (ability, action) =>
                    StartAttack(ability, action, SWITCH))
            };

            _dashAbilityTracker = new(_dashAbility, () => PerformDash(_dashPoint.position, true));
        }

        private void InitializeAttackPoints()
        {
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
        }

        private void InitializeAnimators()
        {
            _animators = new[]
            {
                _fenrirAnimator,
                _helAnimator
            };
        }
        
        protected override void InitializeBaseStats()
        {
            base.InitializeBaseStats();

            _critChance = _playerBaseStats.CritChance;
            _critDamage = _playerBaseStats.CritDamage;
        }
        
        public void LoseControl() => _hasControl = true;
        public void GainControl() => _hasControl = true;
        
        public void SetDashing(bool isDashing) => _isDashing = isDashing;
        
        protected override void Update()
        {
            base.Update();
            
            _inputBuffer.Update();
            if (_hasControl)
                _inputBuffer.NextInput();
            
            foreach (var attackAbilityTracker in _attackAbilityTrackers)
                attackAbilityTracker.Update();
            foreach (var switchAbilityTracker in _switchAbilityTrackers)
                switchAbilityTracker.Update();
            
            _dashAbilityTracker.Update();

            if (!_hasControl && !_isDashing)
            {
                if (_animationLockMoveSpeedFadeDuration == 0)
                {
                    _moveSpeed = 0;
                }
                else
                {
                    float t = Mathf.Clamp01(_controlLossDuration / _animationLockMoveSpeedFadeDuration);
                    _moveSpeed = Mathf.Lerp(_originalMoveSpeed, 0, t);
                }
            }
            else if (!_isDashing)
            {
                _moveSpeed = _originalMoveSpeed;
            }
            
            MoveAndRotate();
            
            _rigidbody.angularVelocity = Vector3.zero;
            
            if(_interactables.Count > 0)
                FindMainInteractable();
            
            if (!_hasControl)
                _controlLossDuration += Time.deltaTime;
            else
                _controlLossDuration = 0;
        }

        private void MoveAndRotate()
        {
            var cameraForward = _camera.transform.forward;
            var downProjection = Vector3.Project(cameraForward, Vector3.up);
            
            var forwardDirection = (cameraForward - downProjection).normalized;
            var rightDirection = _camera.transform.right.normalized;
            
            Vector2 moveVector = _isDashing ? _dashInputSnapshot : _moveInput;
            
            Vector3 movementX = moveVector.x * rightDirection;
            Vector3 movementZ = moveVector.y * forwardDirection;
            
            Vector3 movement = _moveSpeed * (movementX + movementZ).normalized;
            
            _rigidbody.linearVelocity = movement;
            
            transform.LookAt(transform.position + movement);

            _rigidbody.linearVelocity += _knockbackForce;
        }

        private void StartAttack(Ability ability, int useTimes, int animatorHash)
        {
            _currentAbility = ability;
            _currentAbilityUseTimes = useTimes;
            
            CurrentAnimator.SetTrigger(animatorHash);
        }
        
        public void PerformAttack(Transform attackPoint)
        {
            if (!_currentAbility)
                return;
            
            var attackStats = new AttackStats(
                _currentAbility.AttackPrefab, 
                _damage, 
                _critChance, 
                _critDamage);

            var position = attackPoint.position;
            
            if (_currentAbility.Burst)
                StartCoroutine(AttackCoroutine(attackStats, _currentAbilityUseTimes, 
                    _currentAbility.BurstDelay, _currentAbility.SpreadAngle, position));
            else
                Attack.Create(this, position, transform.rotation, attackStats);
        }
        
        public void PerformAttackLunge()
        {
            switch (ActiveCharacter)
            {
                default:
                case Character.Fenrir:
                    KnockBack(transform.forward, _fenrirLungeForce, _fenrirLungeDuration);
                    break;
                case Character.Hel:
                    KnockBack(-transform.forward, _helLungeForce, _helLungeDuration);
                    break;
            }
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
            _hasControl = true;
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
        
        public override int TakeDamage(int amount, Entity attacker)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            int realDamage = base.TakeDamage(reducedDamage, attacker);
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            return realDamage;
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddPotionCharges(Entity _, int damage)
        {
            if (_potionCharges >= _maxPotionCharges)
                return;
            
            Debug.Log($"Added {damage} potion charges");
            
            _potionCharges += damage;
            OnPotionChargesChanged?.Invoke(_potionCharges, _maxPotionCharges);
        }

        private void CharacterIndexChanged()
        {
            for (int i = 0; i < _characterContainer.childCount; i++)
            {
                bool activeState = i == (int) ActiveCharacter;
                
                var character = _characterContainer.GetChild(i);
                character.gameObject.SetActive(activeState);
            }
        }

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
            if(!_hasControl || !context.performed) 
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
            if (!context.performed)
                return;

            _inputBuffer.Add(AttackAbilityTracker.TryUse);
        }

        public void SpecialInput(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            _inputBuffer.Add(SpecialAbilityTracker.TryUse);
        }

        public void HealInput(InputAction.CallbackContext context)
        {
            if (!context.performed || !PotionReady || _currentHealth >= _maxHealth)
                return;
            
            _inputBuffer.Add(() =>
            {
                Heal(_potionHealAmount);
                
                _potionCharges -= _potionCost;
                OnPotionChargesChanged?.Invoke(_potionCharges, _maxPotionCharges);

                return true;
            });
        }

        public void DashInput(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            _inputBuffer.Add(_dashAbilityTracker.TryUse);
        }

        public void SwitchInput(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            _inputBuffer.Add(() =>
            {
                ActiveCharacter = (Character) ((int) ++ActiveCharacter % 2);

                if (!SwitchAbilityTracker.TryUse())
                {
                    ActiveCharacter = (Character) ((int) ++ActiveCharacter % 2);
                    return false;
                }

                foreach (var tracker in _switchAbilityTrackers)
                    tracker.Reset();
                
                CharacterIndexChanged();
                return true;
            });
        }
        #endregion
    }
}
