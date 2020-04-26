using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bounceMovement : MonoBehaviour
{
    public float speed;
    private Vector2 direction;
    public Transform upPoint, downPoint, leftPoint, rightPoint;
    public Vector2 upSize, downSize, leftSize, rightSize;
    private void Start()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
    private void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (direction.x > 0)
        {
            Collider2D hit= Physics2D.OverlapBox(rightPoint.position, rightSize, 0);
            if (hit != null)
            {
                direction.x = -direction.x;
            }
        }
        else {
            Collider2D hit= Physics2D.OverlapBox(leftPoint.position, leftSize, 0);
            if (hit != null)
            {
                direction.x = -direction.x;
            }
        }
        if (direction.y > 0)
        {
            Collider2D hit= Physics2D.OverlapBox(upPoint.position, upSize, 0);
            if (hit != null)
            {
                direction.y = -direction.y;
            }
        }
        else {
           Collider2D hit= Physics2D.OverlapBox(downPoint.position, downSize, 0);
            if (hit != null)
            {
                direction.y = -direction.y;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(upPoint.position, upSize);
        Gizmos.DrawWireCube(downPoint.position, downSize);
        Gizmos.DrawWireCube(leftPoint.position, leftSize);
        Gizmos.DrawWireCube(rightPoint.position, rightSize);
    }
}
