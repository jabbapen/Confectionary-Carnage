using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
