using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    public float speed = 1f;
    public Vector2Int dirPos;
    private Vector2Int startPos;

    private Vector2Int movePos;
    private bool b = false;

    void Start()
    {
        startPos = Vector2Int.FloorToInt(transform.position);
        movePos = startPos + dirPos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos, speed * Time.deltaTime);

        if (Vector2.Distance(movePos, transform.position) <= 0.1f)
        {
            if (b == false)
            {
                movePos = startPos;
            }
            else
            {
                movePos = startPos + dirPos;
            }

            b = !b;
        }
    }
}