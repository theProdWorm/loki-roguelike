using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] int AmountOfRooms = 12;
    int roomsGenerated = 0;

    [Header("Room Pools")]
    [SerializeField] List<Room> normalRooms = new();
    
    [SerializeField] List<Room> startRooms = new();
    
    [SerializeField] List<Room> bossRooms = new();
    
    [SerializeField] int minimumSecretRooms = 0;
    [SerializeField] List<Room> secretRooms = new();
    
    [SerializeField] int minimumReviveRooms = 0;
    [SerializeField] List<Room> reviveRooms = new();


    private Room startRoom;
    private Room bossRoom;


    void Start()
    {
      //  GenerateFloor();
    }

    void Update()
    {
        
    }

    private void GenerateFloor()
    {
        GenerateBossRoom();
        GenerateStartRoom();

        if(minimumReviveRooms > 0)
            GenerateReviveRooms();

        if(minimumSecretRooms > 0)
            GenerateSecrets();

        GenerateGeneralRooms();
    }

    private void GenerateGeneralRooms()
    {
        int roomsToGenerate = AmountOfRooms - roomsGenerated;
        List<Room> selectedRooms = RandomizeRooms(normalRooms, roomsToGenerate);
    }

    private void GenerateBossRoom()
    {
    
    }

    private void GenerateStartRoom()
    {

    }

    private void GenerateReviveRooms()
    {

    }

    private void GenerateSecrets()
    {

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
}
