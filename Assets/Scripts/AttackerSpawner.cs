using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerSpawner : NetworkBehaviour
{
    // Takes in a list of attacking units
    // On round start spawns in those units over time with a delay

    [SerializeField] PlayerBoard board;
    
    List<GameObject> attackerQueue = new List<GameObject>();

    Vector2 startTileId;
    Transform SpawnPos;

    [SerializeField] float spawnDelay;
    
    void Start()
    {
        startTileId = PlayerBoard.startTile;

        SpawnPos = board.HexagonGrid.GetTileById(startTileId).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartSpawner();
        }
    }

    public void UpdateAttackerQueue(List<GameObject> attackers)
    {
        if (!IsServer) { return; }

        foreach (GameObject attacker in attackers)
        {
            attackerQueue.Add(attacker);
        }

    }

    public void StartSpawner()
    {
        if (!IsServer) { return; }
        
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        print("starting spawns");

        while(attackerQueue.Count > 0)
        {
            GameObject nextAttacker = attackerQueue[0];
            attackerQueue.RemoveAt(0);

            SpawnAttacker(nextAttacker);
            yield return new WaitForSeconds(spawnDelay);
        }
        
    }

    private void SpawnAttacker(GameObject attacker)
    {
        if (!IsServer) { return; }

        GameObject newAttacker = Instantiate(attacker, SpawnPos.position, Quaternion.identity);
        newAttacker.GetComponent<NetworkObject>().Spawn();

        // Init attacker
        //newAttacker.GetComponent<AttackerMovement>().SetPath();
    }


}
