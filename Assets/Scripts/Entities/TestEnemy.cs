using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
public class TestEnemy : Entity
{
    private static GameObject _player;

    private Rigidbody rb;
    
    private BehaviorGraphAgent AiAgent;
    private NavMeshAgent navAgent;

    [SerializeField] private BehaviorGraph behaviorGraph;
    public float attackCooldown;

    private void Awake()
    {
        InitializeBaseStats();
        rb = GetComponent<Rigidbody>();
        AiAgent = GetComponent<BehaviorGraphAgent>();
        navAgent = GetComponent<NavMeshAgent>();
        if(!_player)
            _player = GameObject.FindGameObjectWithTag("Player");
        
        AiAgent.Graph = behaviorGraph;
        AiAgent.Start();
        AiAgent.SetVariableValue("Target", _player);
        AiAgent.SetVariableValue("AttackDelay", attackCooldown);
        navAgent.speed = _moveSpeed;
    }
    

    private void Update()
    {
        //var rotation = Quaternion.LookRotation(Player.transform.position - transform.position, Vector3.up);
        transform.LookAt(_player.transform,Vector3.up);
        var rot = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, rot.y, 0);
    }

    public void Destroy()
    {
        AiAgent.End();
        navAgent.enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        Destroy(this);
    }


    public override void TakeDamage(int amount, Entity attacker)
    {
        base.TakeDamage(amount, attacker);
        Debug.Log($"Took {amount} damage and now has {_currentHealth} health");
    }
}
}
