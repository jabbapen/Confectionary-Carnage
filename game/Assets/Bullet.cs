using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
