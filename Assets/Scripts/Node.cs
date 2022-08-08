using UnityEngine;

[System.Serializable]
public class Node
{
    public int gridX;
    public int gridY;
    public Node parentNode;

    public int gCost;
    public int hCost;
    public bool isWall;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int _gridX, int _gridY,bool _isWall=false)
    {
        gridX = _gridX;
        gridY = _gridY;
        isWall = _isWall;
    }

    public Vector3Int Pos()
    {
        return (new Vector3Int(gridX, gridY));
    }
}