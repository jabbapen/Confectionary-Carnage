using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] public int MaxHealth = 3;
    private bool dying = false;
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
