using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpTile : MonoBehaviour
{
    [SerializeField] GameObject helpText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float hoverDistance;
    bool hover;
    void FixedUpdate()
    {
        if (hover != (CursorObj.trfm.position - transform.position).sqrMagnitude < hoverDistance * hoverDistance)
        {
            hover = !hover;

            helpText.SetActive(hover);
        }
    }
}
