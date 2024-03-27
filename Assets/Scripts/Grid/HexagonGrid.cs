using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HexagonGrid : MonoBehaviour
{

    public Material dMaterial;

    public float hexagonWidth = 1.5f;
    private float r;
    private float rc;
    const float root3 = 1.7320508f;

    public Vector2 boardSize;

    public Color color1;
    public Color color2;
    public Color color3;

    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        DefineHexagon();
    }

    public Transform SpawnHexagonGrid(int gridIndex, Vector2 gridsOffset)
    {

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(HexagonTile));

        MeshFilter MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = mesh;
        MeshCollider MeshCollider = gameObject.GetComponent<MeshCollider>();
        MeshCollider.sharedMesh = mesh;
        gameObject.GetComponent<Renderer>().material = dMaterial;


        Transform Grid = SpawnGrid(gameObject, gridIndex);
        Grid.position += new Vector3(gridsOffset.x, 0f, -gridsOffset.y);

        Destroy(gameObject);

        return Grid;
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

    private Transform SpawnGrid(GameObject hexagon, int gridIndex)
    {
        // Spawns all the tiles and returns the parent object
        Transform GridParent = new GameObject().transform;
        GridParent.name = $"Hexagon Grid {gridIndex}";

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

                ColorTile(newHexagon.GetComponent<Renderer>(), j);

                newHexagon.transform.SetParent(GridParent);

                // Tile coordinates
                int xCoord = i*2 + (j % 2);

                newHexagon.name = $"HexagonTile ({xCoord}, {j}) G:{gridIndex}";
                newHexagon.GetComponent<HexagonTile>().tileId = new Vector3(xCoord, j, gridIndex);

                i++;
            }
            j++;
        }

        GridParent.localEulerAngles = new Vector3(90f, 0f, 0f);

        return GridParent;
    }

    private void ColorTile(Renderer TileRenderer, int y)
    {
        int color = y % 3;

        if (color == 0)
        {
            TileRenderer.material.color = color1;
        }
        else if (color == 1)
        {
            TileRenderer.material.color = color2;
        }
        else if (color == 2)
        {
            TileRenderer.material.color = color3;
        }

    }
}
