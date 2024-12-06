using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds health to a GameObject, allowing it to be damaged, and destroying it
/// when health is set to zero.
/// </summary>
public class HealthManager : MonoBehaviour
{
    /// <summary>
    /// Health cap and the amount of health the GameObject starts with.
    /// </summary>
    [SerializeField] public int MaxHealth = 3;
    private bool dying = false;

    /// <summary>
    /// Dying is set to true if the GameObject is in the process of being
    /// destroyed, eg. when a death animation is playing.
    /// </summary>
    public bool Dying { get {return dying;} }

    [SerializeField] private int health;
    public int Health {
        get { return health; }
        set { 
            health = value;
            dying = health <= 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // we might want to delay destroying the gameObject later (eg. death animation)
        if (dying)
            Destroy(gameObject);
    }
}
