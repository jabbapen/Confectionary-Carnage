using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObj : MonoBehaviour
{
    public static Vector2 pos;
    public static Transform trfm;
    public SpriteRenderer spriteRenderer;

    public static CursorObj self;
    // Start is called before the first frame update
    void Awake()
    {
        self = GetComponent<CursorObj>();
        trfm = transform;
    }

    // Update is called once per frame
    void Update()
    {
        pos = trfm.position;
        trfm.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
    }
}
