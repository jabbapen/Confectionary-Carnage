using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    [SerializeField] protected float attackCD;
    [SerializeField] public float attackRange;
    [SerializeField] protected int attackDamage = 1;
    abstract public bool Ready();
    abstract public bool Usable(GameObject target);
    abstract public IEnumerator Use(GameObject target);
}
