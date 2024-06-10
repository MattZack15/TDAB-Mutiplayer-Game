using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDex : MonoBehaviour
{
    
    [SerializeField] public List<int> UnitIDs = new List<int>();
    [SerializeField] public List<GameObject> Units = new List<GameObject>();

    // Contains units and their IDs
    public Dictionary<int, GameObject> Dex = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        if (UnitIDs.Count != Units.Count)
        {
            print("Error: UnitIDs.Count != Units.Count");
        }

        // Build Dict
        int i = 0;
        foreach (int UnitID in UnitIDs) 
        {
            Dex.Add(UnitID, Units[i]);

            i++;
        }
    }
}
