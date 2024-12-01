using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(IWeapon))]
public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float repathCD = 0.2f;
    [SerializeField] private float aggroRadius = 10f;

    private float repathTime = 0f;
    private bool startPathfinding;

    // Start is called before the first frame update
    private PlayerController player;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private IWeapon weapon;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponent<IWeapon>();

        agent.updateRotation = false;
		agent.updateUpAxis = false;
        agent.enabled = false;
        startPathfinding = false;
        GameManager.GameStart.AddListener(OnGameStart);
        if (GameManager.Instance == null)  // Debug mode
        {
            OnGameStart();
        }
    }

    void FixedUpdate()
    {
        if (player == null || agent.enabled == false)
            return;

        // Handle movement
        // TODO: check for line of sight?

        // updatePathing();
        // tryAttack();
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
        if (weapon.Ready() && weapon.Usable(player.gameObject))
        {
            StartCoroutine(weapon.Use(player.gameObject));
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
