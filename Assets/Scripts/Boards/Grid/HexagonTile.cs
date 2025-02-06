using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : MonoBehaviour
{

  
    // (x: X Gird Pos, y:, Y Grid Pos, z: Which grid it belongs to)
    public Vector3 tileId;

    private Renderer Renderer;
    private Color orignalColor;

    public bool occupied {  private set; get; }
    // Object that is occuping this Tile
    public GameObject inhabitor;

    // States
    private bool highlighted;
    private bool isDrawnPath;
    private bool isRealPath;
    private bool isSideBoard;
    private bool isStartEndTile;

    private static Color highlightColor = Color.white;
    private Color drawnPathColor;
    private Color realPathColor;
    private Color sideBoardColor;
    private Color startEndBoardColor;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();

        int hexagonGridLayer = LayerMask.NameToLayer("HexagonGrid");
        gameObject.layer = hexagonGridLayer;
    }

    // Start is called before the first frame update
    public void SetBaseColo(Color color)
    {
        orignalColor = color;
        ComputeColor();
    }

    public void SetDrawnPath(Color color)
    {
        isDrawnPath = true;
        drawnPathColor = color;

        ComputeColor();
    }
    public void RemoveDrawnPath()
    {
        isDrawnPath = false;
        drawnPathColor = new Color();

        ComputeColor();
    }
    public void SetRealPath(Color color)
    {
        isRealPath = true;
        realPathColor = color;

        ComputeColor();
    }
    public void RemoveRealPath()
    {
        isRealPath = false;
        realPathColor = new Color();

        ComputeColor();
    }
    public void SetSideBoard(Color color)
    {
        isSideBoard = true;
        sideBoardColor = color;

        ComputeColor();
    }

    public void SetStartEndTile(Color color)
    {
        isStartEndTile = true;
        startEndBoardColor = color;

        ComputeColor();
    }

    public void Highlight()
    {
        highlighted = true;

        ComputeColor();
    }

    public void Unhighlight()
    {
        highlighted = false;

        ComputeColor();
    }

    private void ComputeColor()
    {
        // Listed By Prio
        
        if (highlighted)
        {
            DisplayColor(highlightColor);
            return;
        }

        if (isSideBoard)
        {
            DisplayColor(sideBoardColor);
            return;
        }

        if (isStartEndTile)
        {
            DisplayColor(startEndBoardColor);
            return;
        }

        if (isDrawnPath)
        {
            DisplayColor(drawnPathColor);
            return;
        }

        if (isRealPath)
        {
            DisplayColor(realPathColor);
            return;
        }

        DisplayColor(orignalColor);
    }

    private void DisplayColor(Color color)
    {
        Renderer.material.color = color;
    }

    public static List<Vector3> GetAdjacentTiles(Vector3 originTile)
    {
        List<Vector3> tiles = new List<Vector3>();

        // Up and Down
        tiles.Add(new Vector3(originTile.x, originTile.y + 2, originTile.z));
        tiles.Add(new Vector3(originTile.x, originTile.y - 2, originTile.z));
        // Right Side
        tiles.Add(new Vector3(originTile.x+1, originTile.y + 1, originTile.z));
        tiles.Add(new Vector3(originTile.x+1, originTile.y - 1, originTile.z));
        // Left
        tiles.Add(new Vector3(originTile.x - 1, originTile.y + 1, originTile.z));
        tiles.Add(new Vector3(originTile.x - 1, originTile.y - 1, originTile.z));

        return tiles;
    }

    public void SetOccupied(GameObject inhabitor)
    {
        this.inhabitor = inhabitor;
        occupied = true;
    }

    public void SetUnoccupied()
    {
        inhabitor = null;
        occupied = false;
    }

    public bool isOccupied()
    {
        return inhabitor != null;
    }
}
