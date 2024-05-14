using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public int GridNumber;
    public Dictionary<Vector2, GameObject> Tiles = new Dictionary<Vector2, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetTileById(Vector2 tileId)
    {
        return Tiles[tileId];
    }


}
