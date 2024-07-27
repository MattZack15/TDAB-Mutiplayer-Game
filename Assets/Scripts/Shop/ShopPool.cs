using System.Collections.Generic;
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
    void Awake()
    {
        Pools.Add(Tier1);
        Pools.Add(Tier2);
        Pools.Add(Tier3);

    }

    public int[] GenerateShopSelection(ulong playerID, int amountOfItems)
    {
        int[] ShopItems = new int[amountOfItems];

        // Default To Tier1 Shop
        List<int> Probablities = Tier1Probablity;

        for (int i = 0; i < amountOfItems; i++)
        {
            List<GameObject> pool = RollTierPool(Probablities);
            int unitID = pool[Random.Range(0, pool.Count)].GetComponent<Unit>().UnitID;

            ShopItems[i] = unitID;
        }

        return ShopItems;
    }

    private List<GameObject> RollTierPool(List<int> Probablities)
    {
        // Pick a Tier based on a list of Probablity
        
        int maxRoll = 0;
        foreach (int weight in Probablities)
        {
            maxRoll += weight;
        }
        
        int roll = Random.Range(1, maxRoll + 1);
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
