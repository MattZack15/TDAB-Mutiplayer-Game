using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VFXManager : NetworkBehaviour
{
    [SerializeField] GameObject RebornVFX;

    // Responsible for creating VFX locally
    // Start is called before the first frame update

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnRebornVFXRPC(ulong networkObjectId)
    {
        NetworkObject networkObject;
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject);

        if (networkObject == null)
        {
            print("Object Not Found");
        }

        Instantiate(RebornVFX, networkObject.transform);
    }

}
