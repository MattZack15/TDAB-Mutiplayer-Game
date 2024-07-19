using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PathManager : NetworkBehaviour
{

    // How many tiles am i allowed to use for my path
    public NetworkVariable<int> tilesUnlocked = new NetworkVariable<int>();
    static int baseTilesUnlocked = 15;

    //[SerializeField] PathCreator PathCreator;
    [SerializeField] PlayerBoard board;

    List<GameObject> tilesInPath = new List<GameObject>();

    public Vector2[] DefaultPath;

    public Color realPathColor;


    public void Init()
    {
        if (!IsServer) { return; }
        tilesUnlocked.Value = baseTilesUnlocked;
    }

    public void CreateDefaultPath()
    {        
        SubmitPathToServerRPC(DefaultPath, board.BoardID);
    }

    void DrawPath()
    {
        int i = 0;
        foreach (GameObject tile in tilesInPath)
        {

            Color color = realPathColor;

            float rand = Random.Range(-0.09f, 0.1f);

            if (rand >= 0)
            {
                color = Color.Lerp(color, Color.white, rand);
            }
            else
            {
                color = Color.Lerp(color, Color.black, -1f*rand);
            }

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
