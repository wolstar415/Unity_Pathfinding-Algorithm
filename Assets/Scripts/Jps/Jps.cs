using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jps
{
    public Vector3Int startPos;
    public Vector3Int endPos;
    public List<Node> finalNodeList = new List<Node>();
    public List<Node> openNodeList = new List<Node>();
    public List<Node> closeNodeList = new List<Node>();
    public Node startNode;
    public Node currentNode;
    public Node endNode;

    readonly int _moveStraightCost = 10;
    readonly int _moveDiagonalCost = 14;

    public Node[,] nodeArray;

    public Jps(Vector3Int _startPos, Vector3Int _endPos)
    {
        nodeArray = new Node[GameManager.inst.sizeX, GameManager.inst.sizeY];
        startPos = _startPos;
        endPos = EndPosCheck(_endPos);
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost = Heuristic(startPos, endPos);
        startNode.isOpen = true;
        startNode.gCost = 0;
        nodeArray[_startPos.x - GameManager.inst.mapMinX, _startPos.y - GameManager.inst.mapMinY] =
            new Node(_startPos.x, _startPos.y);

        openNodeList.Add(startNode);

        Find();
    }

    #region 목표지점을 갈 수 있는지 체크

    private Vector3Int EndPosCheck(Vector3Int check)
    {
        //PriorityQueue< Node, int > checkQueue = new PriorityQueue< Node, int >();
        List<Node> checkNodeList = new List<Node>();
        Node cur = new Node(check.x, check.y);
        Dictionary<Vector2Int, Node> check2 = new Dictionary<Vector2Int, Node>();

        checkNodeList.Add(cur);
        check2.Add(new Vector2Int(cur.gridX, cur.gridY), cur);
        while (checkNodeList.Count > 0)
        {
            cur = checkNodeList[0];
            for (int i = 0; i < checkNodeList.Count; i++)
            {
                if (checkNodeList[i].hCost < cur.hCost)
                {
                    cur = checkNodeList[i];
                }
            }

            checkNodeList.Remove(cur);


            if (Physics2D.OverlapCircle(new Vector2(cur.gridX, cur.gridY), 0.4f, GameManager.inst.wallLayerMask))

            {
                if (!check2.ContainsKey(new Vector2Int(cur.gridX - 1, cur.gridY)))
                {
                    Node left = new Node(cur.gridX - 1, cur.gridY);
                    left.hCost = (Mathf.Abs(startPos.x - left.gridX) + Mathf.Abs(startPos.y - left.gridY)) * 10;
                    checkNodeList.Add(left);
                    check2.Add(new Vector2Int(left.gridX, left.gridY), left);
                }

                if (!check2.ContainsKey(new Vector2Int(cur.gridX + 1, cur.gridY)))
                {
                    Node right = new Node(cur.gridX + 1, cur.gridY);
                    right.hCost = (Mathf.Abs(startPos.x - right.gridX) + Mathf.Abs(startPos.y - right.gridY)) * 10;
                    checkNodeList.Add(right);
                    check2.Add(new Vector2Int(right.gridX, right.gridY), right);
                }

                if (!check2.ContainsKey(new Vector2Int(cur.gridX, cur.gridY + 1)))
                {
                    Node up = new Node(cur.gridX, cur.gridY + 1);
                    up.hCost = (Mathf.Abs(startPos.x - up.gridX) + Mathf.Abs(startPos.y - up.gridY)) * 10;
                    checkNodeList.Add(up);
                    check2.Add(new Vector2Int(up.gridX, up.gridY), up);
                }

                if (!check2.ContainsKey(new Vector2Int(cur.gridX, cur.gridY - 1)))
                {
                    Node down = new Node(cur.gridX, cur.gridY - 1);
                    down.hCost = (Mathf.Abs(startPos.x - down.gridX) + Mathf.Abs(startPos.y - down.gridY)) * 10;
                    checkNodeList.Add(down);
                    check2.Add(new Vector2Int(down.gridX, down.gridY), down);
                }
            }
            else
            {
                break;
            }
        }


        return new Vector3Int(cur.gridX, cur.gridY);
    }

    #endregion


    public Node GetNode(int _x, int _y)
    {
        int x = _x - GameManager.inst.mapMinX;
        int y = _y - GameManager.inst.mapMinY;
        if (nodeArray[x, y] != null)
        {
            return nodeArray[x, y];
        }

        nodeArray[x, y] = new Node(_x, _y);
        bool isMoveAble = true;
        if (MapSizeCheck(_x, _y))
        {
            if (Physics2D.OverlapCircle(new Vector2(_x, _y), 0.4f, GameManager.inst.dontMoveLayerMask))
            {
                isMoveAble = false;
                nodeArray[x, y].isDontMove = true;
            }
        }
        else
        {
            isMoveAble = false;
            nodeArray[x, y].isDontMove = true;
        }

        nodeArray[x, y].isOpen = isMoveAble;


        return nodeArray[x, y];
    }

    public bool EndCheck(Node _currentNode)
    {
        return _currentNode.Pos() == endPos ||
               (Physics2D.OverlapCircle(new Vector2(endPos.x, endPos.y), 0.4f, GameManager.inst.dontMoveLayerMask)
                && _currentNode.hCost <= 14);
    }

    private void Find()
    {
        while (openNodeList.Count > 0)
        {
            currentNode = openNodeList[0];
            for (int i = 0; i < openNodeList.Count; i++)
            {
                if (openNodeList[i].fCost < currentNode.fCost)
                {
                    currentNode = openNodeList[i];
                }
            }

            openNodeList.Remove(currentNode);
            closeNodeList.Add(currentNode);


            if (EndCheck(currentNode))
            {
                endNode = currentNode;
                Node targetNode = endNode;
                while (targetNode != startNode)
                {
                    finalNodeList.Add(targetNode);
                    targetNode = targetNode.parentNode;
                }

                finalNodeList.Add(startNode);
                finalNodeList.Reverse();
                break;
            }

            if (currentNode.isOpen == true)
            {
                Right(currentNode);
                Left(currentNode);
                Up(currentNode);
                Down(currentNode);
                RightUp(currentNode);
                RightDown(currentNode);
                LeftUp(currentNode);
                LeftDown(currentNode);
            }

            currentNode.isOpen = false;
        }

        if (finalNodeList.Count == 0)
        {
            Node targetNode = closeNodeList[0];
            for (int i = 0; i < closeNodeList.Count; i++)
            {
                if (closeNodeList[i].hCost < targetNode.hCost)
                {
                    targetNode = closeNodeList[i];
                }
            }

            endNode = targetNode;
            while (targetNode != startNode)
            {
                finalNodeList.Add(targetNode);
                targetNode = targetNode.parentNode;
            }

            finalNodeList.Add(startNode);
            finalNodeList.Reverse();
        }
    }

    #region 오른쪽

    private bool Right(Node _startNode, bool _isMovealbe = false)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen)
        {
            _currentNode.isOpen = _isMovealbe;


            if (MapSizeCheck(x + 1, y) == false) // 탐색 가능 여부
            {
                break;
            }

            _currentNode = GetNode(++x, y);


            if (_currentNode.isOpen == false)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                isFind = true;
                if (_isMovealbe == false)
                {
                    AddOpenList(_currentNode, _startNode);
                }

                break;
            }


            #region ↑ 벽 ,↗ 이동가능

            if (MapSizeCheck(x + 1, y + 1))
            {
                if (GetNode(x, y + 1).isDontMove && GetNode(x + 1, y + 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↓ 벽 ,↘ 이동가능

            if (MapSizeCheck(x + 1, y - 1))
            {
                if (GetNode(x, y - 1).isDontMove && GetNode(x + 1, y - 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion
        }

        _startNode.isOpen = true;
        return isFind;
    }

    #endregion

    #region 왼쪽

    private bool Left(Node _startNode, bool _isMovealbe = false)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = _isMovealbe;


            if (MapSizeCheck(x - 1, y) == false) // 탐색 가능 여부
            {
                break;
            }

            _currentNode = GetNode(--x, y); // 다음 왼쪽 노드로 이동

            if (_currentNode.isOpen == false)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                isFind = true;
                if (_isMovealbe == false)
                {
                    AddOpenList(_currentNode, _startNode);
                }

                break;
            }


            #region ↑벽,↖이동가능

            if (MapSizeCheck(x - 1, y + 1))
            {
                if (GetNode(x, y + 1).isDontMove && GetNode(x - 1, y + 1).isDontMove == false) // ↑벽,↖이동가능
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↓벽,↙이동가능

            if (MapSizeCheck(x - 1, y - 1))
            {
                if (GetNode(x, y - 1).isDontMove && GetNode(x - 1, y - 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion
        }

        _startNode.isOpen = true;
        return isFind;
    }

    #endregion

    #region 위

    private bool Up(Node _startNode, bool _isMovealbe = false)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = _isMovealbe;


            if (MapSizeCheck(x, y + 1) == false) // 탐색 가능 여부
            {
                break;
            }

            _currentNode = GetNode(x, ++y);

            if (_currentNode.isOpen == false)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                isFind = true;
                if (_isMovealbe == false)
                {
                    AddOpenList(_currentNode, _startNode);
                }

                break;
            }


            #region →벽,↗이동가능

            if (MapSizeCheck(x + 1, y + 1))
            {
                if (GetNode(x + 1, y).isDontMove && GetNode(x + 1, y + 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ←벽,↖이동가능

            if (MapSizeCheck(x - 1, y + 1))
            {
                if (GetNode(x - 1, y).isDontMove && GetNode(x - 1, y + 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion
        }

        _startNode.isOpen = true;
        return isFind;
    }

    #endregion

    #region 아래

    private bool Down(Node _startNode, bool _isMovealbe = false)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = _isMovealbe;


            if (MapSizeCheck(x, y - 1) == false) // 탐색 가능 여부
            {
                break;
            }

            _currentNode = GetNode(x, --y);

            if (_currentNode.isOpen == false)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                isFind = true;
                if (_isMovealbe == false)
                {
                    AddOpenList(_currentNode, _startNode);
                }

                break;
            }


            #region →벽,↘이동가능

            if (MapSizeCheck(x + 1, y - 1))
            {
                if (GetNode(x + 1, y).isDontMove && GetNode(x + 1, y - 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ←벽,↙이동가능

            if (MapSizeCheck(x - 1, y - 1))
            {
                if (GetNode(x - 1, y).isDontMove && GetNode(x - 1, y - 1).isDontMove == false)
                {
                    if (_isMovealbe == false)
                    {
                        AddOpenList(_currentNode, _startNode);
                    }

                    isFind = true;
                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion
        }

        _startNode.isOpen = true;
        return isFind;
    }

    #endregion


    #region 오른쪽 위

    private void RightUp(Node _startNode)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = false;


            if (MapSizeCheck(x + 1, y + 1) == false) // 탐색 가능 여부
            {
                break;
            }


            _currentNode = GetNode(++x, ++y);


            if (GetNode(x - 1, y).isDontMove && GetNode(x, y - 1).isDontMove) //양쪽 막혀있음
            {
                break;
            }

            if (_currentNode.isOpen == false)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                AddOpenList(_currentNode, _startNode);
                break;
            }


            #region ←벽,↖이동가능

            if (MapSizeCheck(x - 1, y + 1))
            {
                if (GetNode(x - 1, y).isDontMove && GetNode(x - 1, y + 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↓벽,↘이동가능

            if (MapSizeCheck(x + 1, y - 1))
            {
                if (GetNode(x, y - 1).isDontMove && GetNode(x + 1, y - 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            if (Right(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }

            if (Up(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }
        }

        _startNode.isOpen = true;
    }

    #endregion

    #region 오른쪽 아래

    private void RightDown(Node _startNode)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = false;


            if (MapSizeCheck(x + 1, y - 1) == false) // 탐색 가능 여부
            {
                break;
            }


            _currentNode = GetNode(++x, --y);


            if (_currentNode.isOpen == false)
            {
                break;
            }

            if (GetNode(x - 1, y).isDontMove && GetNode(x, y + 1).isDontMove)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                AddOpenList(_currentNode, _startNode);
                break;
            }


            #region ←벽,↙이동가능

            if (MapSizeCheck(x - 1, y - 1))
            {
                if (GetNode(x - 1, y).isDontMove && GetNode(x - 1, y - 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↑벽,↗이동가능

            if (MapSizeCheck(x + 1, y + 1))
            {
                if (GetNode(x, y + 1).isDontMove && GetNode(x + 1, y + 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            if (Right(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }

            if (Down(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }
        }

        _startNode.isOpen = true;
    }

    #endregion

    #region 왼쪽 위

    private void LeftUp(Node _startNode)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = false;


            if (MapSizeCheck(x - 1, y + 1) == false) // 탐색 가능 여부
            {
                break;
            }


            _currentNode = GetNode(--x, ++y);


            if (_currentNode.isOpen == false)
            {
                break;
            }

            if (GetNode(x + 1, y).isDontMove && GetNode(x, y - 1).isDontMove)
            {
                break;
            }

            if (EndCheck(_currentNode))
            {
                AddOpenList(_currentNode, _startNode);
                break;
            }


            #region →벽,↗이동가능

            if (MapSizeCheck(x + 1, y + 1))
            {
                if (GetNode(x + 1, y).isDontMove && GetNode(x + 1, y + 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↓벽,↙이동가능

            if (MapSizeCheck(x - 1, y - 1))
            {
                if (GetNode(x, y - 1).isDontMove && GetNode(x - 1, y - 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            if (Left(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }

            if (Up(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }
        }

        _startNode.isOpen = true;
    }

    #endregion

    #region 왼쪽 아래

    private void LeftDown(Node _startNode)
    {
        int x = _startNode.gridX;
        int y = _startNode.gridY;


        Node _currentNode = _startNode;

        bool isFind = false;
        while (_currentNode.isOpen == true)
        {
            _currentNode.isOpen = false;


            if (MapSizeCheck(x - 1, y - 1) == false) // 탐색 가능 여부
            {
                break;
            }


            _currentNode = GetNode(--x, --y);


            if (_currentNode.isOpen == false)
            {
                break;
            }

            if (GetNode(x + 1, y).isDontMove && GetNode(x, y + 1).isDontMove)
            {
                break;
            }


            if (EndCheck(_currentNode))
            {
                AddOpenList(_currentNode, _startNode);
                break;
            }


            #region →벽,↘이동가능

            if (MapSizeCheck(x + 1, y - 1))
            {
                if (GetNode(x + 1, y).isDontMove && GetNode(x + 1, y - 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            #region ↑벽,↖이동가능

            if (MapSizeCheck(x - 1, y + 1))
            {
                if (GetNode(x, y + 1).isDontMove && GetNode(x - 1, y + 1).isDontMove == false)
                {
                    AddOpenList(_currentNode, _startNode);

                    break; // 코너 발견하면 바로 종료
                }
            }

            #endregion

            if (Left(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }

            if (Down(_currentNode, true))
            {
                AddOpenList(_currentNode, _startNode);

                break;
            }
        }

        _startNode.isOpen = true;
    }

    #endregion

    private int Heuristic(Vector3Int _currPosition, Vector3Int _endPosition)
    {
        int x = Mathf.Abs(_currPosition.x - _endPosition.x);
        int y = Mathf.Abs(_currPosition.y - _endPosition.y);
        int reming = Mathf.Abs(x - y);

        return _moveDiagonalCost * Mathf.Min(x, y) + _moveStraightCost * reming;
    }

    public bool MapSizeCheck(int _x, int _y)
    {
        return _x >= GameManager.inst.mapMinX && _x <= GameManager.inst.mapMaxX && _y >= GameManager.inst.mapMinY &&
               _y <= GameManager.inst.mapMaxY;
    }

    private void AddOpenList(Node _currentNode, Node _parentNode)
    {
        int nextCost = _parentNode.gCost + Heuristic(_parentNode.Pos(), _currentNode.Pos());
        _currentNode.parentNode = _parentNode;
        _currentNode.gCost = _parentNode.gCost + Heuristic(_parentNode.Pos(), _currentNode.Pos());
        _currentNode.hCost = Heuristic(_currentNode.Pos(), endPos);
        openNodeList.Add(_currentNode);

        // }
    }
}