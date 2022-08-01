using System.Collections;
using System.Collections.Generic;
using Code.Cube;
using Code.Input;
using Code.Input.Enums.Swipe;
using Code.System.Components;
using Code.System.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sequence = DG.Tweening.Sequence;


namespace Code.System
{
    public class NodesController : MonoBehaviour
    {
        [SerializeField] private CubesMoveComponent _cubesMoveComponent;
        [SerializeField] private NodesComponent _nodesComponent;

        [Header("Input")] [SerializeField] private SwipeDetection _swipeDetection;
        [SerializeField] private float _timeDetectedSwipe = 2.5f;

        [Header("Tilemap data")] [SerializeField]
        private Tilemap _blockZone;

        [SerializeField] private Tilemap _unitsZone;


        private List<Transform> _blockListNodes;
        private List<Transform> _unitListNodes;

        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            _blockListNodes = new List<Transform>();
            _unitListNodes = new List<Transform>();

            _waitForSeconds = new WaitForSeconds(_timeDetectedSwipe);
        }

        private void OnEnable() => _swipeDetection.TouchComplete += BeginLogicMove;
        private void OnDisable() => _swipeDetection.TouchComplete -= BeginLogicMove;

        private void Start()
        {
            _nodesComponent.SetListNodes(_blockZone.transform, _blockListNodes);
        }

        private void BeginLogicMove(ESwapDirection eSwapDirection, Vector2 direction)
        {
            if (!_swipeDetection.GetInput().GetStateInput()) return;

            StartCoroutine(ChangeInputState());

            _nodesComponent.SetListNodes(_unitsZone.transform, _unitListNodes);

            _nodesComponent.Clear();
            _nodesComponent.SetLineNodes(_unitListNodes, _unitListNodes[0], eSwapDirection);
            _nodesComponent.InvokeFindPoint(eSwapDirection);

            Vector3 posRotate =
                new Vector3(_cubesMoveComponent.GetDataAnim.RotateAngel.x * direction.y,
                    0, _cubesMoveComponent.GetDataAnim.RotateAngel.z * direction.x);

            Vector3 directionCube = new Vector3(direction.x, 0, direction.y);

            foreach (var line in _nodesComponent.GetLineNodeList())
            {
                CalculatePossibleStepsNodes(line, directionCube, posRotate);
            }
        }

        private void CalculatePossibleStepsNodes(LineNodes data, Vector3 direction, Vector3 posRotate)
        {
            if (!IsCanMove(data, direction))
            {
                StartCoroutine(LogicMoveNodes(data, posRotate, direction));
                return;
            }

            for (int i = 0; i < data.NodePos.Count; i++)
            {
                int step = _nodesComponent.GetStepNode(data.NodePos[i], direction, data, _blockListNodes);
                if (step == 0) continue;

                Vector3 possiblePosition = (data.NodePos[i] + (direction * step));
                data.NodePos[i] = possiblePosition;
            }


            CalculatePossibleStepsNodes(data, direction, posRotate);
        }

        private bool IsCanMove(LineNodes nodes, Vector3 dir)
        {
            foreach (var node in nodes.NodePos)
            {
                int step = _nodesComponent.GetStepNode(node, dir, nodes, _blockListNodes);
                if (step != 0) return true;
            }

            return false;
        }

        private IEnumerator ChangeInputState()
        {
            _swipeDetection.GetInput().InvokeDisableInput();
            yield return _waitForSeconds;
            _swipeDetection.GetInput().InvokeEnableInput();
        }

        private IEnumerator LogicMoveNodes(LineNodes data, Vector3 posRotate, Vector3 dir)
        {
            yield return StartCoroutine(ApplyAnimatedMoveNodes(data, posRotate));
            yield return StartCoroutine(ApplyAnimatedStackNodes(data, posRotate, dir));
        }

        private IEnumerator ApplyAnimatedStackNodes(LineNodes data, Vector3 posRotate, Vector3 dir)
        {
            if (data.Node.Count == 1)
            {
                yield break;
            }

            ComponentLevel stack = data.Node[0].GetComponent<ComponentLevel>();

            for (int i = 0; i < data.Node.Count; i++)
            {
                Vector3 possibleStep = data.NodePos[i] + dir;
                ComponentLevel moveToStack = data.Node[i].GetComponent<ComponentLevel>();

                if (_nodesComponent.ContainsBlockZone(possibleStep, _blockListNodes))
                {
                    continue;
                }

                if (data.NodePos.Contains(possibleStep))
                {
                    if (moveToStack.GetWeight() == stack.GetWeight())
                    {
                        var stack1 = stack;
                        yield return _cubesMoveComponent
                            .AnimatedMoveCube(moveToStack.transform, possibleStep, posRotate, true)
                            .OnKill((() =>
                            {
                                stack1.UpLevel();
                                moveToStack.gameObject.SetActive(false);
                            })).WaitForCompletion();

                        data.NodePos.Remove(data.NodePos[i]);
                        data.Node.Remove(data.Node[i]);


                        for (int j = i; j < data.Node.Count; j++)
                        {
                            data.NodePos[j] += dir;
                            yield return _cubesMoveComponent
                                .AnimatedMoveCube(data.Node[j], data.NodePos[j], posRotate, true)
                                .WaitForCompletion();
                        }


                        if (i >= data.Node.Count)
                        {
                            StartCoroutine(ApplyAnimatedStackNodes(data, posRotate, dir));
                            yield break;
                        }
                    }

                    stack = data.Node[i].GetComponent<ComponentLevel>();
                }
            }
        }

        private IEnumerator ApplyAnimatedMoveNodes(LineNodes data, Vector3 posRotate)
        {
            Sequence seqMove = DOTween.Sequence();
            for (int i = 0; i < data.NodePos.Count; i++)
            {
                if (data.Node[i].localPosition == data.NodePos[i])
                {
                    continue;
                }

                seqMove = _cubesMoveComponent.AnimatedMoveCube(data.Node[i], data.NodePos[i], posRotate);
            }


            yield return seqMove.WaitForCompletion();
        }
    }
}