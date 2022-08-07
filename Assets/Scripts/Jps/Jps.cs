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
    
    public Dictionary<Vector2Int, Node> nodeCheck = new Dictionary<Vector2Int, Node>();

    public Jps(Vector3Int _startPos, Vector3Int _endPos)
    {
        startPos = _startPos;
        endPos = _endPos;
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost = (Mathf.Abs(startNode.gridX - endPos.x) + Mathf.Abs(startNode.gridY - endPos.y)) * 10;
        nodeCheck.Add(new Vector2Int(startPos.x, startPos.y), startNode);

        openNodeList.Add(startNode);

        //Find();
    }
    
    // private void Find()
    // {
    //     while (openNodeList.Count > 0)
    //     {
    //         currentNode = openNodeList[0];
    //         for (int i = 0; i < openNodeList.Count; i++)
    //         {
    //             if (openNodeList[i].fCost <= currentNode.fCost && openNodeList[i].hCost < currentNode.hCost)
    //             {
    //                 currentNode = openNodeList[i];
    //             }
    //         }
    //
    //         openNodeList.Remove(currentNode);
    //         closeNodeList.Add(currentNode);
    //
    //         if (currentNode.gridX == endPos.x && currentNode.gridY == endPos.y)
    //         {
    //             endNode = currentNode;
    //             Node targetNode = endNode;
    //             while (targetNode != startNode)
    //             {
    //                 finalNodeList.Add(targetNode);
    //                 targetNode = targetNode.parentNode;
    //             }
    //
    //             finalNodeList.Add(startNode);
    //             finalNodeList.Reverse();
    //             if (finalNodeList.Count > 1)
    //             {
    //                 nextNode = finalNodeList[1];
    //             }
    //
    //             break;
    //         }
    //
    //
    //         OpenListAdd(currentNode.gridX, currentNode.gridY + 1);//↑
    //         OpenListAdd(currentNode.gridX, currentNode.gridY - 1);//↓
    //         OpenListAdd(currentNode.gridX + 1, currentNode.gridY);//→
    //         OpenListAdd(currentNode.gridX - 1, currentNode.gridY);//←
    //         if (option != AstarOption.None)
    //         {
    //             OpenListAdd(currentNode.gridX + 1, currentNode.gridY + 1, true);//↗
    //             OpenListAdd(currentNode.gridX - 1, currentNode.gridY + 1, true);//↖
    //             OpenListAdd(currentNode.gridX - 1, currentNode.gridY - 1, true);//↙
    //             OpenListAdd(currentNode.gridX + 1, currentNode.gridY - 1, true);//↘
    //         }
    //     }
    //
    //     if (endNode == null)
    //     {
    //         Node checkNode = closeNodeList[0];
    //         for (int i = 0; i < closeNodeList.Count; i++)
    //         {
    //             if (closeNodeList[i].hCost < checkNode.hCost)
    //             {
    //                 checkNode = closeNodeList[i];
    //             }
    //         }
    //
    //
    //         endNode = checkNode;
    //         while (checkNode != startNode)
    //         {
    //             finalNodeList.Add(checkNode);
    //             checkNode = checkNode.parentNode;
    //         }
    //
    //         finalNodeList.Add(startNode);
    //         finalNodeList.Reverse();
    //     }
    // }
}
