using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawPathUI : MonoBehaviour
{
    [SerializeField] TMP_Text tilesLeftText;

    public void UpdateDisplay(int tilesLeft)
    {
        tilesLeftText.SetText(tilesLeft.ToString());
    }
}
