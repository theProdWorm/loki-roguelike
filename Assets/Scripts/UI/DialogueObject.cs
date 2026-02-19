using Unity.AppUI.UI;
using UnityEngine;

public class DialogueObject : MonoBehaviour, IInteractable
{
    public bool Highlighted { get; set; }
    
    public Vector3 Position { get; private set; }
    
    [TextArea]
    public string[] Dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Position = transform.position;
    }

    public void Interacted()
    {
        DialogueManager.StartDialogue(Dialogue);
    }
    
    private void OnDrawGizmos()
    {
        if (Highlighted)
        {
            var pos = transform.position;
            Gizmos.DrawLine(pos, new Vector3(pos.x,5,pos.z)); 
        }
            
    }
}
