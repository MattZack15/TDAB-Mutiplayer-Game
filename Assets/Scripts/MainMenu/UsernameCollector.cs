using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UsernameCollector : NetworkBehaviour
{
    // Holds the usernames until the Game is started

    public Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SendUserNameClient(string username)
    {
        PlayerPrefs.SetString("username", username);
        StartCoroutine(WaitToSend(username));
    }

    IEnumerator WaitToSend(string username)
    {
        // Waits to be assigned a client id
        while (NetworkManager.LocalClientId == 0)
        {
            yield return null;
        }

        SendUsernameServerRPC(NetworkManager.LocalClientId, username);
    }

    [Rpc(SendTo.Server)]
    public void SendUsernameServerRPC(ulong clientID, string username)
    {
        playerNames[clientID] = username;
    }


}
