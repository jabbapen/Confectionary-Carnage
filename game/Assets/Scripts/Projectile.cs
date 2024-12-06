using UnityEngine;

/// <summary>
/// Projectile launched by enemies, implements Unity's `FixedUpdate` to move
/// across the screen per frame, and implements `OnTriggerEnter2D` to define
/// collision logic.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float lifespan = 10f;
    private Vector3 direction; 
    public Vector3 Direction {
        get {return direction;} 
        set {direction = value.normalized;}
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManager otherHealth;
        if (!other.gameObject.CompareTag(tag) && 
            other.gameObject.TryGetComponent(out otherHealth))
        {
            otherHealth.Health -= damage;
        }
        if (!other.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.position += Time.deltaTime * velocity * transform.up;
        lifespan -= Time.deltaTime;
        if (lifespan <= 0) 
            Destroy(gameObject);
    }
}
