using Code.System.Data;
using DG.Tweening;
using UnityEngine;
using System;

namespace Code.System.Components
{
    [Serializable]
    public class CubesMoveComponent
    {
        [Header("Animation Data")] [SerializeField]
        private DataAnim _dataAnim;

        public DataAnim GetDataAnim => _dataAnim;

        public Sequence AnimatedMoveCube(Transform cube, Vector3 possiblePosition, Vector3 posRotate,
            bool isStack = false)
        {
            Sequence sequence = DOTween.Sequence();
            float duration = isStack ? _dataAnim.DurationStack : _dataAnim.DurationMove;

            return sequence.Append(cube.DOLocalMove(possiblePosition, duration).SetEase(_dataAnim.Ease)
                .OnStart(
                    (() =>
                        {
                            cube.DORotate(new Vector3(0, 0, 0), 0);
                            cube.DOPunchRotation(posRotate, _dataAnim.DurationMove, _dataAnim.Vibrato,
                                _dataAnim.Elasticity);
                        }
                    )));
        }
    }
}