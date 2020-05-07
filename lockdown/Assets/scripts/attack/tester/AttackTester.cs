using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTester : MonoBehaviour
{
    public GameObject attack;
    public int damage;
    public bool attackTrigger;
    public float attackInterval;
    private IAttack AttackScript;
    private void Awake()
    {
        AttackScript = attack.GetComponent<IAttack>();
    }
    private void Start()
    {
        if (attackInterval > 0) StartCoroutine(attackCycle());
    }
    private void Update()
    {
        if (attackTrigger) {
            attackTrigger = false;
            AttackScript.Attack(damage);
        }
    }
    IEnumerator attackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            AttackScript.Attack(damage);
        }

    } 
}
