using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackerHPBar : MonoBehaviour
{
    [SerializeField] private Attacker attacker;

    [SerializeField] private Slider Slider;
    [SerializeField] GridLayoutGroup healthTickLayoutGroup;
    [SerializeField] GameObject healthTickPrefab;
    [SerializeField] int hpPerTick = 100;
    List<GameObject> healthTicks = new List<GameObject>();
    [SerializeField] float maxSpacing = 1.23f;

    private int oldMaxHp = 0;

    void Update()
    {
        int currentMaxHp = attacker.maxHp.Value;
        Slider.value = (float)attacker.hp.Value / (float)currentMaxHp;

        if (oldMaxHp != currentMaxHp)
        {
            GenerateHealthbarTicks(currentMaxHp);
            oldMaxHp = currentMaxHp;
        }
    }

    private void GenerateHealthbarTicks(int maxHp)
    {
        int ticksNeeded = 1 + (maxHp / hpPerTick);
        // Threshold where the whole bar would be black ticks
        if (maxHp > 3200)
        {
            ticksNeeded = 0;
        }

        // If we have less than we need
        while (healthTicks.Count < ticksNeeded)
        {
            GameObject newHpTick = Instantiate(healthTickPrefab, healthTickLayoutGroup.transform);
            healthTicks.Add(newHpTick);
        }

        // If we have more than we need
        while (healthTicks.Count > ticksNeeded)
        {
            GameObject tick = healthTicks[0];
            healthTicks.RemoveAt(0);
            Destroy(tick);
        }

        // Set LayoutGroup Settings
        float totalSpacing = maxSpacing - (ticksNeeded * healthTickLayoutGroup.cellSize.x);
        float xSpacing = totalSpacing / ((float)maxHp / (float)hpPerTick);
        healthTickLayoutGroup.spacing = new Vector2(xSpacing, healthTickLayoutGroup.spacing.y);
    }
}
