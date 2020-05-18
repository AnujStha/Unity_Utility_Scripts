using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class porcupineAttack : MonoBehaviour,IAttack
{
    public int numberOfSpikes;
    public GameObject spike;
    public float projectileSpeed, projectileStrikeSpeed, spikeCollectionRadius, spikeCollectionTime, spikeThrowInterval,spikeAngle;
    private List<GameObject> projectiles;
    public LayerMask projectileDamageLayer;
    public bool throwAtPlayer;

    public void Attack(int damage)
    {
        //spawn
        if (numberOfSpikes > 1)
        {
            for (int i = 0; i < numberOfSpikes; i++)
            {
                GameObject proj = Instantiate(spike, transform.position,transform.rotation,transform);
                projectiles.Add(proj);
                float angleRotation = -(spikeAngle / 2) + i * spikeAngle / (numberOfSpikes - 1);
                proj.transform.Rotate(new Vector3(0, 0, angleRotation));
            }
        }
        else {
            Debug.Log("numberOfSpikeShouldBeGreaterThan1");
        }

        //set and go
        StartCoroutine(setAndGo(damage));
    }
    IEnumerator setAndGo( int damage)
    {
        float time = spikeCollectionRadius / projectileSpeed;
        foreach (GameObject s in projectiles)
        {
            if(s!=null)s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, projectileSpeed, damage, projectileDamageLayer,transform.parent.gameObject);
        }
        yield return new WaitForSeconds(time);
        //wait
        foreach (GameObject s in projectiles)
        {
            if(s!=null)s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, 0, damage, projectileDamageLayer,transform.parent.gameObject);
        }
        yield return new WaitForSeconds(spikeCollectionTime);
        //go
        for (int i = 0; i < projectiles.Count; i++)
        {
            GameObject s = projectiles[i];
            if (s != null) {
                if (throwAtPlayer)
                {
                    s.GetComponent<IProjectile>().throwAtPoint(gameData.player.transform.position, projectileStrikeSpeed, damage, projectileDamageLayer,transform.parent.gameObject);
                }
                else
                {
                    s.GetComponent<IProjectile>().throwAtDirection(s.transform.up, projectileStrikeSpeed, damage, projectileDamageLayer,transform.parent.gameObject);
                }
                s.transform.parent = null;
                yield return new WaitForSeconds(spikeThrowInterval);
            }
        }
        projectiles.Clear();
    }
    void Start()
    {
        projectiles = new List<GameObject>();   
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spikeCollectionRadius);
    }
}
