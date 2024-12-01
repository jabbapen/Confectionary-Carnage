using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    [SerializeField] private float Cooldown;
    [SerializeField] private float Range;
    abstract public bool Ready();
    abstract public bool Usable(GameObject target);
    abstract public IEnumerator Use(GameObject target);
}
