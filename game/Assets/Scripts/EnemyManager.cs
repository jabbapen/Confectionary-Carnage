using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private float repathCD = 0.2f;
    [SerializeField] private float aggroRadius = 3f;

    [SerializeField] private float attackCD = 1f;

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float agentSpeed = 1f;

    private float attackDelay = 0f;
    private float repathTime = 0f;
    private float attackTime = 0f;

    // Start is called before the first frame update
    private PlayerController player;
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
        HealthManager other;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out other))
            {
                Debug.Log("Hit another entity");
            }
        }
    }
}
