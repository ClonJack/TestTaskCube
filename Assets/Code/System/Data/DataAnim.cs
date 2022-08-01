using DG.Tweening;
using UnityEngine;
using System;

namespace Code.System.Data
{
    [Serializable]
    public struct DataAnim
    {
        public float DurationMove;
        public float DurationStack;
        public int Vibrato;
        public float Elasticity;

        public Vector3 RotateAngel;

        public Ease Ease;
    }
}