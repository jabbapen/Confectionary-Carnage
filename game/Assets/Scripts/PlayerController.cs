using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthManager))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;         // Maximum speed for player movement
    public float accelerationRate = 10f; // Rate of acceleration in units per second
    public float decelerationRate = 10f; // Rate of deceleration in units per second
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 targetVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleShooting();

        // Capture input from horizontal and vertical axes (WASD or Arrow Keys by default)
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Set the target velocity based on the input and max speed
        targetVelocity = movement.normalized * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.SetupLevel();
        }
    }

    void FixedUpdate()
    {
        Aim();
        HandleCD();

        // Determine if we're accelerating or decelerating
        float rate = (movement.magnitude > 0) ? accelerationRate : decelerationRate;

        // Smoothly move the current velocity towards the target velocity at a constant rate
        rb.velocity = Vector2.MoveTowards(rb.velocity, targetVelocity, rate * Time.fixedDeltaTime);
    }

    void OnDisable()
    {
        // Stop movement if the script is disabled to prevent unwanted momentum
        rb.velocity = Vector2.zero;
    }

    #region AIMING

    [SerializeField] GameObject bullet;

    [SerializeField] Transform crosshair;
    int shootCD;

    void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootCD < 1)
            {
                Instantiate(bullet, transform.position, transform.rotation);
                shootCD = 50;
            }
        }
    }

    void HandleCD()
    {
        if (shootCD > 0) { shootCD--; }        
    }

    void Aim()
    {
        transform.up = CursorObj.trfm.position - transform.position;
    }

#endregion
}
