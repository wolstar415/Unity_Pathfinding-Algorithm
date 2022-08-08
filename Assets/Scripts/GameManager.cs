using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public LayerMask dontMoveLayerMask;
    public LayerMask wallLayerMask;
    public Node[,] nodeArray;

    private void Awake()
    {
        inst = this;
        mapMinX = tilemap.cellBounds.xMin;
        mapMinY = tilemap.cellBounds.yMin;
        mapMaxX = tilemap.cellBounds.xMax - 1;
        mapMaxY = tilemap.cellBounds.yMax - 1;
        
        
        sizeX = mapMaxX - mapMinX + 1;
        sizeY = mapMaxY - mapMinY + 1;
        nodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + mapMinX, j + mapMinY), 0.4f))
                    if (col.gameObject.layer == wallLayerMask)
                    {
                        isWall = true;
                    }

                nodeArray[i, j] = new Node( i + mapMinX, j + mapMinY,isWall);
            }
        }
    }

    [field: SerializeField]
    public int mapMaxX { get; private set; }
    [field: SerializeField]
    public int mapMaxY { get; private set; }
    [field: SerializeField]
    public int mapMinX { get; private set; }
    [field: SerializeField]
    public int mapMinY { get; private set; }
    [field: SerializeField]
    public int sizeX { get; private set; }
    [field: SerializeField]
    public int sizeY { get; private set; }

    public Tilemap tilemap;
}