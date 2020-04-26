using System.Collections;
using UnityEngine;

public class playerAttack : MonoBehaviour
{
    private attackMelee playerMeleeAttack;
    public float firstAttackTime;
    public Transform attackPointLeft,attackPointRight,attackPointDown,attackPointUp;
    [Header("firstAttack")]
    public float firstAttackRadiusUp, firstAttackRadiusDown, firstAttackRadiusLeft, firstAttackRadiusRight;
    public int firstAttackDamage;
    public LayerMask firstAttackLayersAffected;
    private bool attackReady;
    [Header("test")]
    public GameObject AttackTest;
    public bool showAttack=false;
    private void Awake()
    {
        playerMeleeAttack = new attackMelee();
        attackReady = true;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Attack1")) {
            Attack1();
        }
        AttackTest.GetComponent<SpriteRenderer>().enabled=showAttack;
    }
    void Attack1() {
        if (attackReady)
        {
            StartCoroutine(Attack1Counter());
            if (Input.GetAxisRaw("Vertical") == 1)
            {
                playerMeleeAttack.Attack(attackPointUp.position, firstAttackRadiusUp, firstAttackLayersAffected, firstAttackDamage,transform.position,3);
                testEffect(attackPointUp.position,firstAttackRadiusUp);
            }
            else if (Input.GetAxisRaw("Vertical") == -1)
            {
                playerMeleeAttack.Attack(attackPointDown.position, firstAttackRadiusDown, firstAttackLayersAffected, firstAttackDamage,transform.position,4);
                testEffect(attackPointDown.position, firstAttackRadiusDown);
            }
            else if (GetComponent<PlayerController>().facingRight)
            {
                playerMeleeAttack.Attack(attackPointRight.position, firstAttackRadiusRight, firstAttackLayersAffected, firstAttackDamage,transform.position,2);
                testEffect(attackPointRight.position, firstAttackRadiusRight);
            }
            else {
                playerMeleeAttack.Attack(attackPointLeft.position, firstAttackRadiusLeft, firstAttackLayersAffected, firstAttackDamage,transform.position,1);
                testEffect(attackPointLeft.position, firstAttackRadiusLeft);
            }
        }
    }
    IEnumerator Attack1Counter() {
        attackReady = false;
        yield return new WaitForSeconds(firstAttackTime);
        attackReady = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPointLeft.position, firstAttackRadiusLeft);
        Gizmos.DrawWireSphere(attackPointRight.position, firstAttackRadiusRight);
        Gizmos.DrawWireSphere(attackPointUp.position, firstAttackRadiusUp);
        Gizmos.DrawWireSphere(attackPointDown.position, firstAttackRadiusDown);
    }
    void testEffect(Vector2 position,float radius) {
        AttackTest.transform.position = position;
        AttackTest.transform.localScale = new Vector2(radius*2, radius*2);
        StartCoroutine(ShowAttack());
    }
    IEnumerator ShowAttack() {
        showAttack = true;
        yield return new WaitForSeconds(.2f);
        Debug.Log("dsa");
        showAttack = false;
    }
}
