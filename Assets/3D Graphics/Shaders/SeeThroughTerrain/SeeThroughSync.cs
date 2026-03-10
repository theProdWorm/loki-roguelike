using UnityEngine;
using System.Collections.Generic;

public class SeeThroughSync : MonoBehaviour
{
    private static GameObject PLAYER;

    private int PosID = Shader.PropertyToID("_Pos");
    private int PlayerPosID = Shader.PropertyToID("_Player_Position");
    private int CameraPosID = Shader.PropertyToID("_Camera_Position");
    private int SizeID = Shader.PropertyToID("_Size");

    private float _radius;
    [SerializeField]
    private Material _material;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float _fadeSpeed = 1f;

    private Camera _camera;
    void Awake()
    {
        _camera = Camera.main;
        if (!PLAYER)
            PLAYER = GameObject.FindGameObjectWithTag("Player");
        _radius = _material.GetFloat("_Size");
    }

    float size = 0;
    void Update()
    {
        Vector3 dir = _camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, dir.magnitude, Mask))
        {
            size += Time.deltaTime * _fadeSpeed;
            size = Mathf.Clamp(size, 0, _radius);
            _material.SetFloat(SizeID, size);
        }
        else
        {
            size -= Time.deltaTime * _fadeSpeed;
            size = Mathf.Clamp(size, 0, _radius);
            _material.SetFloat(SizeID, size);
        }

        Vector2 view = _camera.WorldToViewportPoint(transform.position);
        _material.SetVector(PosID, view);
        _material.SetVector(PlayerPosID, transform.position);
        _material.SetVector(CameraPosID, _camera.transform.position);

        //foreach (var mat in SeeThroughMaterials)
        //{
        //mat.SetVector(PosID, view);
        //mat.SetVector(PlayerPosID, transform.position);
        //mat.SetVector(CameraPosID, _camera.transform.position);
        //}
    }
}
