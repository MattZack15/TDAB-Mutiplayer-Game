using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamePhaseUI : MonoBehaviour
{
    [SerializeField] GamePhaseManager GamePhaseManager;
    [SerializeField] TMP_Text GamePhaseText;
    [SerializeField] TMP_Text TimerText;
    
    public List<string> phasesText = new List<string>();


    // Update is called once per frame
    void Update()
    {
        int phaseIndex = GamePhaseManager.GamePhase.Value;
        GamePhaseText.SetText(phasesText[phaseIndex]);

        // Shop Phase
        if (phaseIndex == 0)
        {
            TimerText.gameObject.SetActive(true);
            int turnTimeRemaining = (int)GamePhaseManager.turnTimer.Value;
            TimerText.SetText(turnTimeRemaining.ToString());
        }
        else
        {
            // Battle Phase
            TimerText.gameObject.SetActive(false);
        }


    }
}
