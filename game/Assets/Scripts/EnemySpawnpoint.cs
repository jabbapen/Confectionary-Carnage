using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    public GameObject GetEnemy()
    {
        return enemy;
    }
}
