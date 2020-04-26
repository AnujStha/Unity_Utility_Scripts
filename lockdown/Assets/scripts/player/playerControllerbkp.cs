using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControllerBkp : MonoBehaviour
{
    public float speed, jumpVelocity, maxVerticalVelocity, dashTime, dashSpeed, dashInterval, wallJumpSideForce;
    [Range(0, 1)]
    public float wallGrabSpeedResistance;
    public float maxJumpSteps;
    public int airJumps;
    private int airJumpsRemaining;
    public Vector2 groundCheckBoxCastSize, wallCheckBoxCastSizeRight, wallCheckBoxCastSizeLeft;
    public Transform groundCheckPoint, wallCheckRight, wallCheckLeft;
    public LayerMask platformLayer;
    private float jumpedSteps;
    private float lr;
    private bool jumpBtn, jump, jumpReady, dashing, dashTrig, dashAllow, prevFGrounded, wallGrab;
    public bool facingRight;
    private Rigidbody2D playerRb;

    public UnityEvent jumpStartEvent, LandEvent, dashStartEvent, dashEndEvent, AirJumpStartEvent, rightWallGrabEvent, leftWallGrabEvent, WallReleaseEvent;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        jump = false;
        airJumpsRemaining = 0;
        dashAllow = true;
        if (jumpStartEvent == null) jumpStartEvent = new UnityEvent();
        if (LandEvent == null) LandEvent = new UnityEvent();
        if (dashStartEvent == null) dashStartEvent = new UnityEvent();
        if (dashEndEvent == null) dashEndEvent = new UnityEvent();
        if (AirJumpStartEvent == null) AirJumpStartEvent = new UnityEvent();
        if (rightWallGrabEvent == null) rightWallGrabEvent = new UnityEvent();
        if (leftWallGrabEvent == null) leftWallGrabEvent = new UnityEvent();
        if (WallReleaseEvent == null) WallReleaseEvent = new UnityEvent();
    }
    void Update()
    {
        GetControls();
    }
    private void FixedUpdate()
    {

        if (GroundedCheck())
        {
            if (!prevFGrounded)
            {
                LandEvent.Invoke();
            }
            prevFGrounded = true;
            jumpReady = true;
            jumpedSteps = 0;
            airJumpsRemaining = airJumps;
        }
        else
        {
            //wallGrab mech
            wallSlide();
            prevFGrounded = false;
        }
        handleYMovement();
        //dash
        if (dashTrig)
        {
            if (dashAllow)
            {
                StartCoroutine(DashStart());
            }
            dashTrig = false;
        }
        if (dashing)
        {
            Dash();
        }
        else
        {
            Move(lr * speed);
        }
        //for terminal velocity
        if (-playerRb.velocity.y > maxVerticalVelocity)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -maxVerticalVelocity);
        }
    }
    void wallSlide()
    {
        int walled = WallCheck();
        if (walled != 0)
        {
            if (wallGrab == false)
            {
                wallGrab = true;
                if (walled == 1)
                {
                    rightWallGrabEvent.Invoke();
                }
                else
                {
                    leftWallGrabEvent.Invoke();
                }
            }
            jumpReady = true;
            jumpedSteps = 0;
            airJumpsRemaining = airJumps;
        }
        else
        {
            if (wallGrab == true)
            {
                wallGrab = false;
                WallReleaseEvent.Invoke();
            }
        }
    }
    void Dash()
    {
        playerRb.velocity = new Vector2(dashSpeed * (facingRight ? 1 : -1), 0);
    }
    int WallCheck()
    {
        //1-right, -1-left, 0 none
        RaycastHit2D hit = Physics2D.BoxCast(wallCheckRight.position, wallCheckBoxCastSizeRight, 0f, Vector2.right, 0, platformLayer);
        if (hit.collider != null && hit.collider.CompareTag("platformClimb"))
        {
            return 1;
        }
        hit = Physics2D.BoxCast(wallCheckLeft.position, wallCheckBoxCastSizeLeft, 0f, Vector2.right, 0, platformLayer);
        if (hit.collider != null && hit.collider.CompareTag("platformClimb"))
        {
            return -1;
        }
        return 0;
    }
    void GetControls()
    {
        lr = Input.GetAxis("Horizontal");
        jumpBtn = Input.GetButton("Jump");
        if (Input.GetButtonDown("Dash"))
        {
            //don't make false
            dashTrig = true;
        }
    }
    void handleYMovement()
    {

        if (jumpBtn)
        {
            if (maxJumpSteps > jumpedSteps)
            {
                jumpStartEvent.Invoke();
                jump = true;
                jumpedSteps += Time.fixedDeltaTime;
                jumpReady = false;
            }

            else if (jumpReady && airJumpsRemaining > 0)
            {
                AirJumpStartEvent.Invoke();
                airJumpsRemaining--;
                jumpedSteps = 0;
                jump = true;
                jumpReady = false;
            }
            else
            {
                slowStop();
                jumpReady = false;
            }
        }
        else if (playerRb.velocity.y > 0)
        {
            jumpReady = true;
            fastStop();
            jumpedSteps = maxJumpSteps;
        }
        else
        {
            jumpReady = true;
        }
        if (wallGrab)
        {
            jumpReady = true;
        }
        playerRb.velocity = new Vector2(playerRb.velocity.x, jump ? (jumpVelocity) : (wallGrab ? (1 - wallGrabSpeedResistance) * playerRb.velocity.y : playerRb.velocity.y));
    }
    void slowStop()
    {
        jump = false;
    }
    void fastStop()
    {
        jumpedSteps = maxJumpSteps;
        jump = false;
        playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
    }
    void Move(float dir)
    {
        playerRb.velocity = new Vector2(dir, playerRb.velocity.y);
        if (dir < 0 && facingRight || dir > 0 && !facingRight)
        {
            flip();
        }
    }
    void flip()
    {
        facingRight = !facingRight;
    }
    IEnumerator dashCounter()
    {
        yield return new WaitForSeconds(dashInterval);
        dashAllow = true;
    }
    IEnumerator DashStart()
    {
        dashStartEvent.Invoke();
        dashAllow = false;
        StartCoroutine(dashCounter());
        dashing = true;
        yield return new WaitForSeconds(dashTime);
        dashing = false;
        dashEndEvent.Invoke();
    }
    bool GroundedCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(groundCheckPoint.position, groundCheckBoxCastSize, 0f, Vector2.down, 0, platformLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckBoxCastSize);
        Gizmos.DrawWireCube(wallCheckRight.position, wallCheckBoxCastSizeRight);
        Gizmos.DrawWireCube(wallCheckLeft.position, wallCheckBoxCastSizeLeft);
    }
}
