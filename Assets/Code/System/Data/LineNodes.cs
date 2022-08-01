using System.Collections.Generic;
using UnityEngine;
using System;

namespace Code.System.Data
{
    [Serializable]
    public struct LineNodes
    {
        [SerializeField] public List<Transform> Node;
        [SerializeField] public List<Vector3> NodePos;
    }
}