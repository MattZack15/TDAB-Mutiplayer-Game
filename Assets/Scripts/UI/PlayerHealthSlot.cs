using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private TMP_Text healthText;

    private int index;
    private ulong playerID;
    PlayerHealthManager PlayerHealthManager;

    public void PopulateSlot(ulong playerID, int playerHealth, int index, PlayerHealthManager PlayerHealthManager)
    {
        this.PlayerHealthManager = PlayerHealthManager;
        this.playerID = playerID;

        playerIDText.text = "Player: " + playerID.ToString();
        healthText.text = playerHealth.ToString() + "/" + PlayerHealthManager.baseMaxHealth.ToString();

        this.index = index;
    }

    private void Update()
    {
        int playerHealth = PlayerHealthManager.playerHps[index];
        healthText.text = playerHealth.ToString() + "/" + PlayerHealthManager.baseMaxHealth.ToString();
    }

    public void OnClick()
    {
        FindObjectOfType<CameraMovement>().LookAtPlayersBoard(playerID);
    }
}
