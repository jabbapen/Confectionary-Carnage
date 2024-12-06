using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct so objects interacting with Tile can easily get its `GameObject` and `Transform`, eg. when serializing.
/// GameObjects can't hold onto values otherwise, so sometimes Data classes are necessary in Unity.
/// </summary>
public class Tile : MonoBehaviour
{
    public GameObject obj;
    public Transform trfm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
