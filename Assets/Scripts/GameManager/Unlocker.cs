using UnityEngine;

namespace GameManager
{
    public class Unlocker : MonoBehaviour
    {
        public static void UnlockDialogue1() => ProgressPersistence.UnlockDialogue1();
        public static void UnlockDialogue2() => ProgressPersistence.UnlockDialogue2();
    }
}