using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthManager))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;         // player movement acceleration
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 targetVelocity;

    public static Transform trfm;

    private void Awake()
    {
        trfm = transform;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleShooting();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.SetupLevel();
        }
    }

    void FixedUpdate()
    {
        Aim();
        HandleCD();

        // Capture input from horizontal and vertical axes (WASD or Arrow Keys by default)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Set the target velocity based on the input and max speed
        rb.velocity += movement.normalized * moveSpeed;
    }

    void OnDisable()
    {

    }

    #region AIMING

    [SerializeField] GameObject bullet;

    [SerializeField] Transform crosshair;
    int shootCD, bullets, bulletCD;

    void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootCD < 1)
            {
                Instantiate(bullet, transform.position, transform.rotation);
                shootCD = 50;
                bullets--;
            }
        }
    }

    void HandleCD()
    {
        if (shootCD > 0) { shootCD--; }    
        if (bullets < 3)
        {
            bulletCD--;
            if (bulletCD < 1)
            {
                bulletCD = 60;
                bullets++;
            }
        }
    }

    void Aim()
    {
        transform.up = CursorObj.trfm.position - transform.position;
    }

#endregion
}
