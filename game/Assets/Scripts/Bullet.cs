using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// XXX: Some code duplication with enemy projectiles
/// <summary>
/// Player projectile class. Uses Monobehavior to interact with enemies and move
/// across the screen. Interacts with enemy HealthManagers to deal damage.
/// Implements `Start`, `FixedUpdate`, and `OnTriggerEnder2D` Unity functions.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] GameObject destroyFX;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7 || col.gameObject.layer == 6)
        {
            if (col.gameObject.layer == 7)
            {
                HealthManager otherHealth;
                if (col.gameObject.TryGetComponent(out otherHealth))
                {
                    otherHealth.Health -= damage;
                }
            }
            Instantiate(destroyFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }        
    }
}
