using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container to hold an @Global.EnemyManager GameObject
/// </summary>
public class EnemySpawnpoint : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    public GameObject GetEnemy()
    {
        return enemy;
    }
}
