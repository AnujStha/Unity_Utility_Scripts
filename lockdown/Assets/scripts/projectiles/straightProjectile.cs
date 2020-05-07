
using UnityEngine;
using UnityEngine.Events;

public class straightProjectile : MonoBehaviour,IProjectile
{ 
    private Rigidbody2D rb;
    public UnityEvent releaseEvent, contactEvent;
    public LayerMask  damageLayers;
    private int contactDamage;
    void Awake() {
        if (releaseEvent==null) releaseEvent = new UnityEvent();
        if(contactEvent==null) contactEvent = new UnityEvent();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    public void throwAtDirection(Vector2 direction, float speed,int damage,  LayerMask damageLayer)
    {
        rb.velocity = direction.normalized * speed;
        contactDamage = damage;
        damageLayers = damageLayer;
        releaseEvent.Invoke();
    }
    public void throwAtPoint(Vector2 point, float speed,int damage,  LayerMask damageLayer)
    {
        rb.velocity = ( point- new Vector2(transform.position.x, transform.position.y)).normalized * speed;
        contactDamage = damage;
        damageLayers = damageLayer;
        releaseEvent.Invoke();
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedObject = collision.gameObject;
            if (((1 << collidedObject.layer) & damageLayers) != 0)
            {
                collidedObject.GetComponent<Health>().Damage(contactDamage);
            }
            contactEvent.Invoke();
            Destroy(gameObject);
    }
}
