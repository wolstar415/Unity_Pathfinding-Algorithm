using System.Collections;
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
    
    public Dictionary<Vector2Int, Node> nodeCheck = new Dictionary<Vector2Int, Node>();

    public Jps(Vector3Int _startPos, Vector3Int _endPos)
    {
        startPos = _startPos;
        endPos = _endPos;
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost = Heuristic(startPos,endPos);
        nodeCheck.Add(new Vector2Int(startPos.x, startPos.y), startNode);

        openNodeList.Add(startNode);

        Find();
    }
    
    private void Find()
    {
        while (openNodeList.Count > 0)
        {

            if (currentNode.Pos() == endPos)
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

            //Right(currentNode, new Vector2Int(1, 0));


        }
    
        
    }
    
    // private bool Right(Node _startNode)
    //     {
    //         int x = _currentNode.gridX+direction.x;
    //         int y = _currentNode.gridY;
    //
    //         Node checkNode = _currentNode;
    //
    //         bool isFind = false;
    //         while (currentNode.moveable == true)
    //         {
    //
    //             #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
    //
    //             if (MapSizeCheck(x+direction.x, y+direction.y) == false)   // 탐색 가능 여부
    //             {
    //                 break;
    //             }
    //             if(Physics2D.OverlapCircle(checkNode))
    //             checkNode = GetGrid(++x, y); // 다음 오른쪽 노드로 이동
    //
    //             if (currentNode.moveable == false)
    //             {
    //                 break;
    //             }
    //
    //             if (currentNode == this.endNode)
    //             {
    //                 isFind = true;
    //                 if (isMovealbe == false)
    //                 {
    //                     AddOpenList(currentNode, startNode);
    //                 }
    //                 break;
    //             }
    //
    //             currentNode.tile.SetColor(Color.gray);
    //
    //             #endregion
    //
    //             #region 위쪽이 막혀 있으면서 오른쪽 위는 뚫려있는 경우
    //
    //             if (y + 1 < mapSize.y && x + 1 < mapSize.x)
    //             {
    //                 if (GetGrid(x, y + 1).moveable == false) // 위쪽이 벽이면
    //                 {
    //                     if (GetGrid(x + 1, y + 1).moveable == true) // 오른쪽 위가 막혀있지 않으면
    //                     {
    //                         if (isMovealbe == false)
    //                         {
    //                             AddOpenList(currentNode, startNode);
    //                         }
    //                         isFind = true;
    //                         break; // 코너 발견하면 바로 종료
    //                     }
    //                 }
    //             }
    //
    //             #endregion
    //
    //             #region 위쪽이 막혀 있으면서 오른쪽 아래는 뚫려있는 경우
    //
    //             if (y > 0 && x + 1 < mapSize.x)
    //             {
    //                 if (GetGrid(x, y - 1).moveable == false) // 아래쪽이 벽이고
    //                 {
    //                     if (GetGrid(x + 1, y - 1).moveable == true) // 오른쪽 아래가 막혀 있지 않으면
    //                     {
    //                         if (isMovealbe == false)
    //                         {
    //                             AddOpenList(currentNode, startNode);
    //                         }
    //                         isFind = true;
    //                         break; // 코너 발견하면 바로 종료
    //                     }
    //                 }
    //             }
    //
    //             #endregion
    //         }
    //
    //         startNode.moveable = true;
    //         return isFind;
    //     }
    
    private int Heuristic(Vector3Int _currPosition, Vector3Int _endPosition)
    {
        int x = Mathf.Abs(_currPosition.x - _endPosition.x);
        int y = Mathf.Abs(_currPosition.y - _endPosition.y);
        int reming = Mathf.Abs(x - y);

        return _moveDiagonalCost * Mathf.Min(x, y) + _moveStraightCost * reming;
    }

    public bool MapSizeCheck(int _x, int _y)
    {
        return _x >= GameManager.inst.mapMinX && _x <= GameManager.inst.mapMaxX  && _y >= GameManager.inst.mapMinY &&
               _y <= GameManager.inst.mapMaxY;
    }
}
