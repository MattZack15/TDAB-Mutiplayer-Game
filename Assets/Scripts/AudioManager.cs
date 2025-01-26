using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public List<Sound> Sounds;
    public Dictionary<string, AudioSource> SoundTable = new Dictionary<string, AudioSource>();

    // Singleton instance.
    public static AudioManager Instance = null;

    ClientRpcParams clientRpcParams;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Sound sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;

            SoundTable.Add(sound.name, sound.source);
        }
        
        clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] {  }
            }
        };
    }

    // Play a single clip through the sound effects source.
    public void Play(string clipName)
    {
        if (!SoundTable.ContainsKey(clipName))
        {
            print("No such sound " + clipName);
            return;
        }
        SoundTable[clipName].Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayForEveryoneRPC(string clipName)
    {
        Play(clipName);
    }

    public void PlayOnClient(string clipName, ulong clientID)
    {

        clientRpcParams.Send.TargetClientIds = new ulong[] { clientID };
        PlayOnClientRpc(clipName, clientRpcParams);
    }
    [ClientRpc]
    private void PlayOnClientRpc(string clipName, ClientRpcParams clientRpcParams = default)
    {
        Play(clipName);
    }

}
