using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class playerHealth : Health
{
    public float damageRecoveryInvincibleTime;
    public UnityEvent invincibleEvent, invincibleEndEvent;
    [Header("test")]
    public Color invincibleColor;
    Color def;
    public SpriteRenderer sp;
    protected override void Awake()
    {
        base.Awake();
        if (invincibleEvent == null) invincibleEvent = new UnityEvent();
        if (invincibleEndEvent == null) invincibleEndEvent = new UnityEvent();
        def = sp.color;
    }
    public override void Damage(int damage)
    {
        base.Damage(damage);
        if(!isInvincible) StartCoroutine(invincibleCounter());

    }
    IEnumerator invincibleCounter() {
        //test////
        sp.color = invincibleColor;
        /////////
        isInvincible = true;
        invincibleEvent.Invoke();
        yield return new WaitForSeconds(damageRecoveryInvincibleTime);
        isInvincible = false;
        invincibleEndEvent.Invoke();
        //////test
        sp.color = def;
    }
}
