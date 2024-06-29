using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TribeLabel : MonoBehaviour
{

    [SerializeField] TMP_Text nameLabel;
    [SerializeField] Image tribeIcon;
    [SerializeField] Image background;

    Tribe tribe;

    public void SetDisplay(Tribe tribe)
    {
        this.tribe = tribe;

        nameLabel.text = tribe.tribeName;
        tribeIcon.sprite = tribe.tribeIcon;
        background.color = tribe.tribeColor;
    }
}
