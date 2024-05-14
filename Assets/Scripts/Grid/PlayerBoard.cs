using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBoard : NetworkBehaviour
{
    public NetworkVariable<ulong> owner = new NetworkVariable<ulong>();

    // Set During board generation
    [HideInInspector] public HexagonGrid HexagonGrid;

    public AttackerSpawner AttackerSpawner;

}
