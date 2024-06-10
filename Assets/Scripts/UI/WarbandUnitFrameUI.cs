using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarbandUnitFrameUI : MonoBehaviour
{
    public UnityEngine.UI.Image picture;

    public GameObject MyUnit;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateDisplay(GameObject Unit)
    {
        picture.sprite = Unit.GetComponent<Unit>().UnitIcon;
        MyUnit = Unit;
    }
}
