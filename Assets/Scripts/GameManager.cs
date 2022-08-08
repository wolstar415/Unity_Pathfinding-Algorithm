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
    }

    public int mapMaxX;
    public int mapMaxY;
    public int mapMinX;
    public int mapMinY;
    public int sizeX;
    public int sizeY;

    public Tilemap tilemap;
}