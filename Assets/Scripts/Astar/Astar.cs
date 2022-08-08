using System.Collections.Generic;
using UnityEngine;

public enum AstarOption
{
    None = 0,
    AllowDiagonal = 1,
    AllowDiagonalDontCross = 2,
}

[System.Serializable]
public class Astar
{
    public Vector3Int startPos;
    public Vector3Int endPos;
    public AstarOption option;

    public List<Node> finalNodeList = new List<Node>();

    public List<Node> openNodeList = new List<Node>();


    public List<Node> closeNodeList = new List<Node>();

    public Node startNode;
    public Node currentNode;
    public Node endNode;

    public Node[,] nodeArray;

    readonly int _moveStraightCost = 10;
    readonly int _moveDiagonalCost = 14;
    public int[,] addCost;

    public Astar(Vector3Int _startPos, Vector3Int _endPos,int[,] _addCost, AstarOption _option = AstarOption.None)
    {
        if (_startPos == _endPos)
        {
            return;
        }

        addCost = _addCost;
        nodeArray = new Node[GameManager.inst.sizeX, GameManager.inst.sizeY];

        startPos = _startPos;


        endPos = endCheck(_endPos); //목표지점을 갈 수 있는지 체크
        option = _option;
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost = Heuristic(startPos, endPos);
        nodeArray[_startPos.x - GameManager.inst.mapMinX, _startPos.y - GameManager.inst.mapMinY] =
            new Node(_startPos.x, _startPos.y);
        openNodeList.Add(startNode);
        startNode.openContains = true;
        Find();
    }

    private int Heuristic(Vector3Int _currPosition, Vector3Int _endPosition)
    {
        if (option != AstarOption.None)
        {
            int x = Mathf.Abs(_currPosition.x - _endPosition.x);
            int y = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(x - y);
            return addCost[_currPosition.x-GameManager.inst.mapMinX,_currPosition.y-GameManager.inst.mapMinY]+_moveDiagonalCost * Mathf.Min(x, y) + _moveStraightCost * reming;
        }

        return (Mathf.Abs(_currPosition.x - _endPosition.x) + Mathf.Abs(_currPosition.y - _endPosition.y)) * 10;
    }

    #region 목표지점을 갈 수 있는지 체크

    private Vector3Int endCheck(Vector3Int check)
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


    private void Find()
    {
        while (openNodeList.Count > 0)
        {
            currentNode = openNodeList[0];
            for (int i = 0; i < openNodeList.Count; i++)
            {
                if (openNodeList[i].fCost <= currentNode.fCost && openNodeList[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodeList[i];
                }
            }

            closeNodeList.Add(currentNode);
            currentNode.closeContains = true;
            openNodeList.Remove(currentNode);
            currentNode.openContains = false;

            if (currentNode.Pos() == endPos ||
                Physics2D.OverlapCircle(new Vector2(endPos.x, endPos.y), 0.4f, GameManager.inst.dontMoveLayerMask) &&
                currentNode.hCost <= 14)
            {
                endNode = currentNode;
                Node targetNode = currentNode;
                while (targetNode != startNode)
                {
                    finalNodeList.Add(targetNode);
                    targetNode = targetNode.parentNode;
                }

                finalNodeList.Add(startNode);
                finalNodeList.Reverse();
                break;
            }


            OpenListAdd(currentNode.gridX, currentNode.gridY + 1); //↑
            OpenListAdd(currentNode.gridX, currentNode.gridY - 1); //↓
            OpenListAdd(currentNode.gridX + 1, currentNode.gridY); //→
            OpenListAdd(currentNode.gridX - 1, currentNode.gridY); //←
            if (option != AstarOption.None)
            {
                OpenListAdd(currentNode.gridX + 1, currentNode.gridY + 1, true); //↗
                OpenListAdd(currentNode.gridX - 1, currentNode.gridY + 1, true); //↖
                OpenListAdd(currentNode.gridX - 1, currentNode.gridY - 1, true); //↙
                OpenListAdd(currentNode.gridX + 1, currentNode.gridY - 1, true); //↘
            }
        }

        if (finalNodeList.Count == 0)
        {
            Node checkNode = closeNodeList[0];
            for (int i = 0; i < closeNodeList.Count; i++)
            {
                if (closeNodeList[i].hCost < checkNode.hCost)
                {
                    checkNode = closeNodeList[i];
                }
            }

            endNode = checkNode;
            while (checkNode != startNode)
            {
                finalNodeList.Add(checkNode);
                checkNode = checkNode.parentNode;
            }

            finalNodeList.Add(startNode);
            finalNodeList.Reverse();
        }
    }

    public bool MapSizeCheck(int _x, int _y)
    {
        return _x >= GameManager.inst.mapMinX && _x <= GameManager.inst.mapMaxX && _y >= GameManager.inst.mapMinY &&
               _y <= GameManager.inst.mapMaxY;
    }


    public Node GetNode(int _x, int _y)
    {
        int x = _x - GameManager.inst.mapMinX;
        int y = _y - GameManager.inst.mapMinY;
        if (nodeArray[x, y] != null)
        {
            return nodeArray[x, y];
        }

        nodeArray[x, y] = new Node(_x, _y);
        bool isDontMove = false;
        if (MapSizeCheck(_x, _y))
        {
            if (Physics2D.OverlapCircle(new Vector2(_x, _y), 0.4f, GameManager.inst.dontMoveLayerMask))
            {
                isDontMove = true;
            }
        }
        else
        {
            isDontMove = true;
        }

        nodeArray[x, y].isDontMove = isDontMove;


        return nodeArray[x, y];
    }

    private void OpenListAdd(int checkX, int checkY, bool cross = false)
    {
        if (!(MapSizeCheck(checkX, checkY)))
        {
            return;
        }

        Node checkNode = GetNode(checkX, checkY);
        if (checkNode.isDontMove)
        {
            return;
        }

        if (checkNode.closeContains)
        {
            return;
        }

        if (cross && option != AstarOption.None)
        {
            if (GetNode(currentNode.gridX, checkY).isDontMove && GetNode(checkX, currentNode.gridY).isDontMove)
            {
                return;
            }

            if (option == AstarOption.AllowDiagonalDontCross)
            {
                if (GetNode(currentNode.gridX, checkY).isDontMove && GetNode(checkX, currentNode.gridY).isDontMove)
                {
                    return;
                }

                if (GetNode(checkX, currentNode.gridY).isDontMove)
                {
                    return;
                }
            }
        }

        int moveCost = currentNode.gCost +
                       (cross == true ? 14 : 10);
        if (moveCost < checkNode.gCost || !checkNode.openContains)
        {
            checkNode.gCost = moveCost;
            checkNode.hCost = Heuristic(new Vector3Int(checkNode.gridX, checkNode.gridY), endPos);
            checkNode.parentNode = currentNode;
            openNodeList.Add(checkNode);
            checkNode.openContains = true;
        }
    }
}