using System.Collections.Generic;
using Abilities;
using Abilities.Attacks;
using Items;
using Stats;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    public class Player : Entity
    {
        public enum Character { Fenrir, Hel, Jörmungandr }
        
        [SerializeField] private Rigidbody _rigidbody;
        
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

        private void Start()
        {
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
            MoveAndRotate();
        }

        private void MoveAndRotate()
        {
            Vector3 movementX = _moveInput.x * _rightDirection;
            Vector3 movementZ = _moveInput.y * _forwardDirection;
            
            Vector3 movement = _moveSpeed * (movementX + movementZ);
            
            _rigidbody.linearVelocity = movement;
            
            transform.LookAt(_rigidbody.position + movement);
        }
        
        public override void TakeDamage(int amount)
        {
            int reducedDamage = Mathf.CeilToInt(amount * (1 - _damageReduction));
            base.TakeDamage(reducedDamage);
        }
        
        public void AddItem(IItem item)
        {
            _items.Add(item);
            
            item.Apply(this);
        }

        private void CharacterIndexChanged()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                bool activeState = i == (int) ActiveCharacter;
                
                var child = transform.GetChild(i);
                child.gameObject.SetActive(activeState);
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
        
        #region Input
        public void Move(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void Aim(InputAction.CallbackContext context)
        {
            _aimInput = context.ReadValue<Vector2>();
        }
        
        public void Attack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            var ability = ActiveCharacter switch
            {
                Character.Hel => _helAbilities.Attack,
                Character.Jörmungandr => _jörmungandrAbilities.Attack,
                _ => _fenrirAbilities.Attack // Default to Fenrir
            };

            var attack = Instantiate(ability.Prefab, transform.position, transform.rotation)
                .GetComponent<Attack>();

            if (attack is AreaAttack areaAttack)
                areaAttack.AreaSizeMultiplier = _areaSizeMultiplier;

            // TODO: Perform attack
        }
        
        public void Special(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            // TODO: Perform special
        }
        
        public void Dash(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            // TODO: Perform dash
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
