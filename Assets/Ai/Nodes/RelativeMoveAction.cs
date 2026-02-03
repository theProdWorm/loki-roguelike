using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Relative Move", story: "[Agent] moves relative to [Target]", category: "Action", id: "e6034d269de7d693e4e6c36aee4e104f")]
public partial class RelativeMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> StepDistance = new BlackboardVariable<float>(2.0f);
    [SerializeReference] public BlackboardVariable<float> DirectionAngle = new BlackboardVariable<float>(0f);
    private NavMeshAgent m_NavMeshAgent;
    
    protected override Status OnStart()
    {
        m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
        if (!m_NavMeshAgent) return Status.Failure;
        return Status.Running;
    }

    private bool wait;
    private int waiter;
    protected override Status OnUpdate()
    {
        
        if (!wait)
        {
            var direction = Target.Value.transform.position - Agent.Value.transform.position;
            direction = Quaternion.Euler(0,DirectionAngle,0) * direction;
            m_NavMeshAgent.SetDestination(Agent.Value.transform.position + (direction.normalized*StepDistance.Value));
            wait = true;
        }
        if (wait && m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        {
            wait = false;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        m_NavMeshAgent = null;
    }
}

