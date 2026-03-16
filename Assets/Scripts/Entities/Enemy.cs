using System;
using System.Linq;
using Stats;
using UI;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Entities
{
    public class Enemy : Entity
    {
        private static readonly int MoveDir = Animator.StringToHash("MoveDir");
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
        
        [Header("Death")]
        [Tooltip("How long the ragdoll lasts before starting to dissolve")]
        [SerializeField] private float ragdollDuration = 1f;
        [Tooltip("How long it takes for the ragdoll to dissolve")]
        [SerializeField] private float dissolveDuration = 1f;

        

        protected override void Awake()
        {
            base.Awake();
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
            

            ENEMYAMOUNT++;

            if (type == EncounterManager.EnemyTypes.Wolf)
            {
                if (AiAgent.GetVariable("ChargePrep", out ChargePrepEventChannel))
                {
                   
                }
                else throw new NullReferenceException();

            }
        }

        public void ChargeReady()
        {
            ChargePrepEventChannel.Value.SendEventMessage();
        }

        public void Attack()
        {
            Abilities.Attacks.Attack.Create(this, attackPoint.position, transform.rotation, attackStats);
        }

        public void AttackFinished()
        {
            AiAgent.SetVariableValue("Attacking", false);
        }

        private float dissolveValue;
        protected override void Update()
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
            base.Update();

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

            animator.SetFloat(MoveDir, smoothed);

            prevPos = transform.position;
        }

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
        }

        public override int TakeDamage(int amount, Entity attacker)
        {
            if(!HasSpawned)
                return 0;

            int realDamage = base.TakeDamage(amount, attacker);
            DamageNumbers.CreateDamageNumber(transform, realDamage);
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);

            if (canBeStaggered)
            {
                animator.StopPlayback();
                animator.SetBool("Stagger",true);
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
    }
}