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
