using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTileInteraction : MonoBehaviour
{
    public Camera gameCamera;

    private GameObject prevHighlightedTile;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TileHoverIndicator();
    }

    private void TileHoverIndicator()
    {
        // Highlighhts tile mouse is over
        GameObject tile = GetSelectedTile();

        // Unhighlight prev Tile
        if (prevHighlightedTile != null && prevHighlightedTile != tile)
        {
            prevHighlightedTile.GetComponent<HexagonTile>().Unhighlight();
        }

        if (tile == null) { return; }

        tile.GetComponent<HexagonTile>().Highlight();
        prevHighlightedTile = tile;

    }

    public GameObject GetSelectedTile()
    {
        // Gets object of tile mouse is at
        GameObject selectedTile = null;
        
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.GetComponent<HexagonTile>() != null) 
            {
                // Check if we hit a hex tile
                selectedTile = hitInfo.collider.gameObject;
            }
        }
        
        return selectedTile;
    }
}
