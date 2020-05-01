//author @ Anuj
using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour
{
    private int health;
    public int maxHealth;
    private bool isAlive;
    public bool isInvincible = false;
    public UnityEvent resurectEvent,deadEvent,damageEvent,healEvent;
    protected virtual void Awake()
    {
        if(resurectEvent==null) resurectEvent = new UnityEvent();
        if (damageEvent == null) damageEvent = new UnityEvent();
        if (deadEvent== null) deadEvent= new UnityEvent();
        if (healEvent == null) healEvent = new UnityEvent();
        born();
    }
    void born() {
        health = maxHealth;
        isAlive = true;
        resurectEvent.Invoke();
    }
    public void heal(int heal) {
        if (!isAlive) return;
        if (heal > maxHealth - health)
        {
            health = maxHealth;
        }
        else {
            health += heal;
        }
        healEvent.Invoke();
    }
    public void kill() {
        if (!isAlive) return;
        health = 0;
        deadEvent.Invoke();
        isAlive = false;
    }
    public virtual void Damage(int damage)
    {
        if (!isAlive||isInvincible) return;
        if (health < damage)
        {
            health = 0;
            isAlive = false;
            deadEvent.Invoke();
        }
        else
        {
            health -= damage;
            damageEvent.Invoke();
        }
    }
}
