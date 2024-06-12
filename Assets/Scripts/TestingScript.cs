using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingScript : NetworkBehaviour
{
    public bool spawnObj;
    public bool destroyObj;

    public GameObject prefab;

    private GameObject spawnedObj;


    // Update is called once per frame
    void Update()
    {
        if(!IsServer) { return; }
        
        if (spawnObj)
        {
            GameObject newPrefab = Instantiate(prefab);
            newPrefab.GetComponent<NetworkObject>().Spawn();

            spawnedObj = newPrefab;

            spawnObj = false;
        }

        if (destroyObj)
        {
            spawnedObj.GetComponent<NetworkObject>().Despawn();

            destroyObj = false;
        }
    }
}
