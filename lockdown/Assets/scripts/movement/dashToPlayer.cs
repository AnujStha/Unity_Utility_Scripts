using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class dashToPlayer : MonoBehaviour
{
    public float normalMovementSpeed, dashMovementSpeed,normalMovementInterval,normalMovementProbability,recoveryPeriod;
    public Transform collideCheckPointRight, collideCheckPointLeft;
    public LayerMask dashCollideWith;
    public Vector2 dashArea,collideCheckPointRightSize, collideCheckPointLeftSize;
    private bool dashRight,dashing,isRecoveringFromDashCollide;
    private int normalMovementDirection;
    private float movementHorizontalVelocity;
    public UnityEvent dashStartEvent, dashStopEvent, recoveryEndEvent;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (dashStartEvent == null) dashStartEvent = new UnityEvent();
        if (dashStopEvent == null) dashStartEvent = new UnityEvent();
        if (recoveryEndEvent == null) recoveryEndEvent = new UnityEvent();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(movementCounter());
    }
    void FixedUpdate()
    {
        if (isRecoveringFromDashCollide)
        {
            movementHorizontalVelocity = 0;
        }
        else
        {
            if (dashing)
            {
                movementHorizontalVelocity = dashRight ? dashMovementSpeed : -dashMovementSpeed;
            }
            else
            {
                movementHorizontalVelocity = normalMovementSpeed * normalMovementDirection;
                if (Mathf.Abs(transform.position.x - gameData.player.transform.position.x) < dashArea.x / 2)
                {
                    if (Mathf.Abs(transform.position.y - gameData.player.transform.position.y) < dashArea.y / 2)
                    {
                        dash(gameData.player.transform.position.x > transform.position.x);
                    }
                }
            }
        }
        rb.velocity = new Vector2(movementHorizontalVelocity,rb.velocity.y);
    }
    void dashStop() {
        dashing = false;
        movementHorizontalVelocity = 0;
        StartCoroutine(recovery());
        dashStopEvent.Invoke();
    }
    IEnumerator recovery() {
        isRecoveringFromDashCollide = true;
        yield return new WaitForSeconds(recoveryPeriod);
        isRecoveringFromDashCollide = false;
        recoveryEndEvent.Invoke();
    }
    IEnumerator movementCounter() {
        while (true){
            yield return new WaitForSeconds(normalMovementInterval);
            if (Random.Range(0f, 1f) < normalMovementProbability)
            {
                normalMovementDirection= Random.Range(0, 2) == 0 ? 1 : -1;
            }
            else {
                normalMovementDirection = 0;
            }
        }
    }
    void dash(bool _dashRight) {
        dashRight = _dashRight;
        dashing = true;
        movementHorizontalVelocity = dashRight ? dashMovementSpeed : -dashMovementSpeed;
        dashStartEvent.Invoke();
        collideCheck();
    }
    void collideCheck() {
        if (dashRight)
        {
            if (Physics2D.OverlapBox(collideCheckPointRight.position, collideCheckPointRightSize, 0, dashCollideWith) != null)
            {
                dashStop();
            }
        }
        else
        {
            if (Physics2D.OverlapBox(collideCheckPointLeft.position, collideCheckPointLeftSize, 0, dashCollideWith) != null)
            {
                dashStop();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing)
        {
            collideCheck();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, dashArea);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collideCheckPointRight.position, collideCheckPointRightSize);
        Gizmos.DrawWireCube(collideCheckPointLeft.position, collideCheckPointLeftSize);
    }
}