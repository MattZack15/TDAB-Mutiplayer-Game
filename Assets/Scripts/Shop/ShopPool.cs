using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShopPool : MonoBehaviour
{
    // For managing what Units are avalible in the shop
    [Header("Pools For each Tier")]
    [SerializeField] List<GameObject> Tier1 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier2 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier3 = new List<GameObject>();

    List<List<GameObject>> Pools = new List<List<GameObject>>();

    // What is the chance of rolling a unit from each tier at each level
    // Ex 50, 30, 20
    [Header("Probablities per Level")]
    [SerializeField] List<int> Level1Probabilities = new List<int>();
    [SerializeField] List<int> Level2Probabilities = new List<int>();
    [SerializeField] List<int> Level3Probabilities = new List<int>();

    List<List<int>> PoolProbabilities = new List<List<int>>();

    [Header("Serialize")]
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;


    // Start is called before the first frame update
    void Awake()
    {
        Pools.Add(Tier1);
        Pools.Add(Tier2);
        Pools.Add(Tier3);

        // Create limited number of each unit
        int quantity = 6;
        List<List<GameObject>> newPools = new List<List<GameObject>>();
        foreach (List<GameObject> pool in Pools)
        {
            List<GameObject> newPool = new List<GameObject>();
            foreach (GameObject unit in pool)
            {
                for (int i = 0; i < quantity; i++)
                {
                    newPool.Add(unit);
                }
            }
            newPools.Add(newPool);
        }
        Pools = newPools;


        PoolProbabilities.Add(Level1Probabilities);
        PoolProbabilities.Add(Level2Probabilities);
        PoolProbabilities.Add(Level3Probabilities);

    }

    public int[] GenerateShopSelection(ulong playerID, int amountOfItems)
    {
        int[] ShopItems = new int[amountOfItems];

        // Find Probablities
        int level = ServerPlayerDataManager.GetPlayerData(playerID).level.Value;
        if (level > PoolProbabilities.Count)
        {
            print("Level too high");
            level = PoolProbabilities.Count;
        }
        List<int> Probabilities = PoolProbabilities[level-1];

        // Get a random Unit x times where x is amountOfItems in shop
        for (int i = 0; i < amountOfItems; i++)
        {
            // Pick a Pool
            List<GameObject> pool = RollTierPool(Probabilities);
            // Pick a Unit from that Pool
            GameObject unit = pool[Random.Range(0, pool.Count)];

            int unitID = unit.GetComponent<Unit>().UnitID;


            ShopItems[i] = unitID;
        }

        return ShopItems;
    }

    private List<GameObject> RollTierPool(List<int> Probabilities)
    {
        // Pick a Tier based on a list of Probablity

        // Adds all of the weights
        int maxRoll = 0;
        foreach (int weight in Probabilities)
        {
            maxRoll += weight;
        }

        // then rolls a number
        int roll = Random.Range(1, maxRoll + 1);
        //print($"Rolled a {roll}");
        
        // Finds which tier the rolled number falls into
        int i = 0;
        int previousMax = 0;
        foreach (List<GameObject> pool in Pools)
        {
            int rangeStart = previousMax + 1;
            int rangeEnd = previousMax + Probabilities[i];

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

    public void RemoveUnitFromPool(GameObject Unit)
    {
        // Called When buying a Unit to reduce the number available in the pool
        // Search Pool
        foreach (List<GameObject> pool in Pools)
        {
            if (pool.Contains(Unit))
            {
                pool.Remove(Unit);
                return;
            }
        }

        print("Unit not found in pools");

    }

}
