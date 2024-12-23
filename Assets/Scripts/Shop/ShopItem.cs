using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{

    private int unitID;
    private Unit unit;

    private Shop shop;
    public Image picture;
    public int shopIndex;

    UnitToolTip unitToolTip;

    [SerializeField] Transform TribeLayoutGroup;
    [SerializeField] GameObject TribeLabelPrefab;
    [SerializeField] Image UnitTypeIcon;

    [SerializeField] Sprite TowerIconSprite;
    [SerializeField] Sprite AttackerIconSprite;

    // Start is called before the first frame update
    void Start()
    {
        shop = FindObjectOfType<Shop>();
        unitToolTip = FindObjectOfType<UnitToolTip>();
    }


    public void OnItemClick()
    {
        shop.TryBuyUnit(unitID, shopIndex);
    }

    public void PopulateDisplay(GameObject Unit, int UnitID, int shopIndex)
    {
        this.shopIndex = shopIndex;
        unitID = UnitID;
        unit = Unit.GetComponent<Unit>();
        picture.sprite = unit.UnitIcon;

        foreach (Tribe tribe in unit.tribes)
        {
            TribeLabel tribeLabel = Instantiate(TribeLabelPrefab, TribeLayoutGroup).GetComponent<TribeLabel>();
            tribeLabel.SetDisplay(tribe);
        }

        if (unit.isTower())
        {
            UnitTypeIcon.sprite = TowerIconSprite;
        }
        else
        {
            UnitTypeIcon.sprite = AttackerIconSprite;
        }

    }

    public void OnItemHover()
    {
        unitToolTip.SetDisplay(unit);
    }
}
