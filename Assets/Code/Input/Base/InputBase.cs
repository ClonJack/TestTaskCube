using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Input.Base
{
    [DefaultExecutionOrder(-1)]
    public abstract class InputBase : MonoBehaviour
    {
        private InputActionAsset _input;

        protected delegate void StartInput();

        protected StartInput OnStartInput;

        protected delegate void EndInput();

        protected EndInput OnEndInput;

        protected bool _isEnableInput;

        protected virtual void Awake()
        {
            OnStartInput += InputEnabled;
            OnEndInput += InputDisabled;
        }

        public virtual bool GetStateInput() => _isEnableInput;

        public virtual void InvokeEnableInput()
        {
            _isEnableInput = true;
            OnStartInput?.Invoke();
        }

        public virtual void InvokeDisableInput()
        {
            _isEnableInput = false;
            OnEndInput?.Invoke();
        }

        protected virtual void SetInputActionAsset(InputActionAsset actionAsset)
        {
            _input = actionAsset;
        }

        private void InputEnabled() => _input.Enable();
        private void InputDisabled() => _input.Disable();

        private void OnEnable()
        {
            OnStartInput += InputEnabled;
            OnEndInput += InputDisabled;
        }

        private void OnDisable()
        {
            OnStartInput -= InputEnabled;
            OnEndInput -= InputDisabled;
        }
    }
}