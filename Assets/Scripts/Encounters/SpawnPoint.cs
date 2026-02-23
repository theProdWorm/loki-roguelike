using UnityEngine;
using Entities;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField, Tooltip("The sort of enemy that spawns here")]
    private EncounterManager.EnemyTypes _enemyType;

    [SerializeField]
    private GameObject _draugrPrefab;
    [SerializeField]
    private GameObject _bbPrefab;
    [SerializeField]
    private GameObject _wolfPrefab;

    public Entity Spawn()
    {
        switch (_enemyType)
        {
            case EncounterManager.EnemyTypes.Draugr:
                return SpawnDraugr();
            case EncounterManager.EnemyTypes.BirdOnBird:
                return SpawnBirdOnBird();
            case EncounterManager.EnemyTypes.Wolf:
                return SpawnWolf();
            default:
                break;
        }

        return null;
    }

    bool _hasSpawned = false; // Temp variable
    private Entity SpawnDraugr()
    {
        if (_hasSpawned) return null;
        _hasSpawned = true;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        //TODO: OBJECTPOOL
        GameObject _draugr = Instantiate(_draugrPrefab, transform.position, Quaternion.identity);

        Entity _entity = _draugr.GetComponent<Entity>();

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