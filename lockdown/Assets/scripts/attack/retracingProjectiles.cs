using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class retracingProjectiles : MonoBehaviour,IAttack
{
    public float retraceRadius;
    public int numberOfSpikes;
    public GameObject spike;
    public float projectileSpeed, projectileStrikeSpeed, spikeCollectionRadius, spikeCollectionTime, spikeThrowInterval, spikeAngle;
    private List<GameObject> projectiles;
    public bool destroyProjectilesOnReturn,leaveParentOnGo;
    public LayerMask projectileDamageLayer;

    public void Attack(int damage)
    {
        //spawn
        if (numberOfSpikes > 1)
        {
            for (int i = 0; i < numberOfSpikes; i++)
            {
                GameObject proj = Instantiate(spike, transform.position, transform.rotation, transform);
                projectiles.Add(proj);
                float angleRotation = -(spikeAngle / 2) + i * spikeAngle / (numberOfSpikes - 1);
                proj.transform.Rotate(new Vector3(0, 0, angleRotation));
            }
        }
        else
        {
            Debug.Log("numberOfSpikeShouldBeGreaterThan1");
        }
        //set and go
        StartCoroutine(setAndGo(damage));
    }
    IEnumerator setAndGo(int damage)
    {
        float time = spikeCollectionRadius / projectileSpeed;
        foreach (GameObject s in projectiles)
        {
           if(s!=null) s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, projectileSpeed, damage, projectileDamageLayer,transform.parent.gameObject);
        }
        yield return new WaitForSeconds(time);
        //wait
        foreach (GameObject s in projectiles)
        {
            if(s!=null)s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, 0, damage, projectileDamageLayer,transform.parent.gameObject);
        }
        yield return new WaitForSeconds(spikeCollectionTime);
        //go
        float retraceAcceleration = -(-projectileStrikeSpeed * projectileStrikeSpeed) / (2 * retraceRadius);
        float projectileReturnTime = 2 * projectileStrikeSpeed / retraceAcceleration + +spikeCollectionRadius / projectileStrikeSpeed;
        for (int i = 0; i < projectiles.Count; i++)
        {
            GameObject s = projectiles[i];
            if (s != null)
            {
                s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, projectileStrikeSpeed, damage, projectileDamageLayer,transform.parent.gameObject);
                s.GetComponent<straightProjectileWithConstantAcceleration>().constantAcceleration = -s.transform.up.normalized * retraceAcceleration;
                if(leaveParentOnGo) s.transform.parent = null;
                if (destroyProjectilesOnReturn)
                {
                    Destroy(s, projectileReturnTime);
                }
                if(spikeThrowInterval>0) yield return new WaitForSeconds(spikeThrowInterval);
            }
        }
        Debug.Log("caalled");
        projectiles.Clear();
    }
    void Start()
    {
        projectiles = new List<GameObject>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spikeCollectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retraceRadius);
    }
}
