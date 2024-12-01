using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float repathCD = 0.2f;
    [SerializeField] private float aggroRadius = 10f;

    [SerializeField] private float attackCD = 1f;

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private int attackDamage = 1;

    private float attackDelay = 0f;
    private float repathTime = 0f;
    private float attackTime = 0f;
    private bool startPathfinding;

    // Start is called before the first frame update
    private PlayerController player;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
		agent.updateUpAxis = false;
        agent.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        startPathfinding = false;
        GameManager.GameStart.AddListener(OnGameStart);
        if (GameManager.Instance == null)  // Debug mode
        {
            OnGameStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    void FixedUpdate()
    {
        if (player == null || agent.enabled == false)
            return;

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
            if (attackDelay > 0.25f && attackTime <= 0)
            {
                attack(player.transform.position);
                attackTime = attackCD;
                attackDelay = 0;
            }
        }
        attackTime = Math.Max(attackTime - Time.deltaTime, 0);
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

    void OnGameStart()
    {
        agent.enabled = true;
    }

    public void ResetPath()
    {
        agent.ResetPath(); 
    }
}
