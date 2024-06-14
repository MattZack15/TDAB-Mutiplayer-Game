using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePhaseManager;

public class ShopUI : MonoBehaviour
{

    //Hide Shop During Battle Not During Shop
    [SerializeField] GamePhaseManager gamePhaseManager;

    [SerializeField] GameObject shopUIObj;

    // Update is called once per frame
    void Update()
    {
        bool hide = !(gamePhaseManager.GamePhase == GamePhases.ShopPhase);

        if (hide)
        {
            shopUIObj.SetActive(false);
        }
        else
        {
            shopUIObj.SetActive(true);
        }
    }
}
