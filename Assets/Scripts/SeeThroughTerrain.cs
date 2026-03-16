using Entities;
using System.Linq;
using UnityEngine;

public class SeeThroughTerrain : MonoBehaviour
{
    static GameObject PLAYER;

    [SerializeField]
    private float radius = 3f;
    [SerializeField]
    private LayerMask _seeThroughMask;
    [SerializeField]
    private LayerMask _invisibleMask;

    void Start()
    {
        if (!PLAYER)
            PLAYER = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 dir = transform.position - PLAYER.transform.position;

        RaycastHit[] seeThroughCaster = Physics.SphereCastAll(PLAYER.transform.position, radius, dir.normalized, dir.magnitude, _seeThroughMask);
        RaycastHit[] invisibleCaster = Physics.SphereCastAll(PLAYER.transform.position, radius * 2, dir.normalized, dir.magnitude, _invisibleMask);

        RaycastHit[] allHits = seeThroughCaster.Concat(invisibleCaster).ToArray();

        if (allHits.Length > 0)
        {
            foreach (var hit in allHits)
            {
                Debug.Log("HIDE");
                if (hit.collider.gameObject.layer == _seeThroughMask && seeThroughCaster.Contains(hit))
                {
                    hit.collider.gameObject.layer = _invisibleMask;
                }
                else if (hit.collider.gameObject.layer == _invisibleMask && !seeThroughCaster.Contains(hit))
                {
                    hit.collider.gameObject.layer = _seeThroughMask;
                }
            }
        }
    }
}
