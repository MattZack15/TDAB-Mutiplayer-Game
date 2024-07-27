using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitDex : MonoBehaviour
{

    [HideInInspector] public List<int> UnitIDs = new List<int>();
    [SerializeField] public List<GameObject> Units = new List<GameObject>();

    // Contains units and their IDs
    public Dictionary<int, GameObject> Dex = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        UnitIDs = new List<int>();
        foreach (GameObject Unit in Units)
        {
            int UnitID = Unit.GetComponent<Unit>().UnitID;

            if (UnitIDs.Contains(UnitID))
            {
                print($"Error Duplicate UnitID \n{UnitID}, {Unit.GetComponent<Unit>().UnitName}");
            }

            UnitIDs.Add(UnitID);
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
