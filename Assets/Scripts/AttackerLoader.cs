using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttackerLoader : NetworkBehaviour
{
    
    public int targetBoard = 0;
    
    public List<GameObject> attackers;
    public PlayerWarband Serverwarband;

    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadAttackers();
        }
    }

    private void LoadAttackers()
    {
        if (!IsServer) return;

        attackers = Serverwarband.OwnedUnits;

        //Find Board
        PlayerBoardsManager.PlayerBoards[targetBoard].AttackerSpawner.UpdateAttackerQueue(attackers);

    }
}
