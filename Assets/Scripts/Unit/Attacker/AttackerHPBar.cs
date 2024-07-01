using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackerHPBar : MonoBehaviour
{
    [SerializeField] private Attacker attacker;

    [SerializeField] private Slider Slider;


    void Update()
    {
        Slider.value = (float)attacker.hp.Value / (float)attacker.maxHp.Value;
    }
}
