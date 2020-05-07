
using UnityEngine;

public class spitAtPlayer : MonoBehaviour, IAttack
{
    public GameObject projectile;
    public float projectileSpeed;
    public LayerMask  damageLayers;
    public void Attack(int damage)
    {
        GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
        IProjectile projectileScript = proj.GetComponent<IProjectile>();
        projectileScript.throwAtPoint(gameData.player.transform.position, projectileSpeed,damage, damageLayers);
    }
}
