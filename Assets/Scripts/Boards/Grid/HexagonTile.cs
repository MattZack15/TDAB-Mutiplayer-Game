using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : MonoBehaviour
{

    private static Color highlightColor = Color.white;
    // (x: X Gird Pos, y:, Y Grid Pos, z: Which grid it belongs to)
    public Vector3 tileId;

    private Renderer Renderer;
    private Color currentColor;
    private Color orignalColor;

    public bool occupied {  private set; get; }
    // Object that is occuping this Tile
    public GameObject inhabitor;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        orignalColor = Renderer.material.color;
        currentColor = orignalColor;
    }

    public void UpdateNewColor(Color newColor)
    {
        currentColor = newColor;
        Renderer.material.color = newColor;
    }

    public void ResetColor()
    {
        currentColor = orignalColor;
        Renderer.material.color = orignalColor;
    }

    public void Highlight()
    {
        Renderer.material.color = highlightColor;
    }

    public void Unhighlight()
    {
        Renderer.material.color = currentColor;
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
}
