using System;
using UnityEngine;

public enum SwapDirection
{
    Up,
    Down,
    Left,
    Right
}

namespace Scenes.Code.Input
{
    public class SwipeDetection : MonoBehaviour
    {
        public delegate void OnTouchComplete();

        public event OnTouchComplete TouchComplete;

        [SerializeField] private InputManager _inputManager;


        private Vector2 _startPosition;
        private Vector2 _endPosition;

        [SerializeField] private float _deadZone;

        private SwapDirection _swap;

        private void Start()
        {
            _inputManager.OnStartTouch += SwipeStart;
            _inputManager.OnEndTouch += SwipeEnd;
        }

        private void OnEnable()
        {
            _inputManager.OnStartTouch += SwipeStart;
            _inputManager.OnEndTouch += SwipeEnd;
        }


        private void OnDisable()
        {
            _inputManager.OnStartTouch -= SwipeStart;
            _inputManager.OnEndTouch -= SwipeEnd;
        }

        private void SwipeStart(Vector2 position, double time)
        {
            _startPosition = position;
        }

        private void SwipeEnd(Vector2 position, double time)
        {
            _endPosition = position;

            DetectSwipe();
        }

        private void DetectSwipe()
        {
            Vector3 direction = (_endPosition - _startPosition);

            if (direction.magnitude > _deadZone)
            {
                SwipeDirection(direction);
            }
        }

        private void SwipeDirection(Vector2 direction)
        {
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                _swap = direction.x > 0 ? SwapDirection.Right : SwapDirection.Left;
            }
            else
            {
                _swap = direction.y > 0 ? SwapDirection.Up : SwapDirection.Down;
            }

            TouchComplete?.Invoke();
        }

        public SwapDirection GetSwapDirection() => _swap;
    }
}