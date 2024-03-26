using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    
    List<GameObject> tilesInPath = new List<GameObject>();
    
    public Color pathStartColor;
    public Color pathEndColor;

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
                TryAddToPath(selectedTile);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset Path
            ResetPath();
        }

        DrawPath();
    }

    private void TryAddToPath(GameObject tile)
    {
        // If path empty them add no matter what
        if(tilesInPath.Count == 0)
        {
            tilesInPath.Add(tile);
            return;
        }

        // if already in path dont add
        if (tilesInPath.Contains(tile))
        {
            return;
        }

        // Must Be Connected to the last tile we placed
        List<Vector3>  AdjacentTiles = HexagonTile.GetAdjacentTiles(tilesInPath[tilesInPath.Count-1].GetComponent<HexagonTile>().tileId);
        
        if (!AdjacentTiles.Contains(tile.GetComponent<HexagonTile>().tileId))
        {
            return;
        }

        tilesInPath.Add(tile);

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
        int i = 0;
        foreach (GameObject tile in tilesInPath)
        {
            float ratio = (float)i/ (float)tilesInPath.Count;
            Color color = Color.Lerp(pathStartColor, pathEndColor, ratio);
            tile.GetComponent<HexagonTile>().UpdateNewColor(color);
            i++;
        }
    }

    public List<Vector3> GetPathPoints()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (GameObject tile in tilesInPath)
        {
            points.Add(tile.transform.position);
        }

        return points;

    }
}
