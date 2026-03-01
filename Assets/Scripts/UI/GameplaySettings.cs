using System;
using System.Collections.Generic;
using NUnit.Framework;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    public Toggle EnemyHealthBarToggle;

    

    public void EnemyHealthBarsEnabled(bool value)
    {
        UIEnemyHealth.SlidersEnabled(value);
    }
}
