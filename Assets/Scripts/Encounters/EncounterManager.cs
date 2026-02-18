using UnityEngine;
using System.Collections.Generic;
using Entities.Player;
using Entities;

public class EncounterManager : MonoBehaviour
{
    private static Player _player;
    public enum EnemyTypes
    {
        Draugr,
        BirdOnBird,
        Wolf
    }

    [SerializeField] List<GameObject> _doors = new();

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
    bool _isSpawning = false;

    private float _timeBetweenChecks = .25f;
    private float _t = 0f;


    public void StartEncounter()
    {
        if (_isEncounterActive || _isEncounterCompleted) return;
        _isEncounterActive = true;
        _player = FindFirstObjectByType<Player>();
        CloseDoors();
        ActivateFirstWave();
    }

    private void Update()
    {
        //if (!_isEncounterActive || _isEncounterCompleted || _isSpawning) return;

        _t += Time.deltaTime;

        if (_t >= _timeBetweenChecks)
        {
            _t = 0;
            CountAliveEnemies();

            if (_timeBetweenWaves != -1)
            {
                _timeSinceLastWave += _t;

                if (_timeSinceLastWave >= _timeBetweenWaves)
                {
                    NextWave();
                    Debug.Log("Time Check");
                }
            }

            float _percentageOfEnemiesLeft = (float)(_currentAmountOfEnemiesAlive / _amountOfEnemiesThisWave);
            Debug.Log("Percentage Check: " + _percentageOfEnemiesLeft + ", " + _currentAmountOfEnemiesAlive + "/" + _amountOfEnemiesThisWave);
            if (_percentageOfEnemiesLeft <= _percentageOfEnemiesToSpawnNextWave && _isSpawning == false)
            {
                NextWave();
            }
            _isSpawning = false;
        }
    }

    private void CountAliveEnemies()
    {
        int i = 0;
        foreach (Entity enemy in _enemiesAlive)
        {
            if (!enemy.IsDead)
                i++;
        }

        _currentAmountOfEnemiesAlive = i;
        Debug.Log("Current Enemies Alive: " + i);
    }

    private void NextWave()
    {
        Debug.Log(_currentWaveIndex + ", " + _enemyWaves.Count);
        if (_currentWaveIndex >= _enemyWaves.Count)
        {
            _isEncounterCompleted = true;
            OpenDoors();
            Debug.Log("Encounter Completed");
            return;
        }
        _isSpawning = true;
        Debug.Log("Spawning Next Wave");

        List<EnemyTypes> nextWaveEnemies = _enemyWaves[_currentWaveIndex].Enemies;

        _amountOfEnemiesThisWave = nextWaveEnemies.Count + (int)_currentAmountOfEnemiesAlive;

        SpawnWave(nextWaveEnemies);
        CountAliveEnemies();
        _currentWaveIndex++;
    }

    private void ActivateFirstWave()
    {
        _amountOfEnemiesThisWave = _wave0.Count;
        foreach (GameObject enemy in _wave0)
        {
            Entity script = enemy.GetComponent<Entity>();
            _enemiesAlive.Add(script);
        }
        CountAliveEnemies();
    }

    private void SpawnWave(List<EnemyTypes> wave)
    {
        foreach (EnemyTypes enemy in wave)
        {
            switch (enemy)
            {
                case EnemyTypes.Draugr:
                    SpawnDraugr();
                    break;
                case EnemyTypes.BirdOnBird:
                    SpawnBirdOnBird();
                    break;
                case EnemyTypes.Wolf:
                    SpawnWolf();
                    break;
            }
        }
    }

    private void CloseDoors()
    {
        //TODO: Close doors when encounter starts
    }

    private void OpenDoors()
    {
        //TODO: Open doors when encounter is completed
    }

    private void SpawnDraugr()
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
                r = Random.Range(0, _draugrSpawnPoints.Count);

                //Check distance availability
                if (_tooCloseSpawnPoints.Contains(_draugrSpawnPoints[r]) || _tooFarSpawnPoints.Contains(_draugrSpawnPoints[r]))
                    continue;

                _draugrSpawnPoints[r].GetComponent<SpawnPoint>().Spawn();
                _draugrSpawnPoints.RemoveAt(r);
                return;
            }

            #endregion

            #region Search Far

            for (int i = 0; i < _draugrSpawnPoints.Count; i++)
            {
                r = Random.Range(0, _draugrSpawnPoints.Count);

                //Check distance availability
                if (_tooCloseSpawnPoints.Contains(_draugrSpawnPoints[r]))
                    continue;

                _draugrSpawnPoints[r].GetComponent<SpawnPoint>().Spawn();
                _draugrSpawnPoints.RemoveAt(r);
                return;
            }

            #endregion

            #region Search everywhere

            r = Random.Range(0, _draugrSpawnPoints.Count);

            _draugrSpawnPoints[r].GetComponent<SpawnPoint>().Spawn();
            _draugrSpawnPoints.RemoveAt(r);
            return;

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



        if (_tooCloseDoorways.Count == _doors.Count)
            _availableDoorways = _doors;

        if (_availableDoorways.Count == 0)
            return;

        r = Random.Range(0, _availableDoorways.Count);
        _enemiesAlive.Add(_availableDoorways[r].GetComponentInChildren<SpawnPoint>().Spawn());

        #endregion
    }

    private void SpawnBirdOnBird()
    {
        //TODO: Fill out the spawning of the bird on bird enemy, they spawn from above trees and swoop in to the arena.
    }

    private void SpawnWolf()
    {
        //TODO: Fill out the spawning of the wolf enemy, do they spawn from the doors and run into the arena?
    }
}
