using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
    public Jps jps;
    public int bugCheck;


    [ContextMenu("PosSet")]
    void PosSet()
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
            MoveCo = null;
        }

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
    async void MoveRandom()
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
            MoveCo = null;
        }

        await Task.Yield();
        transform.position = Vector3Int.FloorToInt(transform.position);
        movePos = new Vector2Int(Random.Range(GameManager.inst.mapMinX, GameManager.inst.mapMaxX),
            Random.Range(GameManager.inst.mapMinY, GameManager.inst.mapMaxY));
        MovePos();
    }

    [ContextMenu("MovePos")]
    async void MovePos()
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
            MoveCo = null;
        }

        await Task.Yield();
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
        bugCheck = 0;
        jps = new Jps(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y));
        moveNode = jps.finalNodeList;
        movePos = (Vector2Int)jps.endPos;

        Vector3 targetPos;
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
        targetPos = transform.position + Vector3Int.RoundToInt(dir);
        yield return null;
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
            {
                transform.position = targetPos;
                if (transform.position == new Vector3Int(movePos.x, movePos.y))
                {
                    break;
                }

                jps = new Jps(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y));
                moveNode = jps.finalNodeList;
                movePos = (Vector2Int)jps.endPos;
                if (moveNode.Count <= 1)
                {
                    #region 버그방지

                    //목표지점에 못갔는데 딴 플레이어의 이동때문에 이동경로가 막혀서 멈춰있을 때 버그 방지용
                    if (bugCheck < 2 && transform.position != new Vector3Int(movePos.x, movePos.y))
                    {
                        ani.SetBool(IsMoving, false);
                        yield return new WaitForSeconds(1f);
                        ani.SetBool(IsMoving, true);
                        bugCheck++;
                        jps = new Jps(Vector3Int.FloorToInt(transform.position), new Vector3Int(movePos.x, movePos.y));
                        moveNode = jps.finalNodeList;
                        movePos = (Vector2Int)jps.endPos;
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
                targetPos = transform.position + Vector3Int.RoundToInt(dir);
            }

            yield return null;
        } while (moveNode.Count > 1);

        ani.SetBool(IsMoving, false);
        MoveCo = null;
        moveNode.Clear();
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