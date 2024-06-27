using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class RelayServerUI : MonoBehaviour
{
    [SerializeField] TMP_InputField joinCodeInputField;
    [SerializeField] RelayServer RelayServer;

    [SerializeField] List<GameObject> DisableOnConnection = new List<GameObject>();

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
        }
    }
}
