using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class which moves the children of a GameObject out of the GameObject
/// when it is initialized.
/// </summary>
public class Deparent : MonoBehaviour
{
    [SerializeField] Transform[] children;
    void Awake()
    {
        foreach (Transform child in children)
        {
            child.parent = null;
        }
    }
}
