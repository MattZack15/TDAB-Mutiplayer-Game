using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TribeSynergyToolTip : MonoBehaviour
{

    string ArcaneDescription = "Arcane Towers have a change to double cast their attacks.\n\n";
    string EldritchDescription = "Eldritch Towers, after killing a unit, have a chance to trigger their Bloodlust.\n\n";
    [SerializeField] Color ArcaneColor;
    [SerializeField] Color EldritchColor;

    [SerializeField] GameObject tooltipObj;
    [SerializeField] TMP_Text toolTipText;
    [SerializeField] Image background;

    // Start is called before the first frame update
    void Start()
    {
        List<float> effectchances = ArcaneTowerAttribute.effectChances;
        int level = 1;
        foreach (float chance in effectchances)
        {
            ArcaneDescription += $"Lv {level}: {chance}%\n";
            level += 1;
        }
        
        effectchances = EldritchTowerAttribute.effectChances;
        level = 1;
        foreach (float chance in effectchances)
        {
            EldritchDescription += $"Lv {level}: {chance}%\n";
            level += 1;
        }
    }

    public void OnHoverToolTip(string tribe)
    {
        tooltipObj.SetActive(true);
        if (tribe == "Arcane")
        {
            toolTipText.text = ArcaneDescription;
            background.color = ArcaneColor;
        }
        else if (tribe == "Eldritch")
        {
            toolTipText.text = EldritchDescription;
            background.color = EldritchColor;
        }
    }

    public void OnHoverExit()
    {
        tooltipObj.SetActive(false);
    }


}
