using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private TMP_Text healthText;

    public void PopulateSlot(ulong playerID, int playerHealth)
    {
        playerIDText.text = playerID.ToString();
        healthText.text = playerHealth.ToString();
    }
}
