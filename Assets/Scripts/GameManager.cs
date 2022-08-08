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

    private void Awake()
    {
        inst = this;
        mapMinX = tilemap.cellBounds.xMin;
        mapMinY = tilemap.cellBounds.yMin;
        mapMaxX = tilemap.cellBounds.xMax - 1;
        mapMaxY = tilemap.cellBounds.yMax - 1;
    }

    [field: SerializeField]
    public int mapMaxX { get; private set; }
    [field: SerializeField]
    public int mapMaxY { get; private set; }
    [field: SerializeField]
    public int mapMinX { get; private set; }
    [field: SerializeField]
    public int mapMinY { get; private set; }

    public Tilemap tilemap;
}