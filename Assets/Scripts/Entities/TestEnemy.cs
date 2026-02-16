using System;
using Stats;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class TestEnemy : Entity
    {
        private static GameObject _player;
        private UIEnemyHealth _healthbar;

        private Rigidbody rb;

        private BehaviorGraphAgent AiAgent;
        private NavMeshAgent navAgent;

        [SerializeField] private Blackboard blackboard;
        [SerializeField] private BehaviorGraph behaviorGraph;
        public float attackCooldown;

        public GameObject attackPrefab;
        public AttackStats attackStats;

        protected override void Awake()
        {
            base.Awake();
            InitializeBaseStats();
            rb = GetComponent<Rigidbody>();
            AiAgent = GetComponent<BehaviorGraphAgent>();
            navAgent = GetComponent<NavMeshAgent>();
            if (!_player)
                _player = GameObject.FindGameObjectWithTag("Player");

            AiAgent.Graph = behaviorGraph;
            AiAgent.Start();
            AiAgent.SetVariableValue("Target", _player);
            AiAgent.SetVariableValue("AttackDelay", attackCooldown);
            AiAgent.SetVariableValue("Animator", GetComponent<Animator>());
            navAgent.speed = _moveSpeed;
            attackStats = new AttackStats(attackPrefab, 5, 0, 0, 1);

            _healthbar = GetComponentInChildren<UIEnemyHealth>();
            _healthbar.UpdateHealth(_currentHealth, _maxHealth);
        }

        public void Attack()
        {
            Abilities.Attacks.Attack.Create(this, transform.position, transform.rotation, attackStats);
        }

        public void AttackFinished()
        {
            AiAgent.SetVariableValue("Attacking", false);
        }

        private void Update()
        {
            //var rotation = Quaternion.LookRotation(Player.transform.position - transform.position, Vector3.up);
            transform.LookAt(_player.transform, Vector3.up);
            var rot = transform.eulerAngles;
            transform.eulerAngles = new Vector3(0, rot.y, 0);
        }

        public void Destroy()
        {
            AiAgent.End();
            navAgent.enabled = false;
            rb.constraints = RigidbodyConstraints.None;
            tag = "Untagged";
            this.enabled = false;
        }


        public override void TakeDamage(int amount, Entity attacker)
        {
            base.TakeDamage(amount, attacker);
            Debug.Log($"Took {amount} damage and now has {_currentHealth} health");
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
