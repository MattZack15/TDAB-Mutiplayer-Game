using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    Vector2 startTile;
    Vector2 endTile;

    private int boardIndex = 0;
    [SerializeField] PathManager pathManager;
    [SerializeField] PlayerBoard playerBoard;

    List<GameObject> tilesInPath = new List<GameObject>();
    
    public Color pathStartColor;
    public Color pathEndColor;

    PlayerTileInteraction PlayerTileInteraction;

    public void Init()
    {
        boardIndex = playerBoard.BoardID;

        PlayerTileInteraction = FindObjectOfType<PlayerTileInteraction>();
        startTile = PlayerBoard.startTile; endTile = PlayerBoard.endTile;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
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

        if (Input.GetKeyDown(KeyCode.S))
        {

            List<Vector3> PathTileIds = new List<Vector3>();
            foreach (GameObject tile in tilesInPath)
            {
                PathTileIds.Add(tile.GetComponent<HexagonTile>().tileId);
            }
            
            
            if (ValidatePath(PathTileIds))
            {

                // And start and end tile to path
                tilesInPath.Insert(0, playerBoard.HexagonGrid.GetTileById(startTile));
                tilesInPath.Add(playerBoard.HexagonGrid.GetTileById(endTile));

                // Create Array for sending to server
                Vector2[] tileIDs = new Vector2[tilesInPath.Count];
                for (int i = 0; i < tilesInPath.Count; i++)
                {
                    Vector3 globalTileID = tilesInPath[i].GetComponent<HexagonTile>().tileId;
                    tileIDs[i] = new Vector2(globalTileID.x, globalTileID.y);
                }


                pathManager.SubmitPathToServerRPC(tileIDs, boardIndex);

                ResetPath();

            }
            else
            {
                ResetPath();
            }

        }

        DrawPath();
    }

    private void TryAddToPath(GameObject tile)
    {
        Vector3 tileId = tile.GetComponent<HexagonTile>().tileId;

        // First tile only cares about connecting to start tile
        if (tilesInPath.Count == 0)
        {
            if (TilesAreAdjacent(tileId, new Vector3(startTile.x, startTile.y, boardIndex)))
            {
                tilesInPath.Add(tile);
            }
            return;
        }

        // Cannot be the start tile or end tile
        if (tileId == new Vector3(startTile.x, startTile.y, boardIndex) || tileId == new Vector3(endTile.x, endTile.y, boardIndex))
        {
            return;
        }


        // if already in path dont add
        if (tilesInPath.Contains(tile))
        {
            return;
        }

        // Must Be Connected to the last tile we placed        
        if (!TilesAreAdjacent(tileId, tilesInPath[tilesInPath.Count - 1].GetComponent<HexagonTile>().tileId))
        {
            return;
        }

        tilesInPath.Add(tile);

    }

    private void ResetPath()
    {
        foreach (GameObject tile in tilesInPath)
        {
            tile.GetComponent<HexagonTile>().ResetColor();
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


    private bool TilesAreAdjacent(Vector3 tileId1, Vector3 tileId2)
    {
        List<Vector3> AdjacentTiles = HexagonTile.GetAdjacentTiles(tileId1);
        

        if (AdjacentTiles.Contains(tileId2))
        {
            return true;
        }
        return false;
    }

    private bool ValidatePath(List<Vector3> tileIds)
    {
        // A valid path goes from the start tile to end tile

        if (tileIds == null || tileIds.Count == 0) return false;

        // Last Tile connects to end tile
        Vector3 lastTileId = tileIds[tileIds.Count - 1];
        Vector3 endTileId = new Vector3(endTile.x, endTile.y, boardIndex);

        if (!TilesAreAdjacent(lastTileId, endTileId))
        {
            print("Path not connected to end");
            return false;
        }
        
        // first Tile connects to start tile
        Vector3 firstTileId = tileIds[0];
        Vector3 startTileId = new Vector3(startTile.x, startTile.y, boardIndex);

        if (!TilesAreAdjacent(firstTileId, startTileId))
        {
            print("Path not connected to start");
            return false;
        }

        // Check if each point is connected


        return true;
    }
}
