using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndZone : NetworkBehaviour
{
    // Object that sits at the last tile
    // When attacking units come into it
    // Remove them
    // Send them back to start
    
    [SerializeField] private AttackerSpawner AttackerSpawner;
    [SerializeField] private PlayerBoard PlayerBoard;

    [HideInInspector] public int unitsPassedThroughThisRound = 0;


    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Attacker"))
        {
            GameObject AttackerUnit = other.gameObject;

            AttackerSpawner.SendAttackerToStart(AttackerUnit);

            unitsPassedThroughThisRound += 1;
        }
    }
}
