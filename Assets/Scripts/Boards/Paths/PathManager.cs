using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{

    [SerializeField] PathCreator PathCreator;

    List<GameObject> tilesInPath = new List<GameObject>();


    public Color pathStartColor;
    public Color pathEndColor;


    public void UpdatePath(List<GameObject> tilesInPath)
    {
        // Called by Path Creator after a valid path is made
        foreach (GameObject tile in tilesInPath)
        {
            this.tilesInPath.Add(tile);
        }

    }

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
}
