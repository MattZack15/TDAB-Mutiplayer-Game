using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : MonoBehaviour
{

    private static Color highlightColor = Color.white;
    int tileId;

    private Renderer Renderer;
    private Color normalColor;

    // Server Only
    public bool occupied;
    
    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        normalColor = Renderer.material.color;
    }
    
    public void Highlight()
    {
        Renderer.material.color = highlightColor;
    }

    public void Unhighlight()
    {
        Renderer.material.color = normalColor;
    }

}
