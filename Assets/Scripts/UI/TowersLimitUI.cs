using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowersLimitUI : MonoBehaviour
{
    [SerializeField] GameObject TowersLimitUIObject;
    [SerializeField] TMP_Text text;
    [SerializeField] UnitPlacement UnitPlacement;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    PlayerBoard MyPlayerBoard;

    // Update is called once per frame
    void Update()
    {
        if (!SetPlayerBoard()) { return; }
        
        // Only Show UI When dragging/holding a Tower
        GameObject heldUnit = UnitPlacement.GetHeldUnit();
        if (heldUnit != null && heldUnit.GetComponent<Unit>().isTower())
        {
            if (!TowersLimitUIObject.activeSelf)
            {
                PopulateDisplay();
            }
        }
        else
        {
            TowersLimitUIObject.SetActive(false);
        }
    }

    private void PopulateDisplay()
    {
        TowersLimitUIObject.SetActive(true);
        text.SetText(MyPlayerBoard.GetTowers().Count.ToString() + "/" + MyPlayerBoard.GetTowerLimit().ToString());
    }

    private bool SetPlayerBoard()
    {
        if (MyPlayerBoard) { return true; }

        if (MyPlayerBoard = PlayerBoardsManager.GetMyBoard())
        {
            return true;
        }

        return false;
    }
}
