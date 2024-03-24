using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    
    List<GameObject> tilesInPath = new List<GameObject>();
    public Color pathColor;

    public PlayerTileInteraction PlayerTileInteraction;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Add new tile
            GameObject selectedTile = PlayerTileInteraction.GetSelectedTile();
            if (selectedTile)
            {
                tilesInPath.Add(selectedTile);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset Path
            ResetPath();
        }

        DrawPath();
    }

    private void ResetPath()
    {
        foreach (GameObject tile in tilesInPath)
        {
            tile.GetComponent<HexagonTile>().UpdateNewColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }

        tilesInPath = new List<GameObject>();
    }

    private void DrawPath()
    {
        foreach (GameObject tile in tilesInPath)
        {
            tile.GetComponent<HexagonTile>().UpdateNewColor(pathColor);
        }
    }
}
