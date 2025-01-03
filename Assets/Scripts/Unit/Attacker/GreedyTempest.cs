using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyTempest : MonoBehaviour
{
    public int stacksPerEleSold = 1;
    [Header("Number of Stats per Stack")]
    [SerializeField] private float msBonus = 0f;
    [SerializeField] private int hpBonus = 0;

    public float GetBonusMoveSpeed(int stacks)
    {
        return stacks * msBonus;
    }

    public int GetBonusHealth(int stacks)
    {
        return stacks * hpBonus;
    }

}
