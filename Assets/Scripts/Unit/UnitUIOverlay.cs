using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIOverlay : MonoBehaviour
{
    [SerializeField] GameObject LevelIcon;

    [HideInInspector] public GameObject parentObj;

    // Roates the UI to face the camera
    void Start()
    {
        parentObj = transform.parent.parent.gameObject;

        // Level Icon
        if (parentObj.GetComponent<Unit>().level > 1)
        {
            LevelIcon.SetActive(true);
        }
        else
        {
            LevelIcon.SetActive(false);
        }
    }


}
