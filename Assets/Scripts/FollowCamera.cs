using System.Collections;
using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 20f;
    [Tooltip("Minimum rotation value on the x-axis (pitch).")]
    [SerializeField] private float _minPitch = 30f;
    [Tooltip("Maximum rotation value on the x-axis (pitch).")]
    [SerializeField] private float _maxPitch = 80f;
    
    private float   _upwardOffset;
    private float   _backwardOffset;
    [Header("Screen Shake")]
    [SerializeField] private float _shakeIntensity = 1f;
    
    private Vector3 _rotationEuler;

    private Vector2 _rotateInput;
    
    private Camera _camera;
    
    private Coroutine _shakeCoroutine;
    private Vector3 _shakeOffset;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _rotationEuler = transform.rotation.eulerAngles;
        
        var toTarget = transform.position - _target.position;
        var projection = Vector3.Project(toTarget, transform.forward);

        _upwardOffset = (toTarget - projection).magnitude;
        _backwardOffset = projection.magnitude;
    }

    private void Update()
    {
        if (MenuManager.Paused) return;
        transform.position = _target.position;
        
        float yawDelta = _rotateInput.x * _rotationSpeed * Time.deltaTime;
        float yaw = (_rotationEuler.y + yawDelta) % 360;
        
        float pitchDelta = -_rotateInput.y * _rotationSpeed * Time.deltaTime;
        float pitch = Mathf.Clamp(_rotationEuler.x + pitchDelta, _minPitch, _maxPitch);
        
        _rotationEuler = new Vector3(pitch, yaw, 0);
        
        transform.rotation = Quaternion.Euler(_rotationEuler);
        
        transform.position -= transform.forward * _backwardOffset;
        transform.position += transform.up * _upwardOffset;
        transform.position += _shakeOffset;
        
        //transform.position = _lerpPosition ? LerpPosition() : _target.position + _offset;
        
        if (_lerpZoom)
            _camera.fieldOfView = LerpZoom();
    }

    // private Vector3 LerpPosition()
    // {
    //     Vector3 followPoint = _target.position + _followOffset * _followBehaviour switch
    //     {
    //         FollowBehaviour.Ahead => _target.linearVelocity.normalized,
    //         _ => Vector3.zero
    //     };
    //
    //     return Vector3.Lerp(transform.position, followPoint + _offset, _positionLerpSpeed);
    // }

    private float LerpZoom()
    {
        bool zoomOut = _target.linearVelocity.sqrMagnitude > .5f;
        
        float targetFOV = zoomOut ? _maxFOV : _minFOV;
        float lerpSpeed = zoomOut ? _zoomOutLerpSpeed : _zoomInLerpSpeed;
        
        return Mathf.Lerp(_camera.fieldOfView, targetFOV, lerpSpeed);
    }

    public void Shake(ScreenShakeEvent shakeEvent)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
            _shakeOffset = Vector3.zero;
        }
        
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(shakeEvent));
    }

    private IEnumerator ShakeCoroutine(ScreenShakeEvent shakeEvent)
    {
        float elapsedTime = 0;

        while (elapsedTime < shakeEvent.Duration)
        {
            float t = Mathf.Clamp01(elapsedTime / shakeEvent.Duration);
            float intensity = _shakeIntensity * shakeEvent.IntensityMultiplier * shakeEvent.IntensityCurve.Evaluate(t);

            Vector3 shakeDir = Random.insideUnitSphere;
            Vector3 projection = Vector3.Project(shakeDir, transform.forward);

            _shakeOffset = intensity * (shakeDir - projection);
            
            yield return null;
            
            elapsedTime += Time.deltaTime;
        }
    }

    public void RotateInput(InputAction.CallbackContext context)
    {
        _rotateInput = context.ReadValue<Vector2>();
    }
}