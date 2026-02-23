using Entities;
using Entities.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class EncounterManager : MonoBehaviour
{
    private static Player _player;
    public enum EnemyTypes
    {
        Draugr,
        BirdOnBird,
        Wolf
    }

    [SerializeField] List<GameObject> _gates = new();

    [Header("Enemy Spawn Points")]
    [SerializeField, Tooltip("Represented in percentage form"), Range(0, 100)]
    private int _chanceForDraugrToRunThroughDoor = 25;
    [SerializeField, Tooltip("Set to -1 to turn off")]
    private float minimumDistanceFromPlayerToSpawnDraugr = -1f;
    [SerializeField, Tooltip("Set to -1 to turn off")]
    private float maximumDistanceFromPlayerToSpawnDraugr = -1f;
    [SerializeField]
    private List<GameObject> _draugrSpawnPoints = new();

    [SerializeField]
    private List<GameObject> _birdOnBirdSpawnPoints = new();

    [SerializeField]
    private List<GameObject> _wolfSpawnPoints = new(); //TODO: Maybe just have wolves run in through the doors instead of having spawn points for them?

    [Header("Enemy Waves"), SerializeField, Tooltip("These are the enemies that are supposed to already be in the room")]
    private List<GameObject> _wave0 = new();
    [SerializeField, Tooltip("Just add the EnemyWave script as a component below to edit it, then drag and drop that component to add it to the list")]
    private List<EnemyWave> _enemyWaves = new();

    [SerializeField, Tooltip("The minimum time in seconds between each wave of enemies, set to -1 if you want to turn it off")]
    private float _timeBetweenWaves = -1f;
    private float _timeSinceLastWave = 0f;

    [SerializeField, Tooltip("Represented in decimal form"), Range(0, 1)]
    private float _percentageOfEnemiesToSpawnNextWave = 25;

    private List<Entity> _enemiesAlive = new();
    private float _currentAmountOfEnemiesAlive = 0;
    [Tooltip("Adds together the amount of enemies that were left over from last wave and the ones that spawn in the new wave")]
    private int _amountOfEnemiesThisWave = 0;
    private int _currentWaveIndex = 0;

    private bool _isEncounterActive = false;
    private bool _isEncounterCompleted = false;
    private bool _isSpawning = false;

    private float _timeBetweenChecks = .25f;
    private float _t = 0f;

    private void OnDisable()
    {
        foreach (Entity enemy in _enemiesAlive)
        {
            enemy.OnDeath.RemoveListener(EnemyDied);
        }
    }

    public void StartEncounter()
    {
        if (_isEncounterActive || _isEncounterCompleted) return;
        _isEncounterActive = true;
        _player = FindFirstObjectByType<Player>();
        CloseDoors();

        if (_wave0.Count > 0)
            ActivateFirstWave();
        else
            NextWave();
    }

    private void Update()
    {
        if (!_isEncounterActive || _isEncounterCompleted || _isSpawning) return;

        _t += Time.deltaTime;

        if (_t >= _timeBetweenChecks)
        {
            _t = 0;

            if (_timeBetweenWaves != -1)
            {
                _timeSinceLastWave += _t;

                if (_timeSinceLastWave >= _timeBetweenWaves)
                {
                    NextWave();
                }
            }

            float _percentageOfEnemiesLeft = (float)(_currentAmountOfEnemiesAlive / _amountOfEnemiesThisWave);
            if (_percentageOfEnemiesLeft <= _percentageOfEnemiesToSpawnNextWave && _isSpawning == false)
            {
                NextWave();
            }
        }
    }

    private void EnemyDied(Entity enemy)
    {
        //TODO: Add a father enemy class and put that here instead of the test enemy
        if (enemy is TestEnemy)
        {
            _enemiesAlive.Remove(enemy);
        }
        _currentAmountOfEnemiesAlive = _enemiesAlive.Count;
    }

    private void NextWave()
    {
        if (_currentWaveIndex >= _enemyWaves.Count)
        {
            if (_enemiesAlive.Count <= 0)
            {
                _isEncounterCompleted = true;
                OpenDoors();
            }
            return;
        }
        _isSpawning = true;

        List<EnemyTypes> nextWaveEnemies = _enemyWaves[_currentWaveIndex].Enemies;

        _amountOfEnemiesThisWave = nextWaveEnemies.Count + (int)_currentAmountOfEnemiesAlive;

        SpawnWave(nextWaveEnemies);
        _currentAmountOfEnemiesAlive = _enemiesAlive.Count;

        _currentWaveIndex++;
    }

    private void ActivateFirstWave()
    {
        _amountOfEnemiesThisWave = _wave0.Count;
        foreach (GameObject enemy in _wave0)
        {
            Entity script = enemy.GetComponent<Entity>();
            script.OnDeath.AddListener(EnemyDied);
            _enemiesAlive.Add(script);
        }
        _currentAmountOfEnemiesAlive = _enemiesAlive.Count;
    }

    private void SpawnWave(List<EnemyTypes> wave)
    {
        for (int i = 0; i < wave.Count; i++)
        {
            Entity entity = null;
            switch (wave[i])
            {
                case EnemyTypes.Draugr:
                    entity = SpawnDraugr();
                    break;
                case EnemyTypes.BirdOnBird:
                    entity = SpawnBirdOnBird();
                    break;
                case EnemyTypes.Wolf:
                    entity = SpawnWolf();
                    break;
            }

            if (entity != null)
            {
                entity.OnDeath.AddListener(EnemyDied);
                _enemiesAlive.Add(entity);
            }

            else
                i--;
        }
        _isSpawning = false;
    }

    private void CloseDoors()
    {
        foreach (GameObject gate in _gates)
        {
            //TODO: Maybe put in a encounter start sound effect or something?
            gate.GetComponent<Gateway>().Close();
        }
    }

    private void OpenDoors()
    {
        foreach (GameObject gate in _gates)
        {
            //TODO: Maybe put in a encounter completed sound effect or something here before opening the doors?
            gate.GetComponent<Gateway>().Open();
        }
    }

    private Entity SpawnDraugr()
    {
        int r = Random.Range(1, 101);
        if (r > _chanceForDraugrToRunThroughDoor) //If Draugr chooses to rather not run through door then run this code. Otherwise go directly to the Force Door region
        {
            #region Distance Checks
            List<GameObject> _tooCloseSpawnPoints = new();
            List<GameObject> _tooFarSpawnPoints = new();

            foreach (GameObject statue in _draugrSpawnPoints)
            {
                float distance = Vector2.Distance(statue.transform.position, _player.transform.position);
                if (distance < minimumDistanceFromPlayerToSpawnDraugr && minimumDistanceFromPlayerToSpawnDraugr != -1)
                    _tooCloseSpawnPoints.Add(statue);
                if (distance > maximumDistanceFromPlayerToSpawnDraugr && maximumDistanceFromPlayerToSpawnDraugr != -1)
                    _tooFarSpawnPoints.Add(statue);
            }
            #endregion

            #region Ideal Spawn Distance

            for (int i = 0; i < _draugrSpawnPoints.Count; i++)
            {
                r = Random.Range(0, _draugrSpawnPoints.Count - 1);

                //Check distance availability
                if (_tooCloseSpawnPoints.Contains(_draugrSpawnPoints[r]) || _tooFarSpawnPoints.Contains(_draugrSpawnPoints[r]))
                    continue;

                SpawnPoint spawnPointScript = _draugrSpawnPoints[r].GetComponent<SpawnPoint>();
                _draugrSpawnPoints.RemoveAt(r);
                return spawnPointScript.Spawn();
            }

            #endregion

            #region Search Far

            for (int i = 0; i < _draugrSpawnPoints.Count; i++)
            {
                r = Random.Range(0, _draugrSpawnPoints.Count - 1);

                //Check distance availability
                if (_tooCloseSpawnPoints.Contains(_draugrSpawnPoints[r]))
                    continue;

                SpawnPoint spawnPointScript = _draugrSpawnPoints[r].GetComponent<SpawnPoint>();
                _draugrSpawnPoints.RemoveAt(r);
                return spawnPointScript.Spawn();
            }

            #endregion

            #region Search everywhere

            r = Random.Range(0, _draugrSpawnPoints.Count - 1);

            _draugrSpawnPoints.RemoveAt(r);
            return _draugrSpawnPoints[r].GetComponent<SpawnPoint>().Spawn(); ;

            #endregion
        }

        #region Force Door

        //Distance Check
        List<GameObject> _availableDoorways = new();
        List<GameObject> _tooCloseDoorways = new();

        foreach (GameObject door in _draugrSpawnPoints)
        {
            float distance = Vector2.Distance(door.transform.position, _player.transform.position);
            if (distance < minimumDistanceFromPlayerToSpawnDraugr && minimumDistanceFromPlayerToSpawnDraugr != -1)
                _tooCloseDoorways.Add(door);
            else
                _availableDoorways.Add(door);
        }



        if (_tooCloseDoorways.Count == _gates.Count)
            _availableDoorways = _gates;

        if (_availableDoorways.Count == 0)
            return null;

        r = Random.Range(0, _availableDoorways.Count);

        Entity draugr = _availableDoorways[r].GetComponentInChildren<SpawnPoint>().Spawn();
        return draugr;

        #endregion
    }

    private Entity SpawnBirdOnBird()
    {
        //TODO: Fill out the spawning of the bird on bird enemy, they spawn from above trees and swoop in to the arena.
        return null;
    }

    private Entity SpawnWolf()
    {
        //TODO: Fill out the spawning of the wolf enemy, do they spawn from the doors and run into the arena?
        return null;
    }
}
