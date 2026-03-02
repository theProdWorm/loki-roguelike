using UnityEngine;
using Entities;
using Helpers;
using UnityEngine.AI;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField, Tooltip("The sort of enemy that spawns here")]
    private EncounterManager.EnemyTypes _enemyType;

    [Header("Draugr")]
    [SerializeField]
    private bool _sittingStatue = false;
    public bool hasSpawned = false;

    [SerializeField, Tooltip("DEV TOOL. Designers don't touch")]
    private Component[] _componentsToTurnOffAndOn; //TODO: Fix this ugly ass solution

    Animator animator;

    private float _animatorSpeed; //Holder for the animation speed of current clip

    [Header("UGLY SOLUTION, WILL BE FIXED")]
    [SerializeField]
    private Enemy _enemyScript;
    [SerializeField]
    private Behaviour _behaviourAgentComponent;
    [SerializeField]
    private NavMeshAgent _navMeshAgentComponent;
    [SerializeField]
    private AnimationEventListener _animationEventListenerScript;

    [Header("Bird on Bird")]
    [SerializeField]
    private GameObject _bbPrefab;

    [Header("Wolf")]
    [SerializeField]
    private GameObject _wolfPrefab;

    private void Start()
    {
        if (!hasSpawned)
            PrepareDraugrStatue();
    }

    public Entity Spawn()
    {
        switch (_enemyType)
        {
            //case EncounterManager.EnemyTypes.Draugr:
            //    return SpawnDraugr();
            case EncounterManager.EnemyTypes.BirdOnBird:
                return SpawnBirdOnBird();
            case EncounterManager.EnemyTypes.Wolf:
                return SpawnWolf();
            default:
                break;
        }

        return null;
    }

    private void PrepareDraugrStatue()
    {
        animator = GetComponent<Animator>();

        animator.SetBool("HasSpawned", false);
        animator.SetBool("Sitting", _sittingStatue);

        #region Ugly solution
        _enemyScript.enabled = false;
        _behaviourAgentComponent.enabled = false;
        _navMeshAgentComponent.enabled = false;
        _animationEventListenerScript.enabled = false;
        #endregion

        _animatorSpeed = animator.speed;
        animator.speed = 0f;

        //TODO: Pick a random frame in the animation
    }

    private Entity SpawnDraugr()
    {
        //Safety check
        if (hasSpawned) return null;
        hasSpawned = true;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Entity _entity = GetComponent<Entity>();

        #region Ugly solution
        _enemyScript.enabled = true;
        _behaviourAgentComponent.enabled = true;
        _navMeshAgentComponent.enabled = true;
        _animationEventListenerScript.enabled = true;
        #endregion

        return _entity;
    }

    private Entity SpawnBirdOnBird()
    {
        return null;
        //TODO: OBJECTPOOL
    }

    private Entity SpawnWolf()
    {
        return null;
        //TODO: OBJECTPOOL
    }
}