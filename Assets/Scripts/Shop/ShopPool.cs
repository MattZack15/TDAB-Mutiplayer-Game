using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShopPool : MonoBehaviour
{
    // For managing what Units are avalible in the shop
    [SerializeField] int NumberOfCopiesPerUnit = 6;

    [Header("Pools For each Tier")]
    [SerializeField] List<GameObject> Tier1 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier2 = new List<GameObject>();
    [SerializeField] List<GameObject> Tier3 = new List<GameObject>();

    List<List<GameObject>> PoolsTemplate = new List<List<GameObject>>();
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
    [SerializeField] UnitDex UnitDex;


    // Start is called before the first frame update
    void Awake()
    {
        PoolsTemplate.Add(Tier1);
        PoolsTemplate.Add(Tier2);
        PoolsTemplate.Add(Tier3);

        // Create limited number of each unit
        foreach (List<GameObject> pool in PoolsTemplate)
        {
            List<GameObject> newPool = new List<GameObject>();
            foreach (GameObject unit in pool)
            {
                for (int i = 0; i < NumberOfCopiesPerUnit; i++)
                {
                    newPool.Add(unit);
                }
            }
            Pools.Add(newPool);
        }


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

            // Check if pool is completely empty
            if (pool.Count == 0)
            {
                // In this case ignore rules and pick from tier 1 template pool
                pool = Tier1;
            }

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

    public void AddUnitBackToPool(GameObject Unit)
    {
        // Check if this is a upgraded unit
        if (Unit.GetComponent<Unit>().level > 1)
        {
            // Find Level 1 version
            string UnitID = Unit.GetComponent<Unit>().UnitID.ToString();
            string Level1UnitID = UnitID.Substring(0, UnitID.Length - 3);
            GameObject Level1Unit = UnitDex.Dex[int.Parse(Level1UnitID)];

            // Add 3 copies to the pool
            AddUnitBackToPool(Level1Unit);
            AddUnitBackToPool(Level1Unit);
            AddUnitBackToPool(Level1Unit);
            return;
        }

        // Search Where this unit should go
        int i = 0;
        bool foundLocation = false;
        foreach (List<GameObject> pool in PoolsTemplate)
        {
            if (pool.Contains(Unit)) 
            { 
                foundLocation = true; 
                break; 
            }
            i++;
        }
        // If this is not a unit that can be bought in a shop then don't
        if (!foundLocation)
        {
            print("Unit not found in pools");
            return;
        }

        // Add it to pool
        Pools[i].Add(Unit);
        
    }

}
