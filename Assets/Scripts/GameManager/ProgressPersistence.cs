using System;
using UnityEngine;

public class ProgressPersistence : MonoBehaviour
{
    public static bool FirstBranchDone;
    public static bool SecondBranchDone;

    public bool BeatBoss = false;
    public bool BeatBoss2 = false;

    public int SpawnPersistence = 0;
    public int TutorialProgression = 0;
    public int LeftBranchProgression = 0;
    public int RightBranchProgression = 0;

    //This will always know how long you've progressed in your current branch
    public int CurrentBranchProgression = 0;

    public void UnlockDialogue1() => FirstBranchDone = true;
    public void UnlockDialogue2() => SecondBranchDone = true;
    
    [Tooltip("0 = Tutorial, 1 = Left Branch, 2 = Right Branch")]
    public void ChangeCurrentBranch(int branch)
    {
        switch (branch)
        {
            default:
            case 0:
                CurrentBranchProgression = TutorialProgression;
                break;
            case 1:
                CurrentBranchProgression = LeftBranchProgression;
                break;
            case 2:
                CurrentBranchProgression = RightBranchProgression;
                break;
        }
    }
}
