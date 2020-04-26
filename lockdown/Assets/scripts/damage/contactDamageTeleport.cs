using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contactDamageTeleport : contactDamage
{
    public Transform transportPoint;
    protected override void Damage(Collision2D collision)
    {
        base.Damage(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            gameData.player.transform.position = transportPoint.position;
        }
    }
}
