using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HexagonGridGenerator : MonoBehaviour
{

    [SerializeField] private GameObject HexagonGridContainer;
    
    public Material dMaterial;

    public float hexagonWidth = 1.5f;
    private float r;
    private float rc;
    const float root3 = 1.7320508f;

    public static Vector2 playerBoardSize = new Vector2(7, 18);
    public static Vector2 sideBoardSize = new Vector2(1, 18);

    public Color color1;
    public Color color2;
    public Color color3;

    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        DefineHexagon();
    }

    public Transform SpawnHexagonGrid(int gridIndex, Vector2 gridsOffset, Vector2 boardSize)
    {
        // Create Template Tile
        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(HexagonTile));

        MeshFilter MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = mesh;
        MeshCollider MeshCollider = gameObject.GetComponent<MeshCollider>();
        MeshCollider.sharedMesh = mesh;
        gameObject.GetComponent<Renderer>().material = dMaterial;

        // Setup HexGrid Parent Object
        GameObject HexagonGrid = Instantiate(HexagonGridContainer);
        HexagonGrid.GetComponent<HexagonGrid>().GridNumber = gridIndex;
        HexagonGrid.name = $"Hexagon Grid {gridIndex}";
        
        // Spawn in all tiles
        SpawnTiles(gameObject, gridIndex, HexagonGrid, boardSize);


        HexagonGrid.transform.position += new Vector3(gridsOffset.x, 0f, -gridsOffset.y);

        // Remove template tile
        Destroy(gameObject);

        return HexagonGrid.transform;
    }

    private void DefineHexagon()
    {
        // Computes the hexagon Mesh
        // Starts with hexagonWidth and creates our hexagon Mesh and define all other values for hexagon
        
        ComputeHexagonValues();

        Vector3[] vertices = new Vector3[8];
        Vector2[] uv = new Vector2[8];
        int[] triangles = new int[18];

        // Define Vertices
        vertices[0] = new Vector3(-r, 0, 0);
        vertices[3] = new Vector3(r, 0, 0);

        vertices[1] = new Vector3(-(r / 2), root3 * r / 2, 0);
        vertices[2] = new Vector3((r / 2), root3 * r / 2, 0);
        vertices[4] = new Vector3((r / 2), root3 * -r / 2, 0);
        vertices[5] = new Vector3((-r / 2), root3 * -r / 2, 0);

        vertices[6] = new Vector3(0, 0, 0);

        int i = 0;
        while (i < 8)
        {
            uv[i] = vertices[i];
            i++;
        }

        // Define Triangles
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 6;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 6;

        triangles[6] = 6;
        triangles[7] = 2;
        triangles[8] = 3;

        triangles[9] = 6;
        triangles[10] = 3;
        triangles[11] = 4;

        triangles[12] = 6;
        triangles[13] = 4;
        triangles[14] = 5;

        triangles[15] = 6;
        triangles[16] = 5;
        triangles[17] = 0;

        mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void ComputeHexagonValues()
    {
        // Uses Only Hexagon Width
        r = hexagonWidth / 2;
        rc = (1f / 2f) * root3 * r;
    }

    private Transform SpawnTiles(GameObject hexagon, int gridIndex, GameObject HexagonGrid, Vector2 boardSize)
    {
        // Spawns all the tiles and returns the parent object
        Transform GridParent = HexagonGrid.transform;
        HexagonGrid HexagonGridComponent = HexagonGrid.GetComponent<HexagonGrid>();

        int xLength = (int)boardSize.x;
        int yLength = (int)boardSize.y;
        

        int j = 0;
        while (j < yLength)
        {
            float xOffset = ((j % 2) * 1.5f*r);
            float yOffset = (j * rc);

            int i = 0;
            while (i < xLength)
            {
                GameObject newHexagon = Instantiate(hexagon);
                newHexagon.transform.position += new Vector3((1.5f * i * hexagonWidth) + xOffset, yOffset, 0f);

                ColorTile(newHexagon.GetComponent<HexagonTile>(), j);

                newHexagon.transform.SetParent(GridParent);

                // Tile coordinates
                int xCoord = i*2 + (j % 2);

                newHexagon.name = $"HexagonTile ({xCoord}, {j}) G:{gridIndex}";
                newHexagon.GetComponent<HexagonTile>().tileId = new Vector3(xCoord, j, gridIndex);
                HexagonGridComponent.Tiles.Add(new Vector2(xCoord, j), newHexagon);

                i++;
            }
            j++;
        }

        GridParent.localEulerAngles = new Vector3(90f, 0f, 0f);

        return GridParent;
    }

    private void ColorTile(HexagonTile HexagonTile, int y)
    {
        int type = y % 3;
        Color color = Color.white;

        if (type == 0)
        {
            color = color1;
        }
        else if (type == 1)
        {
            color = color2;
        }
        else if (type == 2)
        {
            color = color3;
        }
        HexagonTile.SetBaseColo(color);
    }
}
