using System;
using System.Linq;
using Animation;
using Effects;
using Stats;
using StatusEffects;
using StatusEffects.Effects;
using UI;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Entities
{
    public class Enemy : Entity
    {
        private static readonly int MOVE_DIR = Animator.StringToHash("MoveDir");
        private static readonly int MOVE_SPEED = Animator.StringToHash("MoveSpeed");
        private static readonly int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
        
        private static int ENEMYAMOUNT = 0;
        private static GameObject PLAYER;

        private UIEnemyHealth _healthBar;
        private Animator animator;
        private Vector3 prevPos = Vector3.zero;
        private float prevDot = 0;
        private BehaviorGraphAgent AiAgent;
        private BlackboardVariable<ChargePrep> ChargePrepEventChannel;
        private NavMeshAgent navAgent;
        private AttackStats attackStats;
        private bool ragdollActive;
        private float ragdollTimeLeft;
        private float dissolveTimeLeft;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private Material[] materials;
        
        [SerializeField] private GameObject attackPrefab;
        [Tooltip("Where the attack will spawn")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] EncounterManager.EnemyTypes type;
        [SerializeField] private bool canBeStaggered;
        public bool HasSpawned = true;

        [SerializeField] private GameObject _iceBlockPrefab;
        
        [Header("Death")]
        [Tooltip("How long the ragdoll lasts before starting to dissolve")]
        [SerializeField] private float ragdollDuration = 1f;
        [Tooltip("How long it takes for the ragdoll to dissolve")]
        [SerializeField] private float dissolveDuration = 1f;

        private StatusEffectList _statusEffects;

        private float _animationSpeed;
        
        protected override void Awake()
        {
            base.Awake();
            _statusEffects = new(this);
            
            InitializeBaseStats();
            AiAgent = GetComponent<BehaviorGraphAgent>();
            navAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            materials = _skinnedMeshRenderer.materials;
            if (!PLAYER)
                PLAYER = GameObject.FindGameObjectWithTag("Player");

            AiAgent.SetVariableValue("Target", PLAYER);
            AiAgent.SetVariableValue("Animator", GetComponent<Animator>());
            AiAgent.Start();
            navAgent.speed = _moveSpeed;
            attackStats = new AttackStats(attackPrefab, _damage, 0, 0);

            _healthBar = GetComponentInChildren<UIEnemyHealth>();
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);
            _animationSpeed = animator.GetFloat(MOVE_SPEED);

            ENEMYAMOUNT++;

            switch (type)
            {
                case EncounterManager.EnemyTypes.Wolf:
                {
                    if (AiAgent.GetVariable("ChargePrep", out ChargePrepEventChannel))
                    {
                   
                    }
                    else throw new NullReferenceException();

                    break;
                }
            }
        }

        public void ChargeReady()
        {
            ChargePrepEventChannel.Value.SendEventMessage();
        }
        public void AttackFinished()
        {
            AiAgent.SetVariableValue("Attacking", false);
        }
        
        public void Attack()
        {
            if (type == EncounterManager.EnemyTypes.BirdOnBird)
            {
                attackStats.Prefab.GetComponent<HomingProjectileAttack>().target = PLAYER.transform;
                Abilities.Attacks.Attack.Create(this, attackPoint.position, Quaternion.LookRotation(PLAYER.transform.position - transform.position) , attackStats);
            }
            else 
                Abilities.Attacks.Attack.Create(this, attackPoint.position, transform.rotation, attackStats);
        }
        
        private float dissolveValue;
        private void Update()
        {
            if (IsDead)
            {
                if (!ragdollActive) return;
                if (ragdollTimeLeft > 0)
                {
                    ragdollTimeLeft -= Time.deltaTime ;
                }
                else
                {
                    dissolveTimeLeft -= Time.deltaTime;
                    materials[0].SetFloat("_Cutoff_Height", Mathf.InverseLerp(0,dissolveDuration,dissolveTimeLeft));
                    if (!(dissolveTimeLeft <= 0)) return;
                    ragdollActive = false;
                    Destroy(gameObject);
                }
                return;
            }
            
            _statusEffects.Update();

            //navAgent.speed = _moveSpeed;

            var pos = transform.position;
            // var rotation = Quaternion.LookRotation(PLAYER.transform.position - transform.position, Vector3.up);
            // var lerpRot = Quaternion.Lerp(transform.rotation,rotation , Time.deltaTime * rotationSpeed);
            // var rot = lerpRot.eulerAngles;
            // transform.eulerAngles = new Vector3(0, rot.y, 0);
            //transform.LookAt(_player.transform, Vector3.up);
            //var rot = transform.eulerAngles;
            //transform.eulerAngles = new Vector3(0, rot.y, 0);

            if (Vector3.Distance(pos, prevPos) < 0.1f) return;
            var between = (pos - prevPos);
            var distance = between.magnitude;
            var direction = between / distance;
            var dot = Vector3.Dot(transform.forward, direction);
            float velocity = .1f;

            float smoothed = 0;
            smoothed = Mathf.SmoothDamp(
                prevDot,
                dot,
                ref velocity,
                .05f
            );

            prevDot = smoothed;
            prevPos = transform.position;
            
            if (type == EncounterManager.EnemyTypes.BirdOnBird) return;
            animator.SetFloat(MOVE_DIR, smoothed);

            
        }
        
        public void ApplyStatusEffect(StatusEffect effect) => 
            _statusEffects.Add(effect);
        public void RemoveAllStatusEffectsOfType<T>() where T : StatusEffect => 
            _statusEffects.RemoveAll<T>();
        public int  CountStatusEffectsOfType<T>() where T : StatusEffect => 
            _statusEffects.GetCount<T>();
        public bool HasStatusEffectOfType<T>() where T : StatusEffect =>
            _statusEffects.HasEffect<T>();

        public void Destroy()
        {
            ENEMYAMOUNT--;
            AiAgent.End();
            navAgent.enabled = false;
            tag = "Untagged";
            //enabled = false;
            //TODO Destroy upon ragdoll deletion
            Destroy(AiAgent);
            Destroy(navAgent);
            Destroy(animator);
            Destroy(GetComponent<Collider>());
            Destroy(_healthBar.gameObject);

            foreach (Rigidbody rbC in GetComponentsInChildren<Rigidbody>(true))
            {
                rbC.gameObject.SetActive(true);
                rbC.isKinematic = false;
            }

            ragdollActive = true;
            ragdollTimeLeft = ragdollDuration;
            dissolveTimeLeft = dissolveDuration;
            
            Unfreeze();
        }

        public override int TakeDamage(int amount, Entity attacker)
        {
            if(!HasSpawned)
                return 0;

            if (HasStatusEffectOfType<StatusEffect_Frozen>() && 
                attacker is Player player)
            {
                if (player.ActiveCharacter == Player.Character.Fenrir)
                {
                    amount *= 2;
                    RemoveAllStatusEffectsOfType<StatusEffect_Frozen>();
                }
                else
                {
                    amount = Mathf.CeilToInt(amount * 0.5f);
                }
            }
            
            int realDamage = base.TakeDamage(amount, attacker);
            DamageNumbers.CreateDamageNumber(transform, realDamage);
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);

            if (canBeStaggered)
            {
                //animator.StopPlayback();
                if(type != EncounterManager.EnemyTypes.BirdOnBird) animator.SetBool("Stagger",true);
                AiAgent.SetVariableValue("Staggered", true);
                AiAgent.SetVariableValue("Attacking", false);
            }

            return realDamage;
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);
        }

        private IceBlock _iceBlockInstance;
        private bool _frozen;
        public void Freeze()
        {
            AiAgent.SetVariableValue("Frozen", true);
            animator.SetFloat(MOVE_SPEED, 0);

            _iceBlockInstance = Instantiate(_iceBlockPrefab, transform.position, transform.rotation)
                .GetComponent<IceBlock>();

            _frozen = true;
        }
        public void Unfreeze()
        {
            if (!_frozen)
                return;
            
            AiAgent.SetVariableValue("Frozen", false);
            animator.SetFloat(MOVE_SPEED, 1);
            
            _iceBlockInstance.Shatter();
            _frozen = false;
        }
        
        public override void AddSpeedMultiplier(float amount)
        {
            base.AddSpeedMultiplier(amount);
            navAgent.speed = _moveSpeed;
            
            float animationSpeed = _animationSpeed * _speedMultiplier;
            animator.SetFloat(MOVE_SPEED, animationSpeed);
            animator.SetFloat(ATTACK_SPEED, animationSpeed);
        }
        public override void RemoveSpeedMultiplier(float amount)
        {
            base.RemoveSpeedMultiplier(amount);
            navAgent.speed = _moveSpeed;
            
            float animationSpeed = _animationSpeed * _speedMultiplier;
            animator.SetFloat(MOVE_SPEED, animationSpeed);
            animator.SetFloat(ATTACK_SPEED, animationSpeed);
        }
    }
}