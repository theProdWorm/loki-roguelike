using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private enum FollowBehaviour
    {
        Ahead, Behind
    }
    
    [SerializeField] private Rigidbody _target;
    
    [Header("Position")]
    [SerializeField] private bool _lerpPosition = true;
    [SerializeField] private float _positionLerpSpeed = .1f;
    [SerializeField] private float _followOffset = 1f;
    [SerializeField] private FollowBehaviour _followBehaviour = FollowBehaviour.Ahead;
    
    [Header("Zoom")]
    [SerializeField] private bool _lerpZoom = true;
    [SerializeField] private float _zoomInLerpSpeed = .05f;
    [SerializeField] private float _zoomOutLerpSpeed = .2f;
    [SerializeField] private float _minFOV = 70f;
    [SerializeField] private float _maxFOV = 85f;
    
    private Vector3 _offset;
    
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _offset = transform.position - _target.position;
    }

    private void FixedUpdate()
    {
        transform.position = _lerpPosition ? LerpPosition() : _target.position + _offset;
        
        if (_lerpZoom)
            _camera.fieldOfView = LerpZoom();
    }

    private Vector3 LerpPosition()
    {
        Vector3 followPoint = _target.position + _followOffset * _followBehaviour switch
        {
            FollowBehaviour.Ahead => _target.linearVelocity.normalized,
            _ => Vector3.zero
        };

        return Vector3.Lerp(transform.position, followPoint + _offset, _positionLerpSpeed);
    }

    private float LerpZoom()
    {
        bool zoomOut = _target.linearVelocity.sqrMagnitude > .5f;
        
        float targetFOV = zoomOut ? _maxFOV : _minFOV;
        float lerpSpeed = zoomOut ? _zoomOutLerpSpeed : _zoomInLerpSpeed;
        
        return Mathf.Lerp(_camera.fieldOfView, targetFOV, lerpSpeed);
    }
}