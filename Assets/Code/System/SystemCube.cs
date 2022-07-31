using System;
using System.Collections;
using System.Collections.Generic;
using Code.Cube;
using DG.Tweening;
using Scenes.Code.Input;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sequence = DG.Tweening.Sequence;


[System.Serializable]
struct DataDir
{
    public Vector3 DirectionMove;
    public Vector3 DirectionRotate;
    public SwapDirection SwapDirection;
}

[System.Serializable]
struct DataAnim
{
    public float DurationMove;
    public float DurationStack;
    public int Vibrato;
    public float Elasticity;

    public Ease Ease;
}

[System.Serializable]
struct LineNodes
{
    [SerializeField] public List<Transform> Node;
    [SerializeField] public List<Vector3> NodePos;
}

public class SystemCube : MonoBehaviour
{
    [Header("Input")] [SerializeField] private SwipeDetection _swipeDetection;
    [SerializeField] private float _timeDetectedSwipe = 2.5f;

    [Header("Tilemap data")] [SerializeField]
    private Tilemap _blockZone;

    [SerializeField] private Tilemap _cubes;

    [Header("Option Move Cell")] [SerializeField]
    private DataDir[] _dataDir;

    [Header("Animation Data")] [SerializeField]
    private DataAnim _dataAnim;

    [Header("Debug list")] [SerializeField]
    private List<LineNodes> lineNodesList;

    private List<Transform> _dataBlockedZone;
    private List<Transform> _dataCube;

    private bool _isPlay;

    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _dataBlockedZone = new List<Transform>();
        _dataCube = new List<Transform>();
        lineNodesList = new List<LineNodes>();

        _waitForSeconds = new WaitForSeconds(_timeDetectedSwipe);
    }

    private void Start()
    {
        SetChildrenArray(_blockZone.transform, _dataBlockedZone);
    }

    private void OnEnable() => _swipeDetection.TouchComplete += Play;
    private void OnDisable() => _swipeDetection.TouchComplete -= Play;

    private void Play()
    {
        if (_isPlay) return;

        StartCoroutine(PlayCorutine());
        SetChildrenArray(_cubes.transform, _dataCube);
        lineNodesList.Clear();


        DataDir dataDir = GetDataDir();
        Vector3 direction = dataDir.DirectionMove;
        Vector3 posRotate = dataDir.DirectionRotate;


        switch (dataDir.SwapDirection)
        {
            case SwapDirection.Up:
                SetLineNodesXpos(_dataCube, _dataCube[0]);
                FindMinPointZ();
                break;
            case SwapDirection.Down:
                SetLineNodesXpos(_dataCube, _dataCube[0]);
                FindMaxPointZ();
                break;
            case SwapDirection.Left:
                SetLineNodesZpos(_dataCube, _dataCube[0]);
                FindMaxPointX();
                break;
            case SwapDirection.Right:
                SetLineNodesZpos(_dataCube, _dataCube[0]);
                FindMinPointX();
                break;
        }

        foreach (var line in lineNodesList)
        {
            MoveToCell(line, direction, posRotate);
        }
    }

    private void SetChildrenArray(Transform parent, List<Transform> list)
    {
        list.Clear();
        for (int i = 0; i < parent.childCount; i++)
        {
            if (!parent.GetChild(i).gameObject.activeInHierarchy) continue;

            list.Add(parent.GetChild(i));
        }
    }

    private bool ContainsBlockZone(Vector3 cubePos)
    {
        foreach (var blockZone in _dataBlockedZone)
        {
            if (blockZone.localPosition == cubePos)
            {
                return true;
            }
        }

        return false;
    }

    private int GetStepCube(Vector3 cubePos, Vector3 direction, LineNodes nodes)
    {
        int step = 1;
        Vector3 possiblePosition = (cubePos + (direction * step));

        if (ContainsBlockZone(possiblePosition))
        {
            return 0;
        }

        if (nodes.NodePos.Contains(possiblePosition))
        {
            return 0;
        }


        while (!ContainsBlockZone(possiblePosition) &&
               !nodes.NodePos.Contains(possiblePosition))
        {
            step++;
            possiblePosition = (cubePos + (direction * step));
        }

        return step - 1;
    }

    private bool IsCanMove(LineNodes nodes, Vector3 dir)
    {
        foreach (var node in nodes.NodePos)
        {
            int step = GetStepCube(node, dir, nodes);
            if (step != 0) return true;
        }

        return false;
    }

    private void MoveToCell(LineNodes data, Vector3 direction, Vector3 posRotate)
    {
        if (!IsCanMove(data, direction))
        {
            StartCoroutine(MoveGroup(data, posRotate, direction));
            return;
        }

        for (int i = 0; i < data.NodePos.Count; i++)
        {
            int step = GetStepCube(data.NodePos[i], direction, data);
            if (step == 0) continue;

            Vector3 possiblePosition = (data.NodePos[i] + (direction * step));
            data.NodePos[i] = possiblePosition;
        }


        MoveToCell(data, direction, posRotate);
    }

    private void FindMaxPointX()
    {
        for (int i = 0; i < lineNodesList.Count; i++)
        {
            if (lineNodesList[i].Node.Count == 0) continue;

            for (int j = 0; j < lineNodesList[i].Node.Count - 1; j++)
            {
                if (!(lineNodesList[i].Node[j].localPosition.x < lineNodesList[i].Node[j + 1].localPosition.x))
                    continue;

                var t = lineNodesList[i].Node[j];
                lineNodesList[i].Node[j] = lineNodesList[i].Node[j + 1];
                lineNodesList[i].Node[j + 1] = t;


                var t1 = lineNodesList[i].NodePos[j];
                lineNodesList[i].NodePos[j] = lineNodesList[i].NodePos[j + 1];
                lineNodesList[i].NodePos[j + 1] = t1;

                FindMaxPointX();
            }
        }
    }

    private void FindMinPointX()
    {
        for (int i = 0; i < lineNodesList.Count; i++)
        {
            if (lineNodesList[i].Node.Count == 0) continue;

            for (int j = 0; j < lineNodesList[i].Node.Count - 1; j++)
            {
                if (!(lineNodesList[i].Node[j].localPosition.x > lineNodesList[i].Node[j + 1].localPosition.x))
                    continue;

                var t = lineNodesList[i].Node[j];
                lineNodesList[i].Node[j] = lineNodesList[i].Node[j + 1];
                lineNodesList[i].Node[j + 1] = t;


                var t1 = lineNodesList[i].NodePos[j];
                lineNodesList[i].NodePos[j] = lineNodesList[i].NodePos[j + 1];
                lineNodesList[i].NodePos[j + 1] = t1;

                FindMinPointX();
            }
        }
    }

    private void FindMaxPointZ()
    {
        for (int i = 0; i < lineNodesList.Count; i++)
        {
            if (lineNodesList[i].Node.Count == 0) continue;

            for (int j = 0; j < lineNodesList[i].Node.Count - 1; j++)
            {
                if (!(lineNodesList[i].Node[j].localPosition.z < lineNodesList[i].Node[j + 1].localPosition.z))
                    continue;

                var t = lineNodesList[i].Node[j];
                lineNodesList[i].Node[j] = lineNodesList[i].Node[j + 1];
                lineNodesList[i].Node[j + 1] = t;


                var t1 = lineNodesList[i].NodePos[j];
                lineNodesList[i].NodePos[j] = lineNodesList[i].NodePos[j + 1];
                lineNodesList[i].NodePos[j + 1] = t1;

                FindMaxPointZ();
            }
        }
    }

    private void FindMinPointZ()
    {
        for (int i = 0; i < lineNodesList.Count; i++)
        {
            if (lineNodesList[i].Node.Count == 0) continue;

            for (int j = 0; j < lineNodesList[i].Node.Count - 1; j++)
            {
                if (!(lineNodesList[i].Node[j].localPosition.z > lineNodesList[i].Node[j + 1].localPosition.z))
                    continue;

                var t = lineNodesList[i].Node[j];
                lineNodesList[i].Node[j] = lineNodesList[i].Node[j + 1];
                lineNodesList[i].Node[j + 1] = t;


                var t1 = lineNodesList[i].NodePos[j];
                lineNodesList[i].NodePos[j] = lineNodesList[i].NodePos[j + 1];
                lineNodesList[i].NodePos[j + 1] = t1;

                FindMinPointZ();
            }
        }
    }

    private void SetLineNodesZpos(List<Transform> data, Transform node)
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
                var t = data[j];
                float equAbs = (Math.Abs(t.localPosition.z - node.localPosition.z));
                if (equAbs == 0)
                {
                    lineNodes.Node.Add(data[j]);
                    lineNodes.NodePos.Add(data[j].localPosition);
                    newData[j] = null;
                }
            }

            lineNodesList.Add(lineNodes);
        }
    }

    private void SetLineNodesXpos(List<Transform> data, Transform node)
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
                var t = data[j];
                float equAbs = (Math.Abs(t.localPosition.x - node.localPosition.x));
                if (equAbs == 0)
                {
                    lineNodes.Node.Add(data[j]);
                    lineNodes.NodePos.Add(data[j].localPosition);
                    newData[j] = null;
                }
            }

            lineNodesList.Add(lineNodes);
        }
    }

    private DataDir GetDataDir()
    {
        DataDir getedData = new DataDir();

        foreach (var data in _dataDir)
        {
            if (data.SwapDirection == _swipeDetection.GetSwapDirection())
            {
                getedData = data;
            }
        }

        return getedData;
    }

    private IEnumerator PlayCorutine()
    {
        _isPlay = true;
        yield return _waitForSeconds;
        _isPlay = false;
    }

    private IEnumerator MoveGroup(LineNodes data, Vector3 posRotate, Vector3 dir)
    {
        yield return StartCoroutine(Moves(data, posRotate));
        yield return StartCoroutine(Stack(data, posRotate, dir));
    }

    private IEnumerator Stack(LineNodes data, Vector3 posRotate, Vector3 dir)
    {
        if (data.Node.Count == 1) yield break;

        ComponentLevel stack = data.Node[0].GetComponent<ComponentLevel>();

        for (int i = 0; i < data.Node.Count; i++)
        {
            Vector3 possibleStep = data.NodePos[i] + dir;
            ComponentLevel moveToStack = data.Node[i].GetComponent<ComponentLevel>();

            if (ContainsBlockZone(possibleStep))
            {
                continue;
            }

            if (data.NodePos.Contains(possibleStep))
            {
                if (moveToStack.GetWeight() == stack.GetWeight())
                {
                    var stack1 = stack;
                    yield return MoveCube(moveToStack.transform, possibleStep, posRotate, true)
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
                        yield return MoveCube(data.Node[j], data.NodePos[j], posRotate, true).WaitForCompletion();
                    }


                    if (i >= data.Node.Count)
                    {
                        StartCoroutine(Stack(data, posRotate, dir));
                        yield break;
                    }
                }

                stack = data.Node[i].GetComponent<ComponentLevel>();
            }
        }
    }

    private IEnumerator Moves(LineNodes data, Vector3 posRotate)
    {
        Sequence seqMove = DOTween.Sequence();
        for (int i = 0; i < data.NodePos.Count; i++)
        {
            if (data.Node[i].localPosition == data.NodePos[i])
            {
                continue;
            }

            seqMove = MoveCube(data.Node[i], data.NodePos[i], posRotate);
        }


        yield return seqMove.WaitForCompletion();
    }

    private Sequence MoveCube(Transform cube, Vector3 possiblePosition, Vector3 posRotate, bool isStack = false)
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