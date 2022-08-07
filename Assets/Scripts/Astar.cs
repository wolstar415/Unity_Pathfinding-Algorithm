using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AstarOption
{
    None=0,
    AllowDiagonal=1,
    AllowDiagonal_DontCross=2,
    
}
[System.Serializable]
public class Astar
{
    public Vector3Int startPos;
    public Vector3Int endPos;
    public AstarOption option;

    public List<Node> finalNodeList=new List<Node>();

    public List<Node> openNodeList=new List<Node>();
    public List<Node> closeNodeList=new List<Node>();

    public Node startNode;
    public Node currentNode;
    public Node endNode;
    public Node nextNode;

    public Dictionary<Vector2Int, Node> nodeCheck = new Dictionary<Vector2Int, Node>();
    

    
    
    public Astar(Vector3Int _startPos, Vector3Int _endPos, AstarOption _option=AstarOption.None)
    {

        startPos = _startPos;
        endPos = _endPos;
        option = _option;
        startNode = new Node(_startPos.x, _startPos.y);
        startNode.hCost=(Mathf.Abs(startNode.gridX - endPos.x) + Mathf.Abs(startNode.gridY - endPos.y)) * 10;
        nodeCheck.Add(new Vector2Int(startPos.x,startPos.y),startNode);
        
        openNodeList.Add(startNode);
        
        Find();
    }


    private void Find()
    {
        while (openNodeList.Count >0)
        {
            currentNode=openNodeList[0];
            for (int i = 0; i < openNodeList.Count; i++)
            {
                if (openNodeList[i].fCost <= currentNode.fCost && openNodeList[i].hCost < currentNode.hCost)
                {
                    currentNode = openNodeList[i];
                }
            }

            openNodeList.Remove(currentNode);
            closeNodeList.Add(currentNode);

            if (currentNode.gridX == endPos.x && currentNode.gridY == endPos.y)
            {

                
                endNode = currentNode;
                Node targetNode = endNode;
                while (targetNode!=startNode)
                {
                    finalNodeList.Add(targetNode);
                    targetNode = targetNode.parentNode;
                }
                finalNodeList.Add(startNode);
                finalNodeList.Reverse();
                if (finalNodeList.Count > 1)
                {
                    nextNode = finalNodeList[1];
                }
                break;
            }

            if (option != AstarOption.None)
            {
                OpenListAdd(currentNode.gridX + 1, currentNode.gridY + 1,true);
                OpenListAdd(currentNode.gridX - 1, currentNode.gridY + 1,true);
                OpenListAdd(currentNode.gridX - 1, currentNode.gridY - 1,true);
                OpenListAdd(currentNode.gridX + 1, currentNode.gridY - 1,true);
            }
            
            OpenListAdd(currentNode.gridX, currentNode.gridY + 1);
            OpenListAdd(currentNode.gridX + 1, currentNode.gridY);
            OpenListAdd(currentNode.gridX, currentNode.gridY - 1);
            OpenListAdd(currentNode.gridX - 1, currentNode.gridY);
            


        }

        if (endNode == null)
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
            while (checkNode!=startNode)
            {
                finalNodeList.Add(checkNode);
                checkNode = checkNode.parentNode;
            }
            finalNodeList.Add(startNode);
            finalNodeList.Reverse();
        }
        
        
        
        
        
        
    }

    private void OpenListAdd(int checkX, int checkY,bool cross=false)
    {
        if (!(checkX >= GameManager.inst.mapMinX && checkX <= GameManager.inst.mapMaxX &&
            checkY >= GameManager.inst.mapMinY && checkY <= GameManager.inst.mapMaxY))
        {
            return;
        }

        if (nodeCheck.ContainsKey(new Vector2Int(checkX,checkY)))
        {
            return;
        }
        

        
        if (Physics2D.OverlapCircle(new Vector2(checkX, checkY), 0.4f, GameManager.inst.dontMoveLayerMask))
        {
            return;
        }
        
        if (cross&&option!=AstarOption.None)
        {
            if (Physics2D.OverlapCircle(new Vector2(currentNode.gridX, checkY), 0.4f, GameManager.inst.dontMoveLayerMask)&&Physics2D.OverlapCircle(new Vector2(checkX, currentNode.gridY), 0.2f, GameManager.inst.dontMoveLayerMask))
            {
                return;
            }
            if (option==AstarOption.AllowDiagonal_DontCross)
            {
                if (Physics2D.OverlapCircle(new Vector2(currentNode.gridX, checkY), 0.4f, GameManager.inst.dontMoveLayerMask))
                {
                    return;
                }
                if (Physics2D.OverlapCircle(new Vector2(checkX, currentNode.gridY), 0.4f, GameManager.inst.dontMoveLayerMask))
                {
                    return;
                }
            }
            
        }
        
        Node checkNode = new Node(checkX, checkY);
        checkNode.gCost = currentNode.gCost + (currentNode.gridX - checkX == 0 || currentNode.gridY - checkY == 0 ? 10 : 14);
        checkNode.hCost=(Mathf.Abs(checkNode.gridX - endPos.x) + Mathf.Abs(checkNode.gridY - endPos.y)) * 10;
        checkNode.parentNode = currentNode;
        openNodeList.Add(checkNode);
        nodeCheck.Add(new Vector2Int(checkX,checkY),checkNode);

    }
}
