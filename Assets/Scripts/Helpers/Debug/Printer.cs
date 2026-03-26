using UnityEngine;

namespace Helpers.Debug
{
    public class Printer : MonoBehaviour
    {
        public void Print(string message) => UnityEngine.Debug.Log(message);
        public void Print(int message)    => UnityEngine.Debug.Log(message);
    }
}
