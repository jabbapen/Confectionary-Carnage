using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : IWeapon 
{
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCD = 1f;
    private float attackTimer = 0f;

    public override IEnumerator Use(GameObject target)
    {
        Vector2 direction = target.transform.position -  transform.position;
        yield return new WaitForSeconds(attackDelay);
        
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.CircleCast(transform.position, attackRange / 2, direction, new ContactFilter2D(), hits, attackRange / 2);
        HealthManager hitHealth;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out hitHealth) && !hitHealth.CompareTag(tag))
            {
                Debug.Log("Hit another entity");
                hitHealth.Health -= attackDamage;
                attackTimer = attackCD;
            }
        }
    }
    public override bool Ready() 
    {
        return attackTimer <= 0;
    }
    public override bool Usable(GameObject target)
    {
        return Vector2.Distance(target.transform.position, transform.position) < attackRange;
    }
    void FixedUpdate()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }
}
