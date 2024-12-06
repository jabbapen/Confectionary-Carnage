using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements Unity's `OnTriggerEnter2D` to detect when a player reaches the
/// endzone, then makes @Global.GameManager load the next level.
/// </summary>
public class EndzoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SetupLevel();
        }
    }
}
