using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerIDText;
    [SerializeField] private TMP_Text healthText;

    private ulong playerID;
    private ServerPlayerData playerData;

    public void PopulateSlot(ServerPlayerData playerData)
    {
        this.playerData = playerData;
        this.playerID = playerData.clientID.Value;

        playerIDText.text = playerData.username.Value.ToString();
        healthText.text = playerData.health.Value.ToString();
    }

    private void Update()
    {
        int playerHealth = playerData.health.Value;
        healthText.text = playerHealth.ToString();
        playerIDText.text = playerData.username.Value.ToString();

        if (playerData.clientID.Value != playerID)
        {
            playerID = playerData.clientID.Value;
        }
    }

    public void OnClick()
    {
        FindObjectOfType<CameraMovement>().LookAtPlayersBoard(playerID);
    }
}
