using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerSpawner: NetworkBehaviour
{
    // Takes in a list of attacking units
    // On round start spawns in those units over time with a delay

    [SerializeField] PlayerBoard board;

    // This list hold an instandce of the attacker and a reference to the original copy
    [HideInInspector]
    private List<(GameObject, GameObject)> attackerQueue = new List<(GameObject, GameObject)>();

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



    public void UpdateAttackerQueue(List<(GameObject, GameObject)> attackers)
    {
        if (!IsServer) { return; }

        foreach ((GameObject, GameObject) attacker in attackers)
        {
            attackerQueue.Add(attacker);
        }
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
            (GameObject, GameObject) nextAttacker = attackerQueue[0];
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

    private void ReleaseAttacker((GameObject, GameObject) attacker)
    {
        if (!IsServer) { return; }

        GameObject instanceAttacker = attacker.Item1;
        GameObject staticAttacker = attacker.Item2;

        instanceAttacker.transform.position = SpawnPos.position;
        instanceAttacker.SetActive(true);
        instanceAttacker.GetComponent<Unit>().SetActive();
        instanceAttacker.GetComponent<NetworkObject>().Spawn();
        instanceAttacker.GetComponent<Attacker>().Init(pathManager.GetBoardPathPoints());

        // Copy Stats
        if (staticAttacker != null)
        {
            Attacker.CopyOverBonusStats(staticAttacker.GetComponent<Attacker>(), instanceAttacker.GetComponent<Attacker>());
        }
        else
        {
            print($"Orignal Attacker was null {instanceAttacker.name}");
        }
        
        // Track it
        AttackersAlive.Add(instanceAttacker);
    }

    public void SendAttackerToStart(GameObject attacker)
    {
        if (!IsServer) { return; }

        attacker.transform.position = SpawnPos.position;
        attacker.GetComponent<AttackerMovement>().SetPath(pathManager.GetBoardPathPoints());
        // Apply static damage everytime we resend attacker to prevent going infinite.
        int staticdamage = Mathf.CeilToInt((float)attacker.GetComponent<Attacker>().maxHp.Value * .05f);
        attacker.GetComponent<Attacker>().TakeHit(staticdamage, null);
    }

    public GameObject PeekNextAttacker(int position)
    {
        // Returns the battle instance of the next attacker
        if (attackerQueue.Count > position)
        {
            return attackerQueue[position].Item1;
        }
        else
        {
            return null;
        }
        
    }

    public void TrackNewAttacker(GameObject attacker)
    {
        // Used when Attackers summon attackers mid battle
        AttackersAlive.Add(attacker);
    }

}
