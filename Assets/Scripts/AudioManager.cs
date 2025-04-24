using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{

    public List<Sound> Sounds;
    public Dictionary<string, AudioSource> SoundTable = new Dictionary<string, AudioSource>();

    [SerializeField] AudioZoneTracker AudioZoneTracker;

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
        //DontDestroyOnLoad(gameObject);

        if (IsServer && !GetComponent<NetworkObject>().IsSpawned)
        {
            GetComponent<NetworkObject>().Spawn();
        }
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
    public void Play(string clipName, bool pitchVariance = false)
    {
        if (!SoundTable.ContainsKey(clipName))
        {
            print("No such sound " + clipName);
            return;
        }
        SoundTable[clipName].pitch = 1.0f;
        if (pitchVariance)
        {
            SoundTable[clipName].pitch = Random.Range(.80f, 1.2f);
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

    // Everyone who is looking at this board should hear this sound effect
    [Rpc(SendTo.ClientsAndHost)]
    public void PlayForBoardRPC(string clipName, int boardID, bool pitchVariance = false)
    {
        // Check if we are looking at this board
        if (AudioZoneTracker.lookingAtBoardID == boardID)
        {
            Play(clipName, pitchVariance);
        }
    }

    // Called Locally of the client to only play a sound if the player is looking at that board
    public void PlayOnBoard(string clipName, int boardID, bool pitchVariance = false)
    {
        // Check if we are looking at this board
        if (AudioZoneTracker.lookingAtBoardID == boardID)
        {
            Play(clipName, pitchVariance);
        }
    }
}
