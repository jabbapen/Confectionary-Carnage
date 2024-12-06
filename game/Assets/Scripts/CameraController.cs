using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the camera around to follow the player. Implements `FixedUpdate()` to
/// update its position smoothly.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerTrfm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (playerTrfm.position - transform.position + Vector3.forward * -10) * 0.05f;
    }
}
