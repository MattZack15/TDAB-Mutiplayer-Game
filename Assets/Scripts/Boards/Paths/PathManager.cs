using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PathManager : NetworkBehaviour
{

    //[SerializeField] PathCreator PathCreator;
    [SerializeField] PlayerBoard board;

    List<GameObject> tilesInPath = new List<GameObject>();


    public Color pathStartColor;
    public Color pathEndColor;


    void DrawPath()
    {
        int i = 0;
        foreach (GameObject tile in tilesInPath)
        {
            float ratio = (float)i / (float)tilesInPath.Count;
            Color color = Color.Lerp(pathStartColor, pathEndColor, ratio);
            tile.GetComponent<HexagonTile>().SetRealPath(color);


            i++;
        }
    }

    void ResetPath()
    {
        foreach (GameObject tile in tilesInPath)
        {
            tile.GetComponent<HexagonTile>().RemoveRealPath();
        }

        tilesInPath = new List<GameObject>();
    }

    private List<Vector3> GetPathPoints()
    {
        // Returns a list of world postions for each tile in the path
        List<Vector3> points = new List<Vector3>();

        foreach (GameObject tile in tilesInPath)
        {
            points.Add(tile.transform.position);
        }

        return points;

    }

    public List<Vector3> GetBoardPathPoints()
    {
        List<Vector3> points = new List<Vector3>();

        if (tilesInPath == null || tilesInPath.Count == 0)
        {
            return points;
        }

        return GetPathPoints();
    }

    [Rpc(SendTo.Server)]
    public void SubmitPathToServerRPC(Vector2[] tileIDs, int boardNumber)
    {
        // Server Should Verify

        RecivePathClientRPC(tileIDs, boardNumber);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RecivePathClientRPC(Vector2[] tileIDs, int boardNumber)
    {
        ResetPath();

        // Update path for all clietns on board
        foreach (Vector2 tileID in tileIDs)
        {
            tilesInPath.Add(board.HexagonGrid.GetTileById(tileID));
        }

        DrawPath();
    }

}
