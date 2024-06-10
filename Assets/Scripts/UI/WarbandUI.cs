using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarbandUI : MonoBehaviour
{
    // Displays Players Warband

    [SerializeField] PlayerWarband PlayerWarband;
    [SerializeField] Transform WarbandUnitFrameUILayoutGroup;
    [SerializeField] private GameObject WarbandUnitFrameUIPrefab;
    
    List<GameObject> WarbandUnitFrameUIObjs = new List<GameObject>();

    int lastWarbandCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastWarbandCount != PlayerWarband.OwnedUnits.Count)
        {
            SetDisplay();
            lastWarbandCount = PlayerWarband.OwnedUnits.Count;
        }
    }

    private void SetDisplay()
    {
        // Reset
        foreach (GameObject WarbandUnitFrameUIObj in WarbandUnitFrameUIObjs)
        {
            Destroy(WarbandUnitFrameUIObj);
        }
        WarbandUnitFrameUIObjs = new List<GameObject>();

        foreach(GameObject Unit in PlayerWarband.OwnedUnits)
        {
            GameObject newWarbandUnitFrame = Instantiate(WarbandUnitFrameUIPrefab, WarbandUnitFrameUILayoutGroup);
            newWarbandUnitFrame.GetComponent<WarbandUnitFrameUI>().PopulateDisplay(Unit);

            WarbandUnitFrameUIObjs.Add(newWarbandUnitFrame);
        }
    }
}
