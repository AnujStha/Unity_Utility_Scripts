using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public bool allowWallJump, allowDash;
    private bool jumpBtn, jumpBtnDown, grounded, dashBtnDown, dashReady, dashing, wallGrab = false;
    public Transform groundCheckPoint, wallCheckRight, wallCheckLeft;
    public Vector2 groundCheckBoxCastSize, wallCheckBoxCastSizeRight, wallCheckBoxCastSizeLeft;
    public LayerMask platformLayer;
    public bool facingRight;
    private float lr, jumpedSteps;
    private Rigidbody2D playerRb;
    public float horizontalSpeed, jumpVelocity, maxVerticalVelocity, dashTime, dashSpeed, dashInterval, wallJumpSideForce, wallJumpHorizontalForce, externalForceDecayFactorPerSec;
    public Vector2 velocity;
    private Vector2 externalForce;
    public float maxJumpSteps;
    public int airJumps;
    private int airJumpsRemaining;
    public UnityEvent groundJumpStartEvent, LandEvent, dashStartEvent, dashEndEvent, AirJumpStartEvent, rightWallGrabEvent, leftWallGrabEvent, WallReleaseEvent, wallJumpEvent;
    private void Awake()
    {
        velocity = new Vector2(0, 0);
        playerRb = GetComponent<Rigidbody2D>();
        dashReady = true;

        if (groundJumpStartEvent == null) groundJumpStartEvent = new UnityEvent();
        if (LandEvent == null) LandEvent = new UnityEvent();
        if (dashStartEvent == null) dashStartEvent = new UnityEvent();
        if (dashEndEvent == null) dashEndEvent = new UnityEvent();
        if (AirJumpStartEvent == null) AirJumpStartEvent = new UnityEvent();
        if (rightWallGrabEvent == null) rightWallGrabEvent = new UnityEvent();
        if (leftWallGrabEvent == null) leftWallGrabEvent = new UnityEvent();
        if (WallReleaseEvent == null) WallReleaseEvent = new UnityEvent();
    }
    private void Update()
    {
        jumpBtn = Input.GetButton("Jump");
        lr = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Dash"))
        {
            dashBtnDown = true;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBtnDown = true;
        }
    }
    private void FixedUpdate()
    {
        velocity = playerRb.velocity;
        if (dashBtnDown)
        {
            dash();
            dashBtnDown = false;
        }
        if (dashing) {
            dashMove();
        }
        else {
            MoveHorizontal();
        }
        if (GroundedCheck())
        {
            if (wallGrab)
            {
                wallGrab = false;
                WallReleaseEvent.Invoke();
            }
            if (grounded == false)
            {
                grounded = true;
                TriggerGrounded();
            }
            if (jumpBtnDown)
            {
                TriggerGroundJump();
                jumpBtnDown = false;
            }
            if (jumpBtn) {
                flight();
            }
        }
        else
        {
            grounded = false;
            int walled = WallCheck();
            if (walled != 0)
            {
                if (!wallGrab) {
                    wallGrab = true;
                    airJumpsRemaining = airJumps;
                    if (walled == 1)
                    {
                        rightWallGrabEvent.Invoke();
                    }
                    else
                    {
                        leftWallGrabEvent.Invoke();

                    }
                }
                if (jumpBtnDown)
                {
                    TriggerWallJump(walled);
                    jumpBtnDown = false;
                }
                wallSlide();
            }
            else
            {
                if (wallGrab)
                {
                    wallGrab = false;
                    WallReleaseEvent.Invoke();
                }
                if (jumpBtnDown)
                {
                    TriggerAirJump();
                    jumpBtnDown = false;
                }
                else if (jumpBtn)
                {
                    flight();
                }
                else
                {
                    fastStop();
                }
            }
        }
        ExternalForce();
        playerRb.velocity = velocity;
    }
    void ExternalForce() {
        velocity = velocity + externalForce;
        Vector2 factor = externalForce * externalForceDecayFactorPerSec;
        externalForce -= factor * Time.fixedDeltaTime;
        if (Mathf.Abs(externalForce.x) < .1f) externalForce.x = 0;
        if (Mathf.Abs(externalForce.y) < .1f) externalForce.y = 0;
    }
    void dashMove() {
        velocity = new Vector2(dashSpeed * (facingRight ? 1 : -1), 0);
    }
    void dash()
    {
        if (dashReady && allowDash)
        {
            StartCoroutine(DashStart());
        }
    }
    IEnumerator dashCounter()
    {
        yield return new WaitForSeconds(dashInterval);
        dashReady = true;
    }
    IEnumerator DashStart()
    {
        dashReady = false;
        dashStartEvent.Invoke();
        StartCoroutine(dashCounter());
        dashing = true;
        yield return new WaitForSeconds(dashTime);
        dashing = false;
        dashEndEvent.Invoke();
    }
    void fastStop()
    {
        jumpedSteps = maxJumpSteps;
        if (playerRb.velocity.y > 0) {
            velocity = new Vector2(playerRb.velocity.x, 0);
        }
    }
    void flight()
    {

        if (jumpedSteps < maxJumpSteps)
        {
            velocity = new Vector2(velocity.x, jumpVelocity);
            jumpedSteps += Time.fixedDeltaTime;
        }
    }
    void TriggerGrounded() {
        airJumpsRemaining = airJumps;
        jumpedSteps = 0;
        LandEvent.Invoke();
    }
    void TriggerGroundJump()
    {
        jumpedSteps = 0;
        groundJumpStartEvent.Invoke();
    }
    void TriggerWallJump(int walled)
    {
        jumpedSteps = 0;//??in wallGrab?
        wallJumpEvent.Invoke();
        externalForce.x += -walled * wallJumpHorizontalForce;
    }
    public void AddForce(Vector2 force) {
        externalForce += force;
    }
    void TriggerAirJump() {
        if (airJumpsRemaining > 0)
        {
            airJumpsRemaining--;
            AirJumpStartEvent.Invoke();
            jumpedSteps = 0;
        }
    }
    void wallSlide()
    {
        if (playerRb.velocity.y < 0)
            velocity.y = wallJumpSideForce;
        ///TODO: wallSlide
    }
    void MoveHorizontal()
    {
        velocity = new Vector2(lr * horizontalSpeed, velocity.y);
        if (lr > 0 && !facingRight || lr < 0 && facingRight) {
            flip();
        }
    }
    bool GroundedCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckBoxCastSize, 0, platformLayer);
        return hit != null;
    }
    int WallCheck()
    {
        //1-right, -1-left, 0 none
        if (!allowWallJump) return 0;
        if (facingRight)
        {
            Collider2D hit = Physics2D.OverlapBox(wallCheckRight.position, wallCheckBoxCastSizeRight, 0, platformLayer);
            if (hit != null && hit.CompareTag("platformClimb"))
            {
                return 1;
            }
        }
        if (!facingRight)
        {
            Collider2D hit = Physics2D.OverlapBox(wallCheckLeft.position, wallCheckBoxCastSizeLeft, 0, platformLayer);
            if (hit != null && hit.CompareTag("platformClimb"))
            {
                return -1;
            }
        }
        return 0;
    }
    void flip()
    {
        facingRight = !facingRight;
    }
    public void transfer(Vector3 position) {
        transform.position = position;
    } 
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckBoxCastSize);
        Gizmos.DrawWireCube(wallCheckRight.position, wallCheckBoxCastSizeRight);
        Gizmos.DrawWireCube(wallCheckLeft.position, wallCheckBoxCastSizeLeft);
    }
}