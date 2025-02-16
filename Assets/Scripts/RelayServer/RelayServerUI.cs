using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

#pragma warning disable 4014 // Disables Warning For Calling async task in non awaited function

public class RelayServerUI : NetworkBehaviour
{
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] RelayServer RelayServer;

    [SerializeField] UsernameCollector UsernameCollector;
    [SerializeField] TMP_InputField usernameIF;
    [SerializeField] List<GameObject> DisableOnConnection = new List<GameObject>();
    [SerializeField] Button hostbutton;
    [SerializeField] Button joinbutton;
    [SerializeField] GameObject StartGameButtonObj;
    [SerializeField] GameObject ConnectedPlayersObj;
    [SerializeField] GameObject WaitForHostTextObj;


    private void Start()
    {
        // Load User Name
        string username = PlayerPrefs.GetString("username");
        usernameIF.SetTextWithoutNotify(username);
    }

    private void Update()
    {
        // If there is no username in the field then disable buttons
        if (usernameIF.text == "")
        {
            hostbutton.interactable = false;
            joinbutton.interactable = false;
        }
        else
        {
            hostbutton.interactable = true;
            joinbutton.interactable = true;
        }
    }

    public void StartHostButton()
    {
        PlayerPrefs.SetString("username", usernameIF.text);
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
            UsernameCollector.SendUsernameServerRPC(NetworkManager.Singleton.LocalClientId, usernameIF.text);
            StartGameButtonObj.SetActive(true);
            ConnectedPlayersObj.SetActive(true);
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
            UsernameCollector.SendUserNameClient(usernameIF.text);
            print("Sent Name");
        }
    }

    public void StartGameButton()
    {

        if (!IsServer) { return; }
        NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
