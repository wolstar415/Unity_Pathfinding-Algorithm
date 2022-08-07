using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MoveMent : MonoBehaviour
{
    public List<Node> moveNode;
    public Tilemap a;

    public Animator ani;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    private static readonly int Horizontal = Animator.StringToHash("Horizontal");

    private static readonly int Vertical = Animator.StringToHash("Vertical");

    public float moveSpeed=2f;

    public Vector2Int movePos;
    public Coroutine moveCo;

    public AstarOption option;

    public Astar aStar;
    // Start is called before the first frame update

    [ContextMenu("PosSet")]
    void PosSet()
    {
        transform.position = new Vector3(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
    }
    [ContextMenu("MoveStop")]
    void MoveStop()
    {
        if (moveCo != null)
        {
            ani.SetBool(IsMoving,false);
            StopCoroutine(moveCo);
            moveCo = null;
            transform.position=Vector3Int.FloorToInt(transform.position);
        }
    }
    [ContextMenu("MoveRandom")]
    void MoveRandom()
    {
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            transform.position=Vector3Int.FloorToInt(transform.position);
        }

        movePos = new Vector2Int(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
        moveCo = StartCoroutine(Co_Move(movePos));
    }
    [ContextMenu("MovePos")]
    void MovePos()
    {
        moveCo =StartCoroutine(Co_Move(movePos));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PosSet();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveStop();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MoveRandom();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            MovePos();
        }

    }

    IEnumerator Co_Move(Vector2Int endPos)
    {
        aStar = new Astar(Vector3Int.FloorToInt(transform.position),new Vector3Int(endPos.x,endPos.y),option);
        moveNode = aStar.finalNodeList;
        if (moveNode.Count <= 1)
        {
            yield break;
        }

        ani.SetBool(IsMoving,true);
        
        Vector2 dir = new Vector2(moveNode[1].gridX - moveNode[0].gridX, moveNode[1].gridY - moveNode[0].gridY)
            .normalized;
        ani.SetFloat(Horizontal,dir.x);
        ani.SetFloat(Vertical,dir.y);
        do
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(moveNode[1].gridX, moveNode[1].gridY), Time.deltaTime * moveSpeed);
            
            if (Vector3.Distance(transform.position, new Vector3(moveNode[1].gridX, moveNode[1].gridY)) <= 0.1f)
            {
                transform.position = new Vector3(moveNode[1].gridX, moveNode[1].gridY);
                aStar = new Astar(Vector3Int.FloorToInt(transform.position),new Vector3Int(endPos.x,endPos.y),option);
                moveNode = aStar.finalNodeList;
                if (moveNode.Count <= 1)
                {
                    break;
                }
                dir = new Vector2(moveNode[1].gridX - moveNode[0].gridX, moveNode[1].gridY - moveNode[0].gridY)
                    .normalized;
                ani.SetFloat(Horizontal,dir.x);
                ani.SetFloat(Vertical,dir.y);
                
            }

            yield return null;
        } while (moveNode.Count>1);
        ani.SetBool(IsMoving,false);
        moveCo = null;
    }

    void OnDrawGizmos()
    {
        if(moveNode.Count != 0) for (int i = 0; i < moveNode.Count - 1; i++)
            Gizmos.DrawLine(new Vector2(moveNode[i].gridX, moveNode[i].gridY), new Vector2(moveNode[i + 1].gridX, moveNode[i + 1].gridY));
    }

    private void OnDrawGizmosSelected()
    {
        if (new Vector3(movePos.x,movePos.y) != transform.position)
        {
            Gizmos.DrawSphere(new Vector3(movePos.x,movePos.y),0.5f);
        }
    }
}
