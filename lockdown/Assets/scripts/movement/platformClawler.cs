using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformClawler : MonoBehaviour
{
    [Header("sensors")]
    public Transform groundCheckPoint, wallcheckPoint;
    public Vector2 groundCheckBoxCastSize, wallCheckBoxCastSize;
    public LayerMask platformLayer;
    [Header("movement")]
    public float speed,haltRotateAfterRotationTime;
    private bool haltRotate;
    private float groundCheckDirection=0, wallCheckDirection=0;
    void Start()
    {
        if (Mathf.Abs(transform.rotation.eulerAngles.z) % 180 == 0)
        {
            groundCheckDirection = 0;
            wallCheckDirection = 0;
        }
        else if ((Mathf.Abs(transform.rotation.eulerAngles.z) + 90) % 180 == 0)
        {
            groundCheckDirection = 90;
            wallCheckDirection = 90;
        }
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
        if (!GroundedCheck()&&!haltRotate) {
            transform.Rotate(new Vector3(0, 0, -90));
            groundCheckDirection -= 90;
            wallCheckDirection -= 90;
            haltRotate = true;
            StartCoroutine(haltRotateCounter());
        }
        if (WallCheck() && !haltRotate) {
            groundCheckDirection += 90;
            wallCheckDirection += 90;
            transform.Rotate(new Vector3(0, 0, 90));
            haltRotate = true;
            StartCoroutine(haltRotateCounter());
        }
    }
    bool GroundedCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckBoxCastSize,groundCheckDirection, platformLayer);
        return hit != null;
    }
    bool WallCheck()
    {
            Collider2D hit = Physics2D.OverlapBox(wallcheckPoint.position, wallCheckBoxCastSize, wallCheckDirection, platformLayer);     
            return (hit!=null);
    }
    IEnumerator haltRotateCounter() {
        yield return new WaitForSeconds(haltRotateAfterRotationTime);
        haltRotate = false;
    }
    private void OnDrawGizmosSelected()
    {

    }
}
