using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Abilities.Attacks;
using Items;
using Stats;
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

        public Character ActiveCharacter;
        
        private CharacterAbilityRecord _fenrirAbilityRecord;
        private CharacterAbilityRecord _helAbilityRecord;
        private CharacterAbilityRecord _jörmungandrAbilityRecord;
        
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
        private readonly List<Effect> _effects = new();

        private float _originalDashDistance;

        private bool _hasControl = true;
        
        private List<IInteractable> _interactables = new();
        private IInteractable _currentInteractable;

        protected override void Start()
        {
            _originalDashDistance = Vector3.Distance(transform.position, _dashPoint.position);

            _fenrirAbilityRecord = new(_fenrirAbilities);
            //_helAbilityRecord = new(_helAbilities);
            //_jörmungandrAbilityRecord = new(_jörmungandrAbilities);
            
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
        
        private void Update()
        {
            _fenrirAbilityRecord.Update();
            //_helAbilityRecord.Update();
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
            if(_currentInteractable !=null)
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
                IInteractable inter = other.GetComponent<IInteractable>();
                if(inter == _currentInteractable) 
                    _currentInteractable.Highlighted = false;
                _interactables.Remove(inter);
            }
        }
        #endregion
        
        #region Input
        public void Move(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void Aim(InputAction.CallbackContext context)
        {
            _aimInput = context.ReadValue<Vector2>();
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            if (_currentInteractable != null)
            {
                print("interacted!");
                _currentInteractable.Interacted();
                _currentInteractable.Highlighted = false;
                _interactables.Remove(_currentInteractable);
                _currentInteractable = null;
            }
                
        }
        
        public void Attack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var record = ActiveCharacter switch
            {
                Character.Hel => _helAbilityRecord,
                Character.Jörmungandr => _jörmungandrAbilityRecord,
                _ => _fenrirAbilityRecord // Default to Fenrir
            };

            if (!record.TryUseAttack())
                return;
            
            var ability = ActiveCharacter switch
            {
                Character.Hel => _helAbilities.Attack,
                Character.Jörmungandr => _jörmungandrAbilities.Attack,
                _ => _fenrirAbilities.Attack // Default to Fenrir
            };
            

            AttackStats attackStats = new(_damage, _critChance, _critDamage, _areaSizeMultiplier);
            
            InstantiateAttack(ability.Prefab, attackStats);
        }
        
        public void Special(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            // TODO: Perform special
        }
        
        public void Dash(InputAction.CallbackContext context)
        {
            if (!context.performed || !_hasControl)
                return;

            var record = ActiveCharacter switch
            {
                Character.Hel => _helAbilityRecord,
                Character.Jörmungandr => _jörmungandrAbilityRecord,
                _ => _fenrirAbilityRecord
            };

            if (!record.TryUseDash())
                return;
            
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
                    if (holeDiameter > 200 || holeDiameter < .1f)
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

        public void SwitchPrevious(InputAction.CallbackContext context)
        {
            int characterIndex = Mathf.Abs((int) --ActiveCharacter) % 2;
            ActiveCharacter = (Character) characterIndex;
            
            CharacterIndexChanged();
        }

        public void SwitchNext(InputAction.CallbackContext context)
        {
            int characterIndex = (int) ++ActiveCharacter % 2;
            ActiveCharacter = (Character) characterIndex;
            
            CharacterIndexChanged();
        }
        #endregion
    }
}
