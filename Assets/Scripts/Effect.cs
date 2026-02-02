using UnityEngine;

public class Effect : ScriptableObject
{
    [Range(-1, 3)]
    public float DamageMultiplier;

    [Range(-1, 3)]
    public float MoveSpeedMultiplier;
        
    [Range(0, 3)]
    public float RangeMultiplier;
        
    [Range(0, 1)]
    public float CritChance;
    [Range(0, 3)]
    public float CritDamage;

    [Range(-1, 1)]
    public float DamageReduction;
}