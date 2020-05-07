
using UnityEngine;

public interface IProjectile 
{
    void throwAtPoint(Vector2 point,float speed,int damage,LayerMask damageLayers);
    void throwAtDirection(Vector2 direction,float speed,int damage, LayerMask damageLayers);
}
