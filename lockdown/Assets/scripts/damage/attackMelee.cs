
using UnityEngine;

public class attackMelee
{
    public void Attack(Vector2 position, float radius, LayerMask layersAffected, int damage,Vector2 attackCutPos,int cutIfIsInLRUD) {
        Collider2D[] victims= Physics2D.OverlapCircleAll(position, radius, layersAffected);
        foreach(Collider2D victim in victims)
        {
            switch (cutIfIsInLRUD) {
                case 0:
                    break;
                case 1:
                    if (victim.transform.position.x > attackCutPos.x) continue;
                    break;
                case 2:
                    if (victim.transform.position.x < attackCutPos.x) continue;
                    break;
                case 3:
                    if (victim.transform.position.y < attackCutPos.y) continue;
                    break;
                case 4:
                    if (victim.transform.position.y > attackCutPos.y) continue;
                    break;
                default:
                    Debug.Log("direction not matched");
                    break;

            }
            Health victimHealth = victim.GetComponent<Health>();
            if (victimHealth != null) {
                victimHealth.Damage(damage);
            }
        }
    }
}
