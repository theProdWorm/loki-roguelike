using System;
using Stats;
using UI;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        private static readonly int MoveDir = Animator.StringToHash("MoveDir");
        private static GameObject _player;
        private UIEnemyHealth _healthBar;
        private Animator animator;
        private Vector3 prevPos = Vector3.zero;
        private float prevDot = 0;
        private BehaviorGraphAgent AiAgent;
        private NavMeshAgent navAgent;
        private AttackStats attackStats;

        [SerializeField] private Transform attackPoint;
        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private GameObject attackPrefab;
        
        protected override void Awake()
        {
            base.Awake();
            InitializeBaseStats();
            AiAgent = GetComponent<BehaviorGraphAgent>();
            navAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            if (!_player)
                _player = GameObject.FindGameObjectWithTag("Player");
            
            AiAgent.SetVariableValue("Target", _player);
            AiAgent.SetVariableValue("Animator", GetComponent<Animator>());
            AiAgent.Start();
            navAgent.speed = _moveSpeed;
            attackStats = new AttackStats(attackPrefab, _damage, 0, 0);

            _healthBar = GetComponentInChildren<UIEnemyHealth>();
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);
            
            
        }

        public void Attack()
        {
            Abilities.Attacks.Attack.Create(this, attackPoint.position, transform.rotation, attackStats);
        }

        public void AttackFinished()
        {
            AiAgent.SetVariableValue("Attacking", false);
        }

        
        
        protected override void Update()
        {
            base.Update();
         
            navAgent.speed = _moveSpeed;
            
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
            enabled = false;
            Destroy(_healthBar);
            
            foreach (Rigidbody rbC in GetComponentsInChildren<Rigidbody>())
            {
                rbC.isKinematic = false;
            }
        }


        public override int TakeDamage(int amount, Entity attacker)
        {
            int realDamage = base.TakeDamage(amount, attacker);
            base.TakeDamage(realDamage, attacker);
            DamageNumbers.CreateDamageNumber(transform, realDamage);
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);

            return realDamage;
        }

        public override void Heal(int amount)
        {
            base.Heal(amount);
            _healthBar.UpdateHealth(_currentHealth, _maxHealth);
        }
    }
}