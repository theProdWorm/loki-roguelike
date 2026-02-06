using UnityEngine;

public interface IInteractable
{
    public void Interacted();
    
    public bool Highlighted { get; set; }
    
    public Vector3 Position { get; }
}
