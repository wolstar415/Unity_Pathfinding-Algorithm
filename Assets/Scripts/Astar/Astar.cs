using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AstarOption
{
    None = 0,
    AllowDiagonal = 1,
    AllowDiagonal_DontCross = 2,
}

[System.Serializable]
public class Astar
{
    public Vector3Int startPos;
    public Vector3Int endPos;
    public AstarOption option;

    public List<Node> finalNodeList = new List<Node>();

    public List<Node> openNodeList = new List<Node>();

    //PriorityQueue< Node, int > openNodeQueue = new PriorityQueue< Node, int >();
    //public PriorityQueue< Node, int > closeNodeQueue = new PriorityQueue< Node, int >();
    public List<Node> closeNodeList = new List<Node>();

    public Node startNode;
    public Node currentNode;
    public Node endNode;

    public Dictionary<Vector2Int, Node> nodeCheck = new Dictionary<Vector2Int, Node>();

    readonly int _moveStraightCost = 10;
    readonly int _moveDiagonalCost = 14;

    public Astar(Vector3Int _startPos, Vector3Int _endPos, AstarOption _option = AstarOption.None)
    {
        if (_startPos == _endPos)
        {
            return;
        }

        startPos = _startPos;
        

        endPos = endCheck(_endPos); //목표지점을 갈 수 있는지 체크
        option = _option;
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost = Heuristic(startPos, endPos);
        nodeCheck.Add(new Vector2Int(startPos.x,startPos.y),startNode);
        endNode = new Node(_endPos.x, _endPos.y);
        nodeCheck.Add(new Vector2Int(_endPos.x,_endPos.y),endNode);
        openNodeList.Add(startNode);
        Find();
    }

    private int Heuristic(Vector3Int _currPosition, Vector3Int _endPosition)
    {
        if (option != AstarOption.None)
        {
            int x = Mathf.Abs(_currPosition.x - _endPosition.x);
            int y = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(x - y);
            return _moveDiagonalCost * Mathf.Min(x, y) + _moveStraightCost * reming;
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
            //currentNode = openNodeQueue.pop();
            currentNode = openNodeList[0];
            for (int i = 0; i < openNodeList.Count; i++)
            {
                if (openNodeList[i].fCost <= currentNode.fCost && openNodeList[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodeList[i];
                }
            }

            closeNodeList.Add(currentNode);
            openNodeList.Remove(currentNode);

            if (currentNode == endNode||Physics2D.OverlapCircle(new Vector2(endPos.x,endPos.y),0.4f,GameManager.inst.dontMoveLayerMask)&&currentNode.hCost<=14)
            {
                Node targetNode = currentNode;
                // if (endNode.parentNode == null)
                // {
                //     Debug.Log("?");
                // }
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

        if (finalNodeList.Count==0)
            //없을경우 가장 가까운 거리로 설정
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

    private void OpenListAdd(int checkX, int checkY, bool cross = false)
    {
        if (!(checkX >= GameManager.inst.mapMinX && checkX <= GameManager.inst.mapMaxX &&
              checkY >= GameManager.inst.mapMinY && checkY <= GameManager.inst.mapMaxY))
        {
            return;
        }

        if (GameManager.inst.nodeArray[checkX - GameManager.inst.mapMinX, checkY - GameManager.inst.mapMinY].isWall)
        {
            return;
        }

        Node checkNode;
        if (nodeCheck.ContainsKey(new Vector2Int(checkX, checkY)))
        {
            checkNode = nodeCheck[new Vector2Int(checkX, checkY)];
        }
        else
        {
            checkNode = new Node(checkX, checkY);
            nodeCheck.Add(new Vector2Int(checkX,checkY),checkNode);
        }
        if (closeNodeList.Contains(checkNode))
        {
            return;
        }

        if (Physics2D.OverlapCircle(new Vector2(checkX, checkY), 0.4f, GameManager.inst.dontMoveLayerMask))
        {
            return;
        }

        if (cross && option != AstarOption.None)
        {
            if (GameManager.inst
                    .nodeArray[currentNode.gridX - GameManager.inst.mapMinX, checkY - GameManager.inst.mapMinY]
                    .isWall && GameManager.inst.nodeArray[checkX - GameManager.inst.mapMinX,
                    currentNode.gridY - GameManager.inst.mapMinY].isWall)
            {
                return;
            }
            
            if (Physics2D.OverlapCircle(new Vector2(currentNode.gridX, checkY), 0.4f,
                    GameManager.inst.dontMoveLayerMask) &&
                Physics2D.OverlapCircle(new Vector2(checkX, currentNode.gridY), 0.4f,
                    GameManager.inst.dontMoveLayerMask))
            {
                return;
            }

            if (option == AstarOption.AllowDiagonal_DontCross)
            {
                if (GameManager.inst
                        .nodeArray[currentNode.gridX - GameManager.inst.mapMinX, checkY - GameManager.inst.mapMinY]
                        .isWall)
                {
                    return;
                }
                
                if (GameManager.inst.nodeArray[checkX - GameManager.inst.mapMinX,
                        currentNode.gridY - GameManager.inst.mapMinY].isWall)
                {
                    return;
                }
                if (Physics2D.OverlapCircle(new Vector2(currentNode.gridX, checkY), 0.4f,
                        GameManager.inst.dontMoveLayerMask))
                {
                    return;
                }

                if (Physics2D.OverlapCircle(new Vector2(checkX, currentNode.gridY), 0.4f,
                        GameManager.inst.dontMoveLayerMask))
                {
                    return;
                }
            }
        }
        
        int moveCost=currentNode.gCost +
                     (cross == true ? 14 : 10);
        if (moveCost < checkNode.gCost || !openNodeList.Contains(checkNode))
        {

            checkNode.gCost = moveCost;
            //checkNode.hCost = (Mathf.Abs(checkNode.gridX - endPos.x) + Mathf.Abs(checkNode.gridY - endPos.y)) * 10;
            checkNode.hCost = Heuristic(new Vector3Int(checkNode.gridX, checkNode.gridY), endPos);
            checkNode.parentNode = currentNode;
            openNodeList.Add(checkNode);
        }
    }
}