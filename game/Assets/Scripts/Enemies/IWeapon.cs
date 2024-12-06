using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the Weapon Strategy, which is used by EnemyManager.
/// Weapons keep track of their own cooldowns, and have some interfaces so the
/// enemy can act depending on the weapon state (eg. run away if weapon is not ready).
/// </summary>
public abstract class IWeapon : MonoBehaviour
{
    /// <summary>
    /// All weapons have an attack cooldown
    /// </summary>
    [SerializeField] protected float attackCD;
    /// <summary>
    /// Get the attack range of the weapon. Enemies need to know this to get in
    /// range of the player.
    /// </summary>
    [SerializeField] public float attackRange;
    /// <summary>
    /// The amount of damage to do when the weapon hits.
    /// </summary>
    [SerializeField] protected int attackDamage = 1;
    
    /// <summary>
    /// Get whether the weapon is Ready or not, typically when the cooldown is zero.
    /// </summary>
    /// <returns>Whether the weapon is ready or not</returns>
    abstract public bool Ready();
    /// <summary>
    /// Get whether the weapon is Usable, typically when the cooldown is zero
    /// AND the player is close enough.
    /// </summary>
    /// <returns>Whether the weapon is usable</returns>
    abstract public bool Usable(GameObject target);
    /// <summary>
    /// What to do when the weapon is used, eg. spawn projectiles, damage the
    /// player directly, etc.
    /// </summary>
    /// <returns>IEnumerator: This happens asynchronously as a Unity coroutine,
    /// so it must return an IEnumerator</returns>
    abstract public IEnumerator Use(GameObject target);
}
