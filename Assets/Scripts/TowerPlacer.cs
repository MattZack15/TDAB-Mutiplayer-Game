using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class TowerPlacer : NetworkBehaviour
{
    public GameObject tower;
    public GameObject attacker;
    [SerializeField] private PlayerTileInteraction playerTileInteraction;

    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {


                GameObject SelectedTile = playerTileInteraction.GetSelectedTile();

                if (SelectedTile == null){ return;}

                Vector3 tilePos = SelectedTile.transform.position;


                if (Input.GetMouseButtonDown(0))
                {

                    SpawnTowerServerRpc(tilePos, 0);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    SpawnTowerServerRpc(tilePos, 1);
                }
            }
        }

    }

    [Rpc(SendTo.Server)]
    private void SpawnTowerServerRpc(Vector3 spawnPos, int obj)
    {
        GameObject wantedObj;
        if(obj == 0)
        {
            wantedObj = tower;
        }
        else
        {
            wantedObj = attacker;
        }


        GameObject newObj = Instantiate(wantedObj, spawnPos, Quaternion.identity);
        newObj.GetComponent<NetworkObject>().Spawn();

    }
}
