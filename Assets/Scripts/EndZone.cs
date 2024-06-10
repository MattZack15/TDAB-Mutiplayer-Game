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
    // Remove HP
    [SerializeField] private AttackerSpawner AttackerSpawner;
    [SerializeField] private PlayerBoard PlayerBoard;


    private PlayerHealthManager PlayerHealthManager;


    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) { return; }

        PlayerHealthManager = FindObjectOfType<PlayerHealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Attacker"))
        {
            GameObject AttackerUnit = other.gameObject;

            AttackerSpawner.AddLiveAttacker(AttackerUnit);

            PlayerHealthManager.OnAttackerReachEnd(PlayerBoard);


            //AttackerUnit.SetActive(false);

            //AttackerUnit.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
