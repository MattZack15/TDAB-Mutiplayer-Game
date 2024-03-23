using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{

    public Material dMaterial;

    float hexagonWidth = 1.5f;
    float r;
    float rc;
    float root3 = 1.7320508f;


    // Start is called before the first frame update
    void Start()
    {
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
            uv[i] = vertices[i] * -1;
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



        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(HexagonTile));
        gameObject.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter MeshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter.mesh = mesh;
        MeshCollider MeshCollider = gameObject.GetComponent<MeshCollider>();
        MeshCollider.sharedMesh = mesh;
        gameObject.GetComponent<Renderer>().material = dMaterial;

        SpawnGrid(gameObject);

        Destroy(gameObject);

    }

    private void ComputeHexagonValues()
    {
        // Uses Only Hexagon Width
        r = hexagonWidth / 2;
        rc = (1f / 2f) * root3 * r;
    }

    private void SpawnGrid(GameObject hexagon)
    {
        Transform GridParent = new GameObject().transform;
        GridParent.name = "Hexagon Grid";

        int gridSize = 15;
        int yLength = 2 * gridSize;
        int xLength = (int)((float)gridSize/1.5f);

        // Spawn 5
        int j = 0;
        while (j < yLength)
        {
            float xOffset = (j % 2) * 1.5f*r;
            float yOffset = j * rc;

            int i = 0;
            while (i < xLength)
            {
                GameObject newHexagon = Instantiate(hexagon);
                newHexagon.transform.position += new Vector3((1.5f * i * hexagonWidth) + xOffset, yOffset, 0f);

                newHexagon.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                newHexagon.transform.SetParent(GridParent);

                newHexagon.name = $"HexagonTile {i} {j}";

                i++;
            }
            j++;
        }

        GridParent.localEulerAngles = new Vector3(90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
