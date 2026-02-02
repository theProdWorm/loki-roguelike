using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Player
{
    public partial class Player
    {
        [SerializeField] protected Rigidbody _rigidbody;

        private Camera _camera;
        
        private Vector3 _rightDirection;
        private Vector3 _forwardDirection;
        
        private Vector2 _moveInput;
        private Vector2 _aimInput;

        private void InitializeMovement()
        {
            _camera = Camera.main!;
            
            _rightDirection = _camera.transform.right.normalized;

            var cameraForward = _camera.transform.forward;
            var downProjection = Vector3.Project(cameraForward, Vector3.up);
            
            _forwardDirection = (cameraForward - downProjection).normalized;
        }
        
        private void Update()
        {
            Vector3 movementX = _moveInput.x * _rightDirection;
            Vector3 movementZ = _moveInput.y * _forwardDirection;
            
            Vector3 movement = _moveSpeed * (movementX + movementZ);
            
            _rigidbody.linearVelocity = movement + new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            _aimInput = context.ReadValue<Vector2>();
        }
    }
}