using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.Code.Input
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : MonoBehaviour
    {
        public delegate void StartTouch(Vector2 position, double time);

        public event StartTouch OnStartTouch;

        public delegate void EndTouch(Vector2 position, double time);

        public event EndTouch OnEndTouch;


        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
        }


        private void OnEnable()
        {
            _playerControls.Enable();
            
            _playerControls.Touch.PrimaryContact.started += StartTouchPrimary;
            _playerControls.Touch.PrimaryContact.canceled += EndTouchPrimary;
        }

        private void OnDisable()
        {
            _playerControls.Disable();

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