using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    Vector2 startTile;
    Vector2 endTile;

    private int boardIndex = 0;
    [SerializeField] PathManager pathManager;
    [SerializeField] PlayerBoard playerBoard;
    GamePhaseManager GamePhaseManager;
    UnitPlacement UnitPlacement;

    private bool isInit = false;

    List<GameObject> tilesInPath = new List<GameObject>();
    
    public Color pathStartColor;
    public Color pathEndColor;

    PlayerTileInteraction PlayerTileInteraction;
    DrawPathUI drawPathUI;

    public void Init()
    {
        boardIndex = playerBoard.BoardID;

        PlayerTileInteraction = FindObjectOfType<PlayerTileInteraction>();
        startTile = PlayerBoard.startTile; endTile = PlayerBoard.endTile;
        drawPathUI = FindObjectOfType<DrawPathUI>();
        GamePhaseManager = FindObjectOfType<GamePhaseManager>();
        UnitPlacement = FindObjectOfType<UnitPlacement>();

        isInit = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isInit) return;
        // Only Owner can interact
        if (playerBoard.owner.Value != NetworkManager.Singleton.LocalClientId) { return;}
        // Only let player draw in shop phase
        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase)
        {
            if (tilesInPath.Count > 0)
            {
                ResetPath();
            }

            return;
        }


        if (Input.GetMouseButton(0))
        {
            // Dont let them make path if they are holding a unit
            if (UnitPlacement.GetHeldUnit() != null) { return; }


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

                ResetPath();

                pathManager.SubmitPathToServerRPC(tileIDs, boardIndex);

                

            }
            else
            {
                ResetPath();
            }

        }

        //DrawPath();
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
                pathManager.EnterDrawMode();
                DrawPath();
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

        // Cannot Exceed Tile Limit
        if ((tilesInPath.Count + 1) > pathManager.GetMaxTiles())
        {
            return;
        }


        // Must Be Connected to the last tile we placed        
        if (!TilesAreAdjacent(tileId, tilesInPath[tilesInPath.Count - 1].GetComponent<HexagonTile>().tileId))
        {
            return;
        }

        // Cannot Place next to any other tiles from the path except the last tile
        List<Vector3> AdjacentTilesInPath = new List<Vector3>();
        foreach (Vector3 adjacentTile in HexagonTile.GetAdjacentTiles(tileId))
        {
            if (tilesInPath.Contains(playerBoard.HexagonGrid.GetTileById(adjacentTile)))
            {
                AdjacentTilesInPath.Add(adjacentTile);
            }
        }
        if (AdjacentTilesInPath.Count > 1)
        {
            return;
        }

        tilesInPath.Add(tile);
        DrawPath();

    }

    private void UpdateDrawPathUI()
    {
        int tilesUsed = tilesInPath.Count;
        int tilesLeft = pathManager.GetMaxTiles() - tilesUsed;


        drawPathUI.UpdateDisplay(tilesLeft);

    }

    private void ResetPath()
    {
        pathManager.ExitDrawMode();
        
        foreach (GameObject tile in tilesInPath)
        {
            tile.GetComponent<HexagonTile>().RemoveDrawnPath();
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
            tile.GetComponent<HexagonTile>().SetDrawnPath(color);
            i++;
        }

        UpdateDrawPathUI();
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
