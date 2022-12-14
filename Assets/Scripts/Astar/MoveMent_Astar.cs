using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveMent_Astar : MonoBehaviour
{
    public List<Node> moveNode;
    public Animator ani;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");

    public float moveSpeed = 2f;

    public Vector2Int movePos;
    private Coroutine _moveCo;
    public AstarOption option;
    public Astar aStar;
    public int bugCheck = 0;
    public int[,] addCost;

    [ContextMenu("PosSet")]
    void PosSet()
    {
        transform.position = new Vector3(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
    }

    [ContextMenu("MoveStop")]
    void MoveStop()
    {
        if (_moveCo != null)
        {
            ani.SetBool(IsMoving, false);
            StopCoroutine(_moveCo);
            _moveCo = null;
            transform.position = Vector3Int.FloorToInt(transform.position);
        }
    }

    [ContextMenu("MoveRandom")]
    void MoveRandom()
    {
        if (_moveCo != null)
        {
            StopCoroutine(_moveCo);
            transform.position = Vector3Int.FloorToInt(transform.position);
        }

        movePos = new Vector2Int(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
        MovePos();
    }

    [ContextMenu("MovePos")]
    void MovePos()
    {
        _moveCo = StartCoroutine(Co_Move());
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
        bugCheck = 0;
        addCost = new int[GameManager.inst.sizeX, GameManager.inst.sizeY];
        aStar = new Astar(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y),addCost, option);
        moveNode = aStar.finalNodeList;
        movePos = (Vector2Int)aStar.endPos;
        if (moveNode.Count <= 1)
        {
            _moveCo = null;
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
                addCost[(int)transform.position.x-GameManager.inst.mapMinX, (int)transform.position.y-GameManager.inst.mapMinY] += 10;
                if (transform.position == new Vector3Int(movePos.x, movePos.y))
                {
                    break;
                }

                aStar = new Astar(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y),addCost,
                    option);
                moveNode = aStar.finalNodeList;
                movePos = (Vector2Int)aStar.endPos;
                if (moveNode.Count <= 1)
                {
                    #region ????????????

                    //??????????????? ???????????? ??? ??????????????? ??????????????? ??????????????? ????????? ???????????? ??? ?????? ?????????
                    if (bugCheck < 2 && transform.position != new Vector3Int(movePos.x, movePos.y))
                    {
                        ani.SetBool(IsMoving, false);
                        yield return new WaitForSeconds(1f);
                        ani.SetBool(IsMoving, true);
                        bugCheck++;
                        aStar = new Astar(Vector3Int.FloorToInt(transform.position),
                            new Vector3Int(movePos.x, movePos.y),addCost,
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
        _moveCo = null;
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