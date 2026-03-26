using UnityEngine;

namespace _LEGACY__Dungeon_Generation
{
    public class LegacyDoorway : MonoBehaviour
    {
        public Corridor ConnectedCorridor;

        private bool _isConnected = false;
        public bool IsConnected { get { return _isConnected; } }

        public void UpdateConnection(bool state)
        {
            _isConnected = state;
            //TODO:Change visual representation of doorway to indicate connection state
        }
    }
}