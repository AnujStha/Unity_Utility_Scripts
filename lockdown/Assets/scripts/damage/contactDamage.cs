using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contactDamage : MonoBehaviour
{
    public LayerMask damageFor;
    public int damage;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1<<collision.gameObject.layer) & damageFor.value) != 0)
        {
            Damage(collision);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & damageFor.value) != 0)
        {
            Damage(collision);
        }
    }
    protected virtual void Damage(Collision2D collision) {
        Health victim = collision.gameObject.GetComponent<Health>();
        if (victim != null)
        {
            victim.Damage(damage);
        }
    }
}
