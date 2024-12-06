using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the tile painting feature when in the LevelEditor.
/// Uses `Update` to "paint" the level scene per frame the mouse is down, and
/// communicates with @Global.LevelEditor to update the level state accordingly.
/// </summary>
public class PaletteItem : MonoBehaviour
{
    [SerializeField] int tileID;
    [SerializeField] float hoverDistance;
    [SerializeField] Vector2 defaultSize, hoverSize;
    bool hover;

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
