using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    #region Dungeon Generation Variables
    public int Weight;
    public List<Doorway> Doorways = new();

    [/*HideInInspector,*/ Tooltip("Only used for connecting all doors before pruning the connections with Prim's algorithm, creating a minimum spanning tree")]
    public List<Corridor> ConnectedCorridors;

    public int DistanceFromStart = -1;
    #endregion

    public bool IsVisited = false;

    private void Awake()
    {
       // gameObject.SetActive(false); //Rooms are initially inactive until visited
    }
}
