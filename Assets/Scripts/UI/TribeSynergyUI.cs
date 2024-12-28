using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TribeSynergyUI : MonoBehaviour
{
    [SerializeField] TMP_Text ArcaneText;
    [SerializeField] TMP_Text EldritchText;
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    
    PlayerBoard MyPlayerBoard;
    TribeSynergy TribeSynergy;

    // Update is called once per frame
    void Update()
    {
        if (!SetPlayerBoard()) { return; }

        ArcaneText.SetText(TribeSynergy.ArcaneCount.Value.ToString() + "/3");
        EldritchText.SetText(TribeSynergy.EldritchCount.Value.ToString() + "/4");
    }

    private bool SetPlayerBoard()
    {
        if (MyPlayerBoard) { return true; }

        if (MyPlayerBoard = PlayerBoardsManager.GetMyBoard())
        {
            TribeSynergy = MyPlayerBoard.gameObject.GetComponent<TribeSynergy>();
            return true;
        }

        return false;
    }
}
