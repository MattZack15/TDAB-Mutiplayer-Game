using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerSpawner: NetworkBehaviour
{
    // Takes in a list of attacking units
    // On round start spawns in those units over time with a delay

    [SerializeField] PlayerBoard board;

    [HideInInspector]
    private List<GameObject> attackerQueue = new List<GameObject>();

    Vector2 startTileId;
    Transform SpawnPos;

    [SerializeField] float spawnDelay;
    
    [SerializeField] private PathManager pathManager;

    private List<GameObject> AttackersAlive = new List<GameObject>();

    public bool activeAtttack;

    public void Init()
    {
        if (!IsServer) { return; }

        startTileId = PlayerBoard.startTile;

        SpawnPos = board.HexagonGrid.GetTileById(startTileId).transform;
    }



    public void UpdateAttackerQueue(List<GameObject> attackers)
    {
        if (!IsServer) { return; }

        foreach (GameObject attacker in attackers)
        {
            attackerQueue.Add(attacker);
        }

    }

    public void AddLiveAttacker(GameObject attacker)
    {
        if (!IsServer) { return; }

        attacker.transform.position = SpawnPos.position;
        attacker.GetComponent<AttackerMovement>().SetPath(pathManager.GetBoardPathPoints());
    }

    public void StartSpawner()
    {
        if (!IsServer) { return; }

        activeAtttack = true;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        AttackersAlive = new List<GameObject>();

        while (attackerQueue.Count > 0)
        {
            GameObject nextAttacker = attackerQueue[0];
            attackerQueue.RemoveAt(0);

            ReleaseAttacker(nextAttacker);
            yield return new WaitForSeconds(spawnDelay);
        }

        StartCoroutine(CheckForRoundEnd());
    }

    IEnumerator CheckForRoundEnd()
    {
        bool attackIsActive = true;
        
        while (attackIsActive)
        {

            
            // If list is empty we are done
            if (AttackersAlive.Count == 0)
            {
                if (attackerQueue.Count == 0)
                {
                    attackIsActive = false;
                    break;
                }
            }
            // if all are null we are done
            bool allNull = true;
            foreach (GameObject attacker in AttackersAlive)
            {
                if (attacker != null)
                {
                    allNull = false;
                    break;
                }
            }
            if (allNull)
            {
                if (attackerQueue.Count == 0)
                {
                    attackIsActive = false;
                }
            }

            
            yield return null;
        }

        //print("Attack is Over");
        activeAtttack = false;

    }

    private void ReleaseAttacker(GameObject attacker)
    {
        if (!IsServer) { return; }

        attacker.transform.position = SpawnPos.position;
        attacker.SetActive(true);
        attacker.GetComponent<Unit>().SetActive();
        attacker.GetComponent<NetworkObject>().Spawn();
        attacker.GetComponent<Attacker>().Init(pathManager.GetBoardPathPoints());


        // Track it
        AttackersAlive.Add(attacker);
    }

    public GameObject PeekNextAttacker()
    {
        if (attackerQueue.Count > 0)
        {
            return attackerQueue[0];
        }
        else
        {
            return null;
        }
        
    }

}
