using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] Shop shop;
    [SerializeField] TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        text.SetText(shop.coins.ToString());
    }
}
