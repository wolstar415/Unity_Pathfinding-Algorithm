using UnityEngine;

[System.Serializable]
public class Node
{
    public int gridX;
    public int gridY;
    public Node parentNode;

    public int gCost;
    public int hCost;

    public bool openContains; // Astar사용
    public bool closeContains; //Astar사용

    public bool isOpen; //jps 사용
    public bool isDontMove; // jps 사용


    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int _gridX, int _gridY)
    {
        gridX = _gridX;
        gridY = _gridY;
        gCost = int.MaxValue;
    }

    public Vector3Int Pos()
    {
        return (new Vector3Int(gridX, gridY));
    }
}