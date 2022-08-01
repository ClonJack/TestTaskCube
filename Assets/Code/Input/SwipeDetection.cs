using System;
using Code.Input.Base;
using Code.Input.Enums.Swipe;
using UnityEditor.Media;
using UnityEngine;

namespace Code.Input
{
    public class SwipeDetection : MonoBehaviour
    {
        [SerializeField] private InputTouch _inputTouch;


        private Vector2 _startPosition;
        private Vector2 _endPosition;

        [SerializeField] private float _deadZone;

        private ESwapDirection _swap;

        public delegate void OnTouchComplete(ESwapDirection swapDirection, Vector2 dir);

        public event OnTouchComplete TouchComplete;

        public InputBase GetInput() => _inputTouch;

        private void Start()
        {
            _inputTouch.OnStartTouch += SwipeStart;
            _inputTouch.OnEndTouch += SwipeEnd;
        }

        private void OnEnable()
        {
            _inputTouch.OnStartTouch += SwipeStart;
            _inputTouch.OnEndTouch += SwipeEnd;
        }

        private void OnDisable()
        {
            _inputTouch.OnStartTouch -= SwipeStart;
            _inputTouch.OnEndTouch -= SwipeEnd;
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
            Vector2 dirStep = Vector2.zero;
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                _swap = direction.x > 0 ? ESwapDirection.Right : ESwapDirection.Left;
                dirStep = _swap == ESwapDirection.Right ? Vector2.left : Vector2.right;
            }
            else
            {
                _swap = direction.y > 0 ? ESwapDirection.Up : ESwapDirection.Down;
                dirStep = _swap == ESwapDirection.Up ? Vector2.down : Vector2.up;
            }

            TouchComplete?.Invoke(_swap, dirStep);
        }

        public ESwapDirection GetSwapDirection() => _swap;
    }
}