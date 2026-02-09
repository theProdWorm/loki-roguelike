using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField] private int _amountOfRooms = 12;
    [SerializeField, Range(0, 1), Tooltip("Likeliness of an extra connection to be made from a otherwise empty door in decimal form")]
    private float _likelinessOfExtraConnections = 0.2f;

    [SerializeField] private int _minDistanceFromStartToBoss = 5;

    [SerializeField] private GameObject _corridorPrefab;
    [SerializeField, Tooltip("Where all the rooms will be grouped in the hierarchy")] 
    private Transform _roomsParent;
    [SerializeField, Tooltip("Where all the corridors will be grouped in the hierarchy")] 
    private Transform _corridorsParent;

    [Header("Room Pools")]
    [SerializeField] private List<Room> _normalRooms = new();

    [SerializeField] private List<Room> _startRooms = new();

    [SerializeField] private List<Room> _bossRooms = new();

    [SerializeField] private int _minimumSecretRooms = 0;
    [SerializeField] private List<Room> _secretRooms = new();

    [SerializeField] private int _minimumReviveRooms = 0;
    [SerializeField] private List<Room> _reviveRooms = new();

    [Header("Displacement Settings")]
    [SerializeField, Range(5, 50)] private float _minimumDistanceBetweenRooms;
    [SerializeField, Range(5, 50)] private float _maximumDistanceBetweenRooms;

    private Room _startRoom;
    private Room _bossRoom;

    private List<Room> _generalRoomsOnFloor = new();

    private int _roomsGenerated = 0;

    private void Start()
    {
        GenerateFloor();
    }

    private void GenerateFloor()
    {
        GenerateRooms();
        ConnectAllRooms();
        PruneConnections();
        ConnectStartRoom();
        AddExtraConnections();
        CountDistanceFromStart();
        ConnectBossRoom();
        SpaceOutRooms();
    }

    private void GenerateRooms()
    {
        List<Room> selectedRooms = new();
        List<Room> instantiatedRooms = new();

        //Generate boss room
        selectedRooms = RandomizeRooms(_bossRooms, 1);
        instantiatedRooms = InstantiateRooms(selectedRooms);
        _bossRoom = instantiatedRooms[0];
        _roomsGenerated++;

        //Generate start room
        selectedRooms = RandomizeRooms(_startRooms, 1);
        instantiatedRooms = InstantiateRooms(selectedRooms);
        _startRoom = instantiatedRooms[0];
        _roomsGenerated++;

        if (_minimumReviveRooms > 0)
        {
            //Generate revive rooms
            selectedRooms = RandomizeRooms(_reviveRooms, _minimumReviveRooms);
            instantiatedRooms = InstantiateRooms(selectedRooms);
            _generalRoomsOnFloor.AddRange(instantiatedRooms);
            _roomsGenerated += selectedRooms.Count;
        }

        if (_minimumSecretRooms > 0)
        {
            //Generate secret rooms
            selectedRooms = RandomizeRooms(_secretRooms, _minimumSecretRooms);
            instantiatedRooms = InstantiateRooms(selectedRooms);
            _generalRoomsOnFloor.AddRange(instantiatedRooms);
            _roomsGenerated += selectedRooms.Count;
        }

        //Generate general rooms
        int roomsToGenerate = _amountOfRooms - _roomsGenerated;
        selectedRooms = RandomizeRooms(_normalRooms, roomsToGenerate);
        instantiatedRooms = InstantiateRooms(selectedRooms);
        _generalRoomsOnFloor.AddRange(instantiatedRooms);
        _roomsGenerated += selectedRooms.Count;
    }

    private void ConnectAllRooms()
    {
        for (int i = 0; i < _generalRoomsOnFloor.Count; i++)
        {
            foreach (Doorway doorway in _generalRoomsOnFloor[i].Doorways)
            {
                for (int j = i + 1; j < _generalRoomsOnFloor.Count; j++)
                {
                    foreach (Doorway targetDoorway in _generalRoomsOnFloor[j].Doorways)
                    {
                        Corridor corridor = ConnectDoors(doorway, targetDoorway);
                        _generalRoomsOnFloor[i].ConnectedCorridors.Add(corridor);

                        corridor = ConnectDoors(targetDoorway, doorway);
                        _generalRoomsOnFloor[j].ConnectedCorridors.Add(corridor);
                    }
                }
            }
        }
    }

    private void PruneConnections()
    {
        _generalRoomsOnFloor.ForEach(room =>
        {
            room.ConnectedCorridors.Sort((a, b) => a.weight.CompareTo(b.weight));
            for (int i = 1; i < room.ConnectedCorridors.Count; i++)
            {
                Destroy(room.ConnectedCorridors[i].gameObject);

            }
            if (room.ConnectedCorridors.Count <= 0)
                return;

            room.ConnectedCorridors.RemoveRange(1, room.ConnectedCorridors.Count - 1);

            Corridor corridor = room.ConnectedCorridors[0];
            corridor.DoorwayA.UpdateConnection(true);
            corridor.DoorwayA.ConnectedCorridor = corridor;

            corridor.DoorwayB.UpdateConnection(true);
            corridor.DoorwayB.ConnectedCorridor = corridor;
        });
    }

    private void ConnectStartRoom()
    {
        List<Doorway> availableDoorways = new();

        foreach (Room room in _generalRoomsOnFloor)
        {
            foreach (Doorway doorway in room.Doorways)
            {
                if (!doorway.IsConnected)
                {
                    availableDoorways.Add(doorway);
                }
            }
        }

        int i = Random.Range(0, availableDoorways.Count);

        _startRoom.ConnectedCorridors.Add(ConnectDoors(_startRoom.Doorways[0], availableDoorways[i]));
    }

    private void AddExtraConnections()
    {
        if (_likelinessOfExtraConnections <= 0) return;
        foreach (Room room in _generalRoomsOnFloor)
        {
            foreach (Doorway doorway in room.Doorways)
            {
                if (!doorway.IsConnected)
                {
                    if (Random.value <= _likelinessOfExtraConnections)
                    {
                        List<Doorway> availableDoorways = new();
                        foreach (Room targetRoom in _generalRoomsOnFloor)
                        {
                            if (targetRoom == room) continue;
                            foreach (Doorway targetDoorway in targetRoom.Doorways)
                            {
                                if (!targetDoorway.IsConnected)
                                {
                                    availableDoorways.Add(targetDoorway);
                                }
                            }
                        }
                        if (availableDoorways.Count > 0)
                        {
                            int i = Random.Range(0, availableDoorways.Count);
                            room.ConnectedCorridors.Add(ConnectDoors(doorway, availableDoorways[i]));
                        }
                    }
                }
            }
        }
    }

    private void CountDistanceFromStart()
    {
        List<Room> openList = new();
        openList.Add(_startRoom);
        openList.AddRange(_generalRoomsOnFloor);
        List<Room> closedList = new();

        foreach (Room room in openList)
        {
            if (room == _startRoom)
            {
                room.DistanceFromStart = -1;
                Debug.Log(room.ConnectedCorridors.Count);
            }

            foreach (Doorway door in room.Doorways)
            {
                if (door.IsConnected)
                {
                    Corridor corridor = door.ConnectedCorridor;
                    Room targetRoom = (door == corridor.DoorwayA ? corridor.DoorwayB : corridor.DoorwayA).GetComponentInParent<Room>();
                    Debug.Log(targetRoom);

                    if (!closedList.Contains(targetRoom))
                    {
                        int tentativeDistance = room.DistanceFromStart + 1;
                        if (tentativeDistance < targetRoom.DistanceFromStart || targetRoom.DistanceFromStart == 0)
                        {
                            targetRoom.DistanceFromStart = tentativeDistance;
                        }
                    }
                }
            }
            closedList.Add(room);
        }
    }

    private void ConnectBossRoom()
    {
        //There's already an unoccupied doorway far enough from the start room, so we can just connect the boss room to it
        #region Try Connect
        List<Doorway> availableDoorways = new();

        foreach (Room room in _generalRoomsOnFloor)
        {
            foreach (Doorway doorway in room.Doorways)
            {
                if (!doorway.IsConnected && room.DistanceFromStart >= _minDistanceFromStartToBoss)
                {
                    availableDoorways.Add(doorway);
                }
            }
        }

        if (availableDoorways.Count > 0)
        {
            int i = Random.Range(0, availableDoorways.Count);
            ConnectDoors(_bossRoom.Doorways[0], availableDoorways[i]);
            _bossRoom.DistanceFromStart = availableDoorways[i].transform.parent.GetComponent<Room>().DistanceFromStart + 1;
            return; //Doorway found and boss room connected, exit the method
        }
        #endregion

        //There are no free doorways far enough from the start room, so we find a random door far enough away and hijack it
        #region Try Hijack
        List<Doorway> occupiedDoorways = new();
        foreach (Room room in _generalRoomsOnFloor)
        {
            foreach (Doorway doorway in room.Doorways)
            {
                if (room.DistanceFromStart >= _minDistanceFromStartToBoss)
                {
                    occupiedDoorways.Add(doorway);
                }
            }
        }
        Doorway targetDoorway;
        Corridor oldCorridor;
        Room targetRoom;

        bool continueLoop = true;
        List<Doorway> triedDoorways = new();

        while (continueLoop)
        {
            int j = Random.Range(0, occupiedDoorways.Count - 1);
            targetDoorway = occupiedDoorways[j];
            oldCorridor = targetDoorway.ConnectedCorridor;
            Room otherRoom = oldCorridor.DoorwayB.transform.parent.GetComponent<Room>();

            if (otherRoom.Doorways.Count <= 1) //Don't hijack a doorway if it would isolate a room
            {
                if (!triedDoorways.Contains(targetDoorway))
                {
                    triedDoorways.Add(targetDoorway);
                    if (triedDoorways.Count >= occupiedDoorways.Count)
                    {
                        continueLoop = false; //All doorways have been tried, exit the loop and move on to force hijack
                    }
                }
                continue;
            }

            //Disconnect the old corridor
            targetDoorway.UpdateConnection(false);
            targetRoom = oldCorridor.DoorwayA.transform.parent.GetComponent<Room>();
            targetRoom.ConnectedCorridors.Remove(oldCorridor);
            Destroy(oldCorridor.gameObject);
            //Connect the boss room to the target doorway
            ConnectDoors(_bossRoom.Doorways[0], targetDoorway);
            _bossRoom.DistanceFromStart = targetDoorway.transform.parent.GetComponent<Room>().DistanceFromStart + 1;
            return; //Doorway already hijacked, exit the method
        }
        #endregion

        //There are no free doorways and no occupied doorways that can be hijacked, so we just connect the boss room to a random doorway and hope for the best
        #region Force Hijack
        int k = Random.Range(0, occupiedDoorways.Count - 1);
        targetDoorway = occupiedDoorways[k];
        oldCorridor = targetDoorway.ConnectedCorridor;

        //Disconnect the old corridor
        targetDoorway.UpdateConnection(false);
        targetRoom = oldCorridor.DoorwayA.transform.parent.GetComponent<Room>();
        targetRoom.ConnectedCorridors.Remove(oldCorridor);
        Destroy(oldCorridor.gameObject);
        //Connect the boss room to the target doorway
        ConnectDoors(_bossRoom.Doorways[0], targetDoorway);
        _bossRoom.DistanceFromStart = targetDoorway.transform.parent.GetComponent<Room>().DistanceFromStart + 1;
        #endregion
    }

    private void SpaceOutRooms()
    {
        List<Room> openlist = _generalRoomsOnFloor;
        List<Room> closedList = new();
        openlist.Sort((a, b) => a.DistanceFromStart.CompareTo(b.DistanceFromStart)); //Sorts from closest to farthest from the start room

        foreach (Room room in openlist)
        {
            foreach (Doorway doorway in room.Doorways)
            {
                if (doorway.IsConnected)
                {
                    Corridor corridor = doorway.ConnectedCorridor;
                    Room targetRoom = corridor.DoorwayB.transform.parent.GetComponent<Room>();
                    if (!closedList.Contains(targetRoom))
                    {
                        Vector3 direction = (targetRoom.transform.position - room.transform.position).normalized;
                        float distance = Random.Range(_minimumDistanceBetweenRooms, _maximumDistanceBetweenRooms);
                        targetRoom.transform.position = room.transform.position + direction * distance;
                    }
                }
            }
            closedList.Add(room);
        }
    }

    private List<Room> RandomizeRooms(List<Room> rooms, int amount)
    {
        List<Room> weightedList = GetWeightedList(rooms);
        List<Room> selectedRooms = new();

        for (int i = 0; i < amount; i++)
        {
            int j = Random.Range(0, weightedList.Count);
            selectedRooms.Add(weightedList[j]);
        }

        return selectedRooms;
    }

    private List<Room> InstantiateRooms(List<Room> rooms)
    {
        List<Room> instantiatedRooms = new();
        foreach (Room room in rooms)
        {
            Room instantiatedRoom = Instantiate(room, transform.position, Quaternion.identity, _roomsParent);
            instantiatedRooms.Add(instantiatedRoom);
        }
        return instantiatedRooms;
    }

    private List<Room> GetWeightedList(List<Room> rooms)
    {
        List<Room> weightedList = new();
        foreach (Room room in rooms)
        {
            for (int i = 0; i < room.Weight; i++)
            {
                weightedList.Add(room);
            }
        }
        return weightedList;
    }

    private Corridor ConnectDoors(Doorway doorA, Doorway doorB)
    {
        Vector3 distance = doorA.transform.position - doorB.transform.position;
        GameObject corridorObject = Instantiate(_corridorPrefab, Vector3.Lerp(doorA.transform.position, doorB.transform.position, 0.5f), Quaternion.Euler(distance), _corridorsParent);
        corridorObject.transform.localScale = new Vector3(corridorObject.transform.localScale.x, corridorObject.transform.localScale.y, distance.magnitude);

        Corridor corridor = corridorObject.GetComponent<Corridor>();
        corridor.weight = Random.Range(0, 50);

        corridor.DoorwayA = doorA;
        corridor.DoorwayB = doorB;

        return corridor;
    }
}
