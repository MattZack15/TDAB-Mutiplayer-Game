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



    // Update is called once per frame
    void Update()
    {
        DrawPath();
    }

    void DrawPath()
    {
        int i = 0;
        foreach (GameObject tile in tilesInPath)
        {
            float ratio = (float)i / (float)tilesInPath.Count;
            Color color = Color.Lerp(pathStartColor, pathEndColor, ratio);
            tile.GetComponent<HexagonTile>().UpdateNewColor(color);
            i++;
        }
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
        tilesInPath = new List<GameObject>();

        // Update path for all clietns on board
        foreach (Vector2 tileID in tileIDs)
        {
            tilesInPath.Add(board.HexagonGrid.GetTileById(tileID));
        }
        
        
    }

}
