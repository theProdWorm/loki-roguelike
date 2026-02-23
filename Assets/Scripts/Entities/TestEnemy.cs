using System;
using Stats;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class TestEnemy : Entity
    {
        private static readonly int MoveDir = Animator.StringToHash("MoveDir");
        private static GameObject _player;
        private UIEnemyHealth _healthbar;

        private Rigidbody rb;
        private Animator animator;

        private BehaviorGraphAgent AiAgent;
        private NavMeshAgent navAgent;

        public Transform attackPoint;
        [SerializeField] private Blackboard blackboard;
        [SerializeField] private BehaviorGraph behaviorGraph;
        public float attackCooldown;
        public float rotationSpeed = 5;
        public GameObject attackPrefab;
        public AttackStats attackStats;

        protected override void Awake()
        {
            base.Awake();
            InitializeBaseStats();
            rb = GetComponent<Rigidbody>();
            AiAgent = GetComponent<BehaviorGraphAgent>();
            navAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            if (!_player)
                _player = GameObject.FindGameObjectWithTag("Player");

            AiAgent.Graph = behaviorGraph;
            AiAgent.Start();
            AiAgent.SetVariableValue("Target", _player);
            AiAgent.SetVariableValue("AttackDelay", attackCooldown);
            AiAgent.SetVariableValue("Animator", GetComponent<Animator>());
            navAgent.speed = _moveSpeed;
            attackStats = new AttackStats(attackPrefab, _damage, 0, 0, 1);

            _healthbar = GetComponentInChildren<UIEnemyHealth>();
            _healthbar.UpdateHealth(_currentHealth, _maxHealth);
            
        }

        public void Attack()
        {
            Abilities.Attacks.Attack.Create(this, attackPoint.position, transform.rotation, attackStats);
        }

        public void AttackFinished()
        {
            AiAgent.SetVariableValue("Attacking", false);
        }

        
        private Vector3 prevPos = Vector3.zero;
        private float prevDot = 0;
        private void Update()
        {
            var pos = transform.position;
            var rotation = Quaternion.LookRotation(_player.transform.position - transform.position, Vector3.up);
            var lerpRot = Quaternion.Lerp(transform.rotation,rotation , Time.deltaTime * rotationSpeed);
            var rot = lerpRot.eulerAngles;
            transform.eulerAngles = new Vector3(0, rot.y, 0);
            //transform.LookAt(_player.transform, Vector3.up);
            //var rot = transform.eulerAngles;
            //transform.eulerAngles = new Vector3(0, rot.y, 0);
            
            if(Vector3.Distance(pos, prevPos) < 0.1f) return;
            var between = (pos - prevPos);
            var distance = between.magnitude;
            var direction = between/distance;
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
            AiAgent.End();
            navAgent.enabled = false;
            tag = "Untagged";
            animator.enabled = false;
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
            Destroy(_healthbar);
            
            foreach (Rigidbody rbC in GetComponentsInChildren<Rigidbody>())
            {
                rbC.isKinematic = false;
            }
            //ragdollRoot.parent = null;
        }


        public override void TakeDamage(int amount, Entity attacker)
        {
            base.TakeDamage(amount, attacker);
            DamageNumbers.CreateDamageNumber(transform,amount);
            _healthbar.UpdateHealth(_currentHealth, _maxHealth);
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            Debug.Log($"Healed {amount} health and now has {_currentHealth} health");
            _healthbar.UpdateHealth(_currentHealth, _maxHealth);
        }
    }
}