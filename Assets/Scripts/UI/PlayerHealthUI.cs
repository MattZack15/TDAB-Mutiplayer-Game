using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject PlayerSlot;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField]
    private Transform layoutGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            DrawPlayerHealthList();
        }
    }

    private void DrawPlayerHealthList()
    {
        int i = 0;
        while (i < playerHealth.playerIds.Count)
        {
            GameObject newPlayerSlot = Instantiate(PlayerSlot, layoutGroup);
            newPlayerSlot.GetComponent<PlayerHealthSlot>().PopulateSlot(playerHealth.playerIds[i], playerHealth.playerHps[i]);
            i++;
        }
    }
}
