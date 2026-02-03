using System;
using Unity.Behavior;
using UnityEngine;

namespace Entities
{
public class TestEnemy : Entity
{
    private static GameObject _player;

    private Rigidbody rb;
    
    private BehaviorGraphAgent agent;

    public float attackCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<BehaviorGraphAgent>();
        
        if(!_player) _player = GameObject.FindGameObjectWithTag("Player");
        
        agent.SetVariableValue("Target", _player);
        agent.SetVariableValue("AttackDelay", attackCooldown);
    }

    private void Update()
    {
        //var rotation = Quaternion.LookRotation(Player.transform.position - transform.position, Vector3.up);
        transform.LookAt(_player.transform,Vector3.up);
        var rot = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, rot.y, 0);
    }
}
}
