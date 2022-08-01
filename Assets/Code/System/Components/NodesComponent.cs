using System;
using System.Collections.Generic;
using Code.Input.Enums.Swipe;
using Code.System.Data;
using UnityEngine;

namespace Code.System.Components
{
    [Serializable]
    public class NodesComponent
    {
        private delegate void OnFindPoint();

        [Header("Debug list")] [SerializeField]
        private List<LineNodes> _lineNodesList;

        private Dictionary<ESwapDirection, OnFindPoint> _findPoint;

        public NodesComponent()
        {
            _lineNodesList = new List<LineNodes>();

            _findPoint = new Dictionary<ESwapDirection, OnFindPoint>
            {
                { ESwapDirection.Up, FindMinPointZ },
                { ESwapDirection.Down, FindMaxPointZ },
                { ESwapDirection.Right, FindMinPointX },
                { ESwapDirection.Left, FindMaxPointZ }
            };
        }


        public int GetStepNode(Vector3 cubePos, Vector3 direction, LineNodes nodes, List<Transform> listBlockZone)
        {
            int step = 1;
            Vector3 possiblePosition = (cubePos + (direction * step));

            if (ContainsBlockZone(possiblePosition, listBlockZone))
            {
                return 0;
            }

            if (nodes.NodePos.Contains(possiblePosition))
            {
                return 0;
            }


            while (!ContainsBlockZone(possiblePosition, listBlockZone) &&
                   !nodes.NodePos.Contains(possiblePosition))
            {
                step++;
                possiblePosition = (cubePos + (direction * step));
            }

            return step - 1;
        }


        public bool ContainsBlockZone(Vector3 cubePos, List<Transform> blockListNodes)
        {
            foreach (var blockZone in blockListNodes)
            {
                if (blockZone.localPosition == cubePos)
                {
                    return true;
                }
            }

            return false;
        }


        public void SetListNodes(Transform parent, List<Transform> list)
        {
            list.Clear();
            for (int i = 0; i < parent.childCount; i++)
            {
                if (!parent.GetChild(i).gameObject.activeInHierarchy) continue;

                list.Add(parent.GetChild(i));
            }
        }

        public void Clear()
        {
            _lineNodesList.Clear();
        }

        public void InvokeFindPoint(ESwapDirection eSwapDirection)
        {
            _findPoint[eSwapDirection].Invoke();
        }

        public void SetLineNodes(List<Transform> data, Transform node, ESwapDirection eSwapDirection)
        {
            List<Transform> newData = new List<Transform>(data);
            for (int i = 0; i < newData.Count; i++)
            {
                LineNodes lineNodes = new LineNodes
                {
                    Node = new List<Transform>(),
                    NodePos = new List<Vector3>()
                };

                if (newData[i] == null) continue;

                node = newData[i];
                for (var j = 0; j < newData.Count; j++)
                {
                    if (newData[j] == null) continue;

                    Transform t = data[j];


                    float equAbs = (eSwapDirection == ESwapDirection.Left || eSwapDirection == ESwapDirection.Right)
                        ? (MathF.Abs(t.localPosition.z - node.localPosition.z))
                        : (MathF.Abs(t.localPosition.x - node.localPosition.x));

                    if (equAbs == 0)
                    {
                        lineNodes.Node.Add(data[j]);
                        lineNodes.NodePos.Add(data[j].localPosition);
                        newData[j] = null;
                    }
                }

                _lineNodesList.Add(lineNodes);
            }
        }

        public List<LineNodes> GetLineNodeList() => _lineNodesList;

        private void FindMaxPointX()
        {
            for (int i = 0; i < _lineNodesList.Count; i++)
            {
                if (_lineNodesList[i].Node.Count == 0) continue;

                for (int j = 0; j < _lineNodesList[i].Node.Count - 1; j++)
                {
                    if (!(_lineNodesList[i].Node[j].localPosition.x < _lineNodesList[i].Node[j + 1].localPosition.x))
                        continue;

                    var t = _lineNodesList[i].Node[j];
                    _lineNodesList[i].Node[j] = _lineNodesList[i].Node[j + 1];
                    _lineNodesList[i].Node[j + 1] = t;


                    var t1 = _lineNodesList[i].NodePos[j];
                    _lineNodesList[i].NodePos[j] = _lineNodesList[i].NodePos[j + 1];
                    _lineNodesList[i].NodePos[j + 1] = t1;

                    FindMaxPointX();
                }
            }
        }

        private void FindMinPointX()
        {
            for (int i = 0; i < _lineNodesList.Count; i++)
            {
                if (_lineNodesList[i].Node.Count == 0) continue;

                for (int j = 0; j < _lineNodesList[i].Node.Count - 1; j++)
                {
                    if (!(_lineNodesList[i].Node[j].localPosition.x > _lineNodesList[i].Node[j + 1].localPosition.x))
                        continue;

                    var t = _lineNodesList[i].Node[j];
                    _lineNodesList[i].Node[j] = _lineNodesList[i].Node[j + 1];
                    _lineNodesList[i].Node[j + 1] = t;


                    var t1 = _lineNodesList[i].NodePos[j];
                    _lineNodesList[i].NodePos[j] = _lineNodesList[i].NodePos[j + 1];
                    _lineNodesList[i].NodePos[j + 1] = t1;

                    FindMinPointX();
                }
            }
        }

        private void FindMaxPointZ()
        {
            for (int i = 0; i < _lineNodesList.Count; i++)
            {
                if (_lineNodesList[i].Node.Count == 0) continue;

                for (int j = 0; j < _lineNodesList[i].Node.Count - 1; j++)
                {
                    if (!(_lineNodesList[i].Node[j].localPosition.z < _lineNodesList[i].Node[j + 1].localPosition.z))
                        continue;

                    var t = _lineNodesList[i].Node[j];
                    _lineNodesList[i].Node[j] = _lineNodesList[i].Node[j + 1];
                    _lineNodesList[i].Node[j + 1] = t;


                    var t1 = _lineNodesList[i].NodePos[j];
                    _lineNodesList[i].NodePos[j] = _lineNodesList[i].NodePos[j + 1];
                    _lineNodesList[i].NodePos[j + 1] = t1;

                    FindMaxPointZ();
                }
            }
        }

        private void FindMinPointZ()
        {
            for (int i = 0; i < _lineNodesList.Count; i++)
            {
                if (_lineNodesList[i].Node.Count == 0) continue;

                for (int j = 0; j < _lineNodesList[i].Node.Count - 1; j++)
                {
                    if (!(_lineNodesList[i].Node[j].localPosition.z > _lineNodesList[i].Node[j + 1].localPosition.z))
                        continue;

                    var t = _lineNodesList[i].Node[j];
                    _lineNodesList[i].Node[j] = _lineNodesList[i].Node[j + 1];
                    _lineNodesList[i].Node[j + 1] = t;


                    var t1 = _lineNodesList[i].NodePos[j];
                    _lineNodesList[i].NodePos[j] = _lineNodesList[i].NodePos[j + 1];
                    _lineNodesList[i].NodePos[j + 1] = t1;

                    FindMinPointZ();
                }
            }
        }
    }
}