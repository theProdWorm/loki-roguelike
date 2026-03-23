using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    static Player PLAYER;

    [SerializeField]
    private List<Transform> _reSpawnPoints = new();
    [SerializeField]
    private List<Transform> _doorwayPoints = new();

    private ProgressPersistence _progressPersistence;

    private bool _justDied = false;

    void Start()
    {
        _progressPersistence = FindFirstObjectByType<ProgressPersistence>();

        if (_justDied)
        {
            _justDied = false;
            int i = _progressPersistence.CurrentBranchProgression;
            PLAYER.transform.position = _reSpawnPoints[i].position;
            PLAYER.transform.rotation = _reSpawnPoints[i].rotation;
        }
        else
            StartCoroutine(WalkThroughDoorRoutine());

    }

    private void OnEnable()
    {
        PLAYER = FindFirstObjectByType<Player>();
        PLAYER.OnDeath.AddListener(JustDied);
    }

    private void OnDisable()
    {
        PLAYER.OnDeath.RemoveListener(JustDied);
    }

    private void JustDied(Entity entity)
    {
        if (entity == PLAYER)
            _justDied = true;
    }

    private IEnumerator WalkThroughDoorRoutine()
    {
        int i = _progressPersistence.CurrentBranchProgression;
        PLAYER.transform.position = _doorwayPoints[i].position;
        PLAYER.transform.rotation = _doorwayPoints[i].rotation;

        PLAYER.SetDashing(true);
        yield return new WaitForSecondsRealtime(1);
        PLAYER.SetDashing(false);
    }

    [Tooltip("0 = Tutorial, 1 = Left Branch, 2 = Right Branch")]
    public void ChangeCurrentBranch(int branch)
    {
        if (_progressPersistence == null)
            _progressPersistence = FindFirstObjectByType<ProgressPersistence>();

        switch (branch)
        {
            default:
            case 0:
                _progressPersistence.CurrentBranchProgression = _progressPersistence.TutorialProgression;
                break;
            case 1:
                _progressPersistence.CurrentBranchProgression = _progressPersistence.LeftBranchProgression;
                break;
            case 2:
                _progressPersistence.CurrentBranchProgression = _progressPersistence.RightBranchProgression;
                break;
        }
    }

    public void ChangeBranchProgression(int i)
    {
        if (_progressPersistence == null)
            _progressPersistence = FindFirstObjectByType<ProgressPersistence>();

        _progressPersistence.CurrentBranchProgression = i;

        switch (_progressPersistence.CurrentBranch)
        {
            default:
            case 0:
                _progressPersistence.TutorialProgression = i;
                break;
            case 1:
                _progressPersistence.LeftBranchProgression = i;
                break;
            case 2:
                _progressPersistence.RightBranchProgression = i;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var item in _reSpawnPoints)
        {
            Vector3 pos = item.position;

            Gizmos.DrawWireSphere(pos, .5f);

            Gizmos.DrawRay(item.position, item.transform.forward * 2);
        }

        Gizmos.color = Color.blue;
        foreach (var item in _doorwayPoints)
        {
            Vector3 pos = item.position;

            Gizmos.DrawWireSphere(pos, .5f);

            Gizmos.DrawRay(item.position, item.transform.forward * 2);
        }
    }
}
