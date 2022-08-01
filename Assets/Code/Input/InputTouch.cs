using Code.Input.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Input
{
    public class InputTouch : InputBase
    {
        public delegate void StartTouch(Vector2 position, double time);

        public event StartTouch OnStartTouch;

        public delegate void EndTouch(Vector2 position, double time);

        public event EndTouch OnEndTouch;

        private PlayerControls _playerControls;

        protected override void Awake()
        {
            base.Awake();
            
            _playerControls = new PlayerControls();

            SetInputActionAsset(_playerControls.asset);
        }

        private void OnEnable()
        {
            InvokeEnableInput();

            _playerControls.Touch.PrimaryContact.started += StartTouchPrimary;
            _playerControls.Touch.PrimaryContact.canceled += EndTouchPrimary;
        }

        private void OnDisable()
        {
            InvokeDisableInput();

            _playerControls.Touch.PrimaryContact.started -= StartTouchPrimary;
            _playerControls.Touch.PrimaryContact.canceled -= EndTouchPrimary;
        }

        private void StartTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnStartTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(),
                ctx.startTime);
        }

        private void EndTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnEndTouch?.Invoke(_playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(),
                ctx.time);
        }
    }
}