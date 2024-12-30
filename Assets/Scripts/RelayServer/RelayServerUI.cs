using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Netcode;

#pragma warning disable 4014 // Disables Warning For Calling async task in non awaited function

public class RelayServerUI : NetworkBehaviour
{
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] RelayServer RelayServer;

    [SerializeField] List<GameObject> DisableOnConnection = new List<GameObject>();
    [SerializeField] GameObject StartGameButtonObj;
    [SerializeField] GameObject WaitForHostTextObj;

    public void StartHostButton()
    {
        StartHost();
    }

    public async Task StartHost()
    {
        string joinCode = await RelayServer.StartHostWithRelay();

        if (joinCode != null && joinCode != "")
        {
            // Success
            joinCodeInputField.text = joinCode;

            foreach (GameObject go in DisableOnConnection)
            {
                go.SetActive(false);
            }

            print(joinCode);
            StartGameButtonObj.SetActive(true);
        }
        else
        {
            print("Fail To Start Host");
        }

        return;
    }

    public void JoinServerButton()
    {
        if (joinCodeInputField.text != "")
        {
            StartClient(joinCodeInputField.text);
        }
    }

    public async Task StartClient(string joinCode)
    {
        bool connection = await RelayServer.StartClientWithRelay(joinCode);

        if (connection)
        {
            foreach (GameObject go in DisableOnConnection)
            {
                go.SetActive(false);
            }

            print("Client Connected");
            WaitForHostTextObj.SetActive(true);
        }
    }

    public void StartGameButton()
    {

        if (!IsServer) { return; }
        NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
