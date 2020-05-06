using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerFollowMovement : MonoBehaviour
{
    private GameObject player;
    private Vector2 currentVelocity;
    public float playerFollowRange,playerMinDistance,acceleration,maxSpeed;
    [Header("randomMovementBias")]
    public bool doRandomMovementBias;
    private bool biasTrigger=true;
    private Vector2 randomMovementBiasVelocity;
    public float randomMovementBiasStrength, randomMovementBiasPeriod;
    void Start()
    {
        player = gameData.player;
    }
    void FixedUpdate()
    {
        float distamce = Vector2.Distance(player.transform.position, transform.position);
        if (distamce< playerFollowRange&&distamce>playerMinDistance)
        {
            Vector2 direction = player.transform.position-transform.position  ;
            Vector2 force = direction * acceleration * Time.fixedDeltaTime;
            currentVelocity += force;
            //speed constraint
            float speedSquared = currentVelocity.sqrMagnitude;
            if (speedSquared > maxSpeed * maxSpeed)
            {
                currentVelocity = direction.normalized * maxSpeed;
            }
        }
        else {
            currentVelocity -= currentVelocity * Time.fixedDeltaTime * acceleration;
        }
        if (doRandomMovementBias )
        {
            if (biasTrigger)
            {
                biasTrigger = false;
                StartCoroutine(biasTigger());
                randomMovementBiasVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * randomMovementBiasStrength;
            }
            currentVelocity += randomMovementBiasVelocity * Time.fixedDeltaTime;
        }

        GetComponent<Rigidbody2D>().velocity = currentVelocity;
    }
    IEnumerator biasTigger() {
        yield return new WaitForSeconds(randomMovementBiasPeriod);
        biasTrigger = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, playerFollowRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerMinDistance);
    }
}
