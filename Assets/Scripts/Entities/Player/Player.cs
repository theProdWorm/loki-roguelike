using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Abilities.Attacks;
using StatusEffects;
using Items;
using Stats;
using StatusEffects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Entity
    {
        public enum Character { Fenrir, Hel, Jörmungandr }
        
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
        
        [Header("Dash")]
        [SerializeField] private Ability _dashAbility;
        
        [SerializeField] private Transform _dashPoint;

        [Range(0.01f, 0.5f)]
        [SerializeField] private float _dashDuration;

        [Tooltip("The fraction cutoff for dashing OVER holes")]
        [Range(0.5f, 1f)]
        [SerializeField] private float _dashHoleSnapFraction;

        [SerializeField] private LayerMask _dashingPlayerLayer;
        
        [Header("Fenrir")]
        [SerializeField] private CharacterAbilitySet _fenrirAbilities;
        [SerializeField] private Animator _fenrirAnimator;
        
        [Header("Hel")]
        [SerializeField] private CharacterAbilitySet _helAbilities;
        [SerializeField] private Animator _helAnimator;
        
        [Header("Jörmungandr")]
        [SerializeField] private CharacterAbilitySet _jörmungandrAbilities;
        [SerializeField] private Animator _jörmungandrAnimator;

        private Animator[] _animators;
        
        public Character ActiveCharacter;
        
        private AttackAbilityTracker[] _attackAbilityTrackers;
        private AttackAbilityTracker[] _specialAbilityTrackers;
        private AbilityTracker _dashAbilityTracker;
        
        private PlayerBaseStats _playerBaseStats;
        
        private Camera _camera;
        
        private Vector3 _rightDirection;
        private Vector3 _forwardDirection;
        
        private Vector2 _moveInput;
        private Vector2 _aimInput;
        
        private float _critChance;
        private float _critDamage;
        
        protected float _damageReduction = 0f;
        
        private readonly List<IItem> _items = new();

        private float _originalDashDistance;

        private bool _hasControl = true;
        
        private List<IInteractable> _interactables = new();
        private IInteractable _currentInteractable;

        protected override void Start()
        {
            _originalDashDistance = Vector3.Distance(transform.position, _dashPoint.position);

            _animators = new[]
            {
                _fenrirAnimator,
                _helAnimator,
                _jörmungandrAnimator
            };
            
            _attackAbilityTrackers = new AttackAbilityTracker[]
            {
                new(_fenrirAbilities.Attack, PerformAttack),
                new(_helAbilities.Attack, PerformAttack),
                //new(_jörmungandrAbilities.Attack, PerformAttack)
            };
            
            _specialAbilityTrackers = new AttackAbilityTracker[]
            {
                // new(_fenrirAbilities.Special, PerformAttack),
                // new(_helAbilities.Special, PerformAttack),
                // new(_jörmungandrAbilities.Special, PerformAttack)
            };
            
            _dashAbilityTracker = new(_dashAbility, PerformDash);
            
            _playerBaseStats = (PlayerBaseStats) EntityBaseStats;
            
            InitializeBaseStats();
            InitializeMovement();
            
            CharacterIndexChanged();
        }
        
        protected override void InitializeBaseStats()
        {
            base.InitializeBaseStats();

            _critChance = _playerBaseStats.CritChance;
            _critDamage = _playerBaseStats.CritDamage;
        }
        
        private void InitializeMovement()
        {
            _camera = Camera.main!;
            
            _rightDirection = _camera.transform.right.normalized;

            var cameraForward = _camera.transform.forward;
            var downProjection = Vector3.Project(cameraForward, Vector3.up);
            
            _forwardDirection = (cameraForward - downProjection).normalized;
        }
        
        protected override void Update()
        {
            base.Update();
            
            foreach (var abilityTracker in _attackAbilityTrackers)
                abilityTracker.Update();
            foreach (var abilityTracker in _specialAbilityTrackers)
                abilityTracker.Update();
            
            _dashAbilityTracker.Update();
            //_jörmungandrAbilityRecord.Update();
            
            if (_hasControl)
                MoveAndRotate();
            
            _rigidbody.angularVelocity = Vector3.zero;
            
            if(_interactables.Count > 0)
                FindMainInteractable();
        }

        private void MoveAndRotate()
        {
            Vector3 movementX = _moveInput.x * _rightDirection;
            Vector3 movementZ = _moveInput.y * _forwardDirection;
            
            Vector3 movement = _moveSpeed * (movementX + movementZ);
            
            _rigidbody.linearVelocity = movement;
            
            transform.LookAt(transform.position + movement);
        }

        private void PerformAttack(AbilityStats stats, int useTimes)
        {
            var attackStats = new AttackStats(
                stats.AttackPrefab, 
                _damage, 
                _critChance, 
                _critDamage, 
                _areaSizeMultiplier);

            if (stats.Burst)
                StartCoroutine(AttackCoroutine(attackStats, useTimes, stats.BurstDelay, stats.SpreadAngle));
            else
                Attack.Create(this, transform.position, transform.rotation, attackStats);
        }

        private IEnumerator AttackCoroutine(AttackStats stats, int times, float delay, float spreadAngle)
        {
            float halfAngle = spreadAngle * (times - 1) * 0.5f;
            
            for (int i = 0; i < times; i++)
            {
                float angle = spreadAngle * i - halfAngle;
                Quaternion rotation = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
                
                Attack.Create(this, transform.position, rotation, stats);

                if (i != times - 1)
                    yield return new WaitForSeconds(delay);
            }
        }

        private void PerformDash()
        {
            Vector3 dashPoint = _dashPoint.position;
            
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
            StartCoroutine(DashCoroutine(dashPoint));
        }
        
        private IEnumerator DashCoroutine(Vector3 dashPoint)
        {
            _hasControl = false;

            int defaultPlayerLayer = gameObject.layer;
            
            int dashingPlayerLayer = _dashingPlayerLayer;
            int dashLayer = 0;
            while ((dashingPlayerLayer >>= 1) > 0)
                dashLayer++;
            
            gameObject.layer = dashLayer;

            float actualDashDistance = Vector3.Distance(_rigidbody.position, dashPoint);
            
            float dashDistanceFraction = actualDashDistance / _originalDashDistance;
            float dashDuration = _dashDuration * dashDistanceFraction;
            float dashSpeed = actualDashDistance / dashDuration;

            if (dashDuration < 0.01f)
                goto stop;
            
            _rigidbody.linearVelocity = dashSpeed * transform.forward;

            yield return new WaitForSeconds(dashDuration);
            
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.position = dashPoint;
            
            stop:
            _hasControl = true;
            gameObject.layer = defaultPlayerLayer;
        }
        
        public override void TakeDamage(int amount, Entity attacker)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            base.TakeDamage(reducedDamage, attacker);
        }
        
        public void AddItem(IItem item)
        {
            _items.Add(item);
            
            item.Apply(this);
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
        }

        public void AimInput(InputAction.CallbackContext context)
        {
            _aimInput = context.ReadValue<Vector2>();
        }

        public void InteractInput(InputAction.CallbackContext context)
        {
            if(!context.performed) 
                return;

            if (_currentInteractable == null)
                return;
        
            print("interacted!");
            
            _currentInteractable.Interacted();
            _currentInteractable.Highlighted = false;
            
            _interactables.Remove(_currentInteractable);
            _currentInteractable = null;
        }
        
        public void AttackInput(InputAction.CallbackContext context) =>
            _attackAbilityTrackers[(int) ActiveCharacter].RegisterInput(context);
        
        public void SpecialInput(InputAction.CallbackContext context) =>
             _specialAbilityTrackers[(int) ActiveCharacter].RegisterInput(context);
        
        public void DashInput(InputAction.CallbackContext context) =>
            _dashAbilityTracker.RegisterInput(context);

        public void PreviousInput(InputAction.CallbackContext context)
        {
            int characterIndex = Mathf.Abs((int) --ActiveCharacter) % 2;
            ActiveCharacter = (Character) characterIndex;
            
            CharacterIndexChanged();
        }

        public void NextInput(InputAction.CallbackContext context)
        {
            int characterIndex = (int) ++ActiveCharacter % 2;
            ActiveCharacter = (Character) characterIndex;
            
            CharacterIndexChanged();
        }
        #endregion
    }
}
