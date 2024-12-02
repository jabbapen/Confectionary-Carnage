using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWeapon : IWeapon
{
    private float attackTimer = 0f;
    public GameObject projectilePrefab;

    public override bool Ready() 
    {
        return attackTimer <= 0;
    }
    public override bool Usable(GameObject target)
    {
        return Vector2.Distance(target.transform.position, transform.position) < attackRange;
    }

    public override IEnumerator Use(GameObject target)
    {
        if (!Ready()) 
            yield break;

        // spawn a projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Direction = target.transform.position - transform.position;
        attackTimer = attackCD;
    }

    void FixedUpdate()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }
}
