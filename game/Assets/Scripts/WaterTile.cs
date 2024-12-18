using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slows the player down when a Tile with this Monobehavior collides with
/// the player.
/// </summary>
public class WaterTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            PlayerController.self.EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            PlayerController.self.ExitWater();
        }
    }
}
