using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PathManager : NetworkBehaviour
{

    // How many tiles am i allowed to use for my path
    public List<int> tilesUnlockedTable = new List<int> { 15, 17, 20, 23, 27, 31};

    //public bool drawMode;

    //[SerializeField] PathCreator PathCreator;
    [SerializeField] PlayerBoard board;
    DrawPathUI drawPathUI;

    List<GameObject> tilesInPath = new List<GameObject>();

    public Vector2[] DefaultPath;

    public Color realPathColor;
    public Color greyedPathColor;


    public void Init()
    {
        drawPathUI = FindObjectOfType<DrawPathUI>();
    }

    public void CreateDefaultPath()
    {        
        SubmitPathToServerRPC(DefaultPath, board.BoardID);
    }

    void DrawPath(Color pathColor)
    {
        int i = 0;
        foreach (GameObject tile in tilesInPath)
        {

            Color color = pathColor;

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

        ExitDrawMode();

        // Update path for all clietns on board
        foreach (Vector2 tileID in tileIDs)
        {
            tilesInPath.Add(board.HexagonGrid.GetTileById(tileID));
        }

        DrawPath(realPathColor);
    }

    public void EnterDrawMode()
    {
        // Grey Out Path
        DrawPath(greyedPathColor);

        // Enable UI
        drawPathUI.gameObject.SetActive(true);


    }

    public void ExitDrawMode()
    {
        DrawPath(realPathColor);
        drawPathUI.gameObject.SetActive(false);
    }

    public int GetMaxTiles()
    {
        // Returns the number of tiles allowed for use in creating a path
        int level = FindObjectOfType<ServerPlayerDataManager>().GetMyPlayerData().level.Value;
        if (level > tilesUnlockedTable.Count) { level = tilesUnlockedTable.Count; }
        return tilesUnlockedTable[level - 1];
    }

}
