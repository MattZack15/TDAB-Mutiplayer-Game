using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxGrid : MonoBehaviour
{
    public HexagonGridGenerator HexagonGridGenerator;

    public Color color1 = Color.white;
    public Color color2 = Color.white;
    public Color color3 = Color.white;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSkyBox();
    }


    private void GenerateSkyBox()
    {
        Transform SkyBox = HexagonGridGenerator.SpawnHexagonGrid(0, Vector2.zero, new Vector2(20, 70));
        SkyBox.position = new Vector3(-200f, -40f, -300f);
        SkyBox.localScale = new Vector3(1f, 1f, 1f) * 20f;

        int i = 0;
        foreach (Vector2 key in SkyBox.gameObject.GetComponent<HexagonGrid>().Tiles.Keys)
        {
            HexagonTile tile = SkyBox.gameObject.GetComponent<HexagonGrid>().GetTileById(key).GetComponent<HexagonTile>();

            tile.gameObject.GetComponent<Collider>().enabled = false;


            if (key.y % 3 == 0)
            {
                tile.SetBaseColo(color1);
            }
            else if (key.y % 3 == 1)
            {
                tile.SetBaseColo(color2);
            }
            else
            {
                tile.SetBaseColo(color3);
            }


            i++;
        }
    }
}
