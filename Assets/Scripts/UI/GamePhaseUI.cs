using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamePhaseUI : MonoBehaviour
{
    [SerializeField] GamePhaseManager GamePhaseManager;
    [SerializeField] TMP_Text GamePhaseText;
    
    public List<string> phasesText = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GamePhaseText.SetText(phasesText[((int)GamePhaseManager.GamePhase)]);
    }
}
