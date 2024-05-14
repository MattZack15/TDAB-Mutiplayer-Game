using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private TMP_Text healthText;

    private int index;
    PlayerHealthManager PlayerHealthManager;

    public void PopulateSlot(ulong playerID, int playerHealth, int index, PlayerHealthManager PlayerHealthManager)
    {
        this.PlayerHealthManager = PlayerHealthManager;


        playerIDText.text = "Player ID: " + playerID.ToString();
        healthText.text = playerHealth.ToString() + "/" + PlayerHealthManager.baseMaxHealth.ToString();

        this.index = index;
    }

    private void Update()
    {
        int playerHealth = PlayerHealthManager.playerHps[index];
        healthText.text = playerHealth.ToString() + "/" + PlayerHealthManager.baseMaxHealth.ToString();
    }
}
