using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public int GridNumber;
    public Dictionary<Vector2, GameObject> Tiles = new Dictionary<Vector2, GameObject>();


    public GameObject GetTileById(Vector2 tileId)
    {
        if (Tiles.ContainsKey(tileId)) return Tiles[tileId];
        
        return null;
    }


}
