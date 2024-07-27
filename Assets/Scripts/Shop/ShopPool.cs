using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopPool : MonoBehaviour
{
    // For managing what Units are avalible in the shop
    [Header("Pools For each Tier")]
    [SerializeField] List<GameObject> Tier1 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier2 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier3 = new List<GameObject>();

    List<List<GameObject>> Pools = new List<List<GameObject>>();

    // What is the chance of rolling a unit from each tier
    // Ex 50, 30, 20
    [Header("Probablities")]
    [SerializeField] List<int> Tier1Probablity = new List<int>();

    // Starting Prob - 50 Teir1 30 Tier2 20 Tier 3

    // Start is called before the first frame update
    void Start()
    {
        Pools.Add(Tier1);
        Pools.Add(Tier2);
        Pools.Add(Tier3);

        RollTierPool(Tier1Probablity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<GameObject> RollTierPool(List<int> Probablities)
    {
        // Pick a Tier based on a list of Probablity
        int roll = UnityEngine.Random.Range(1, 100 + 1);
        //print($"Rolled a {roll}");
        
        int i = 0;
        int previousMax = 0;
        foreach (List<GameObject> pool in Pools)
        {
            int rangeStart = previousMax + 1;
            int rangeEnd = previousMax + Probablities[i];

            //print($"{i}: range ({rangeStart}, {rangeEnd})");

            bool isInRange = roll >= rangeStart && roll <= rangeEnd;

            if (isInRange)
            {
                return pool;
            }

            previousMax = rangeEnd;
            i++;
        }

        print("Failure to create Pool");
        return null;
    }


}
