using UnityEngine;
using System.Collections.Generic;

public class SeeThroughSync : MonoBehaviour
{
    private static GameObject PLAYER;

    private int PosID = Shader.PropertyToID("_Pos");
    private int PlayerPosID = Shader.PropertyToID("_Player_Position");
    private int CameraPosID = Shader.PropertyToID("_Camera_Position");
    private int SizeID = Shader.PropertyToID("_Size");

    private float _treeRadius;
    private float _stoneRadius;
    [SerializeField]
    private Material _treeMaterial;
    [SerializeField]
    private Material _stoneMaterial;
    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private float _fadeSpeed = 1f;

    private Camera _camera;
    void Awake()
    {
        _camera = Camera.main;
        if (!PLAYER)
            PLAYER = GameObject.FindGameObjectWithTag("Player");
        //_treeRadius = _treeMaterial.GetFloat("_Size");
        //_stoneRadius = _stoneMaterial.GetFloat("_Size");
        _treeRadius = 3f;
        _stoneRadius = 3f;
    }

    float size = 0;
    void Update()
    {
        Vector2 view = _camera.WorldToViewportPoint(transform.position);
        Vector3 dir = _camera.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, dir.magnitude, _mask))
        {
            size += Time.deltaTime * _fadeSpeed;
            size = Mathf.Clamp(size, 0, _treeRadius);
            _treeMaterial.SetFloat(SizeID, size);
            _stoneMaterial.SetFloat(SizeID, size);
        }
        else
        {
            size -= Time.deltaTime * _fadeSpeed;
            size = Mathf.Clamp(size, 0, _treeRadius);
            _treeMaterial.SetFloat(SizeID, size);
            _stoneMaterial.SetFloat(SizeID, size);
        }
        _treeMaterial.SetVector(PosID, view);
        _stoneMaterial.SetVector(PosID, view);
    }
}
