using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An implementation of @Global.IWeapon which directly damages the player when
/// used, but has a short range.
/// </summary>
public class MeleeWeapon : IWeapon 
{
    [SerializeField] private float attackDelay = 0.5f;
    private float attackTimer = 0f;

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

    public override IEnumerator Use(GameObject target)
    {
        if (!Ready()) 
            yield break;
        Vector2 direction = target.transform.position -  transform.position;
        attackTimer = attackCD;
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
                // attackTimer = attackCD;
            }
        }
    }
}
