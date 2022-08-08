using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MoveMent_Jps : MonoBehaviour
{
    public List<Node> moveNode;
    public Animator ani;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    public float moveSpeed = 2f;

    public Vector2Int movePos;
    public Coroutine MoveCo;
    public AstarOption option;
    public Astar aStar;
    public bool bugCheck = false;


    [ContextMenu("PosSet")]
    void PosSet()
    {
        transform.position = new Vector3(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
    }

    [ContextMenu("MoveStop")]
    void MoveStop()
    {
        if (MoveCo != null)
        {
            ani.SetBool(IsMoving, false);
            StopCoroutine(MoveCo);
            MoveCo = null;
            transform.position = Vector3Int.FloorToInt(transform.position);
        }
    }

    [ContextMenu("MoveRandom")]
    void MoveRandom()
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
            transform.position = Vector3Int.FloorToInt(transform.position);
        }

        movePos = new Vector2Int(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
        MovePos();
    }

    [ContextMenu("MovePos")]
    void MovePos()
    {
        MoveCo = StartCoroutine(Co_Move());
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

    IEnumerator Co_Move()
    {
        bugCheck = false;
        aStar = new Astar(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y), option);
        moveNode = aStar.finalNodeList;
        movePos = (Vector2Int)aStar.endPos;
        if (moveNode.Count <= 1)
        {
            MoveCo = null;
            yield break;
        }

        ani.SetBool(IsMoving, true);

        Vector2 dir = new Vector2(moveNode[1].gridX - moveNode[0].gridX, moveNode[1].gridY - moveNode[0].gridY)
            .normalized;
        ani.SetFloat(Horizontal, dir.x);
        ani.SetFloat(Vertical, dir.y);
        yield return null;
        do
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(moveNode[1].gridX, moveNode[1].gridY), Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, new Vector3(moveNode[1].gridX, moveNode[1].gridY)) <= 0.01f)
            {
                transform.position = new Vector3(moveNode[1].gridX, moveNode[1].gridY);
                if (transform.position == new Vector3Int(movePos.x, movePos.y))
                {
                    break;
                }

                aStar = new Astar(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y),
                    option);
                moveNode = aStar.finalNodeList;
                movePos = (Vector2Int)aStar.endPos;
                if (moveNode.Count <= 1)
                {
                    #region  버그방지
                    //목표지점에 못갔는데 딴 플레이어의 이동때문에 이동경로가 막혀서 멈춰있을 때 버그 방지용
                    if (bugCheck == false && transform.position != new Vector3Int(movePos.x, movePos.y))
                    {
                        ani.SetBool(IsMoving, false);
                        yield return new WaitForSeconds(1f);
                        ani.SetBool(IsMoving, true);
                        bugCheck = true;
                        aStar = new Astar(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y),
                            option);
                        moveNode = aStar.finalNodeList;
                        movePos = (Vector2Int)aStar.endPos;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                    

                    #endregion
                    
                }

                dir = new Vector2(moveNode[1].gridX - moveNode[0].gridX, moveNode[1].gridY - moveNode[0].gridY)
                    .normalized;
                ani.SetFloat(Horizontal, dir.x);
                ani.SetFloat(Vertical, dir.y);
            }

            yield return null;
        } while (moveNode.Count > 1);

        ani.SetBool(IsMoving, false);
        MoveCo = null;
        moveNode.Clear();
        aStar = null;
    }

    void OnDrawGizmos()
    {
        if (moveNode.Count != 0)
            for (int i = 0; i < moveNode.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(moveNode[i].gridX, moveNode[i].gridY),
                    new Vector2(moveNode[i + 1].gridX, moveNode[i + 1].gridY));
    }

    private void OnDrawGizmosSelected()
    {
        if (new Vector3(movePos.x, movePos.y) != transform.position)
        {
            Gizmos.DrawSphere(new Vector3(movePos.x, movePos.y), 0.5f);
        }
    }
}