using UnityEngine;

[System.Serializable]
public class Node
{
    public int gridX;
    public int gridY;
    public Node parentNode;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int _gridX, int _gridY)
    {
        gridX = _gridX;
        gridY = _gridY;
    }
}