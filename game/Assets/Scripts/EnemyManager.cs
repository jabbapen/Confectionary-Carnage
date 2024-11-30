using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float repathCD = 0.2f;
    [SerializeField] private float aggroRadius = 3f;

    [SerializeField] private float attackCD = 1f;

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int attackDamage = 1;

    private float attackDelay = 0f;
    private float repathTime = 0f;
    private float attackTime = 0f;

    // Start is called before the first frame update
    private PlayerController player;
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent agent;
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void FixedUpdate()
    {
        // Handle movement
        // TODO: check for line of sight?
        if (Vector2.Distance(transform.position, player.transform.position) < aggroRadius && repathTime <= 0)
        {
            if (!agent.pathPending)
            {
                agent.SetDestination(player.transform.position);
                repathTime = repathCD;
            }
        }
        else 
        {
            repathTime -= Time.deltaTime;
        }

        // if close, attack the player
        if (Vector2.Distance(transform.position, player.transform.position) < attackRadius)
        {
            attackDelay += Time.deltaTime;
            if (attackDelay > 0.25f)
                attack(player.transform.position);
            attackTime = attackCD;
        }
        else
        {
            attackDelay = 0;
            attackTime -= Time.deltaTime;
        }

    }
    void attack(Vector2 direction)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.CircleCast(transform.position, attackRadius/2, direction, new ContactFilter2D(), hits, attackRadius/2);
        HealthManager hitHealth;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out hitHealth) && !hitHealth.CompareTag(tag))
            {
                Debug.Log("Hit another entity");
                hitHealth.Health -= attackDamage;
            }
        }
    }
}
