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
        // Return The the tile under the mouse
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray);

        if (hits.Length == 0) { return null; }

        GameObject HoveredTile = null;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<HexagonTile>() != null)
            {
                HoveredTile = hit.collider.gameObject;
                break;
            }
        }

        return HoveredTile;
    }
}
