using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteItem : MonoBehaviour
{
    [SerializeField] int tileID;
    [SerializeField] float hoverDistance;
    [SerializeField] Vector2 defaultSize, hoverSize;
    bool hover;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && hover)
        {
            LevelEditor.self.RequestBrush(tileID);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hover != (CursorObj.trfm.position - transform.position).sqrMagnitude < hoverDistance * hoverDistance)
        {
            hover = !hover;
            LevelEditor.paletteHover = hover;

            if (hover)
            {
                transform.localScale = hoverSize;
            } else
            {
                transform.localScale = defaultSize;
            }
        }
    }
}
