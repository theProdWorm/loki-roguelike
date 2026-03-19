using UnityEngine;

public class SpringBones : MonoBehaviour
{

    public float Spring = 10f;
    public float Damper = 0.2f;
    public float Mass = 10f;
    public float Size = 0.002f;
    public float AngularDamper = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            var a = child.gameObject.AddComponent<Rigidbody>();
            a.mass = Mass;
            a.angularDamping = AngularDamper;
            var b = child.gameObject.AddComponent<SpringBones>();
       
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}