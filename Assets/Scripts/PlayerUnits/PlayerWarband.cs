using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarband : MonoBehaviour
{
    public List<GameObject> OwnedUnits = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddUnit(GameObject Unit)
    {
        OwnedUnits.Add(Unit);
    }

    public List<int> GetOwnedUnitIDs()
    {
        List<int> UnitIDs = new List<int>();
        foreach (GameObject unit in OwnedUnits)
        {
            UnitIDs.Add(unit.GetComponent<Unit>().UnitID);
        }

        return UnitIDs;
    }
}
