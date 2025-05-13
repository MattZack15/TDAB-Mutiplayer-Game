using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class EndGameRankings : MonoBehaviour
{
    [SerializeField] Transform layoutGroupTrans;
    [SerializeField] GameObject container;
    [SerializeField] GameObject placementTagPrefab;
    [SerializeField] List<Color> placementColors = new List<Color>();
    [SerializeField] GameObject BattleDamageSummaryContainer;


    // Start is called before the first frame update
    void Start()
    {
        container.SetActive(false);
    }

    public void DisplayRankings(FixedString128Bytes[] playerNames)
    {
        StartCoroutine(WaitForDamageSummaryToEnd(playerNames));
    }

    IEnumerator WaitForDamageSummaryToEnd(FixedString128Bytes[] playerNames)
    {
        // Wait for this damage recap to disappear
        // Plays without a recap will see results instantly
        while (BattleDamageSummaryContainer.activeSelf)
        {
            yield return null;
        }
        
        container.SetActive(true);

        int placementNumber = 1;
        for (int i = playerNames.Length - 1; i >= 0; i--)
        {
            GameObject newplacementTag = Instantiate(placementTagPrefab, layoutGroupTrans);
            SetPlacementTagDisplay(newplacementTag, placementNumber, playerNames[i].ToString());

            placementNumber++;
        }
    }

    private void SetPlacementTagDisplay(GameObject placementTag, int placementNumber, string playerName)
    {
        TMP_Text placmentNumberText = placementTag.transform.GetChild(0).GetComponent<TMP_Text>();
        placmentNumberText.text = placementNumberString(placementNumber);
        placmentNumberText.color = placementColor(placementNumber);

        placementTag.transform.GetChild(1).GetComponent<TMP_Text>().SetText(playerName);
    }

    private string placementNumberString(int placementNumber)
    {
        if (placementNumber == 1)
        {
            return "1st";
        }
        if (placementNumber == 2)
        {
            return "2nd";
        }
        if (placementNumber == 3)
        {
            return "3rd";
        }
        return $"{placementNumber}th";
    }

    private Color placementColor(int placementNumber)
    {
        if (placementNumber <= 3)
        {
            return placementColors[placementNumber - 1];
        }

        return placementColors[3];
    }

    public void CloseLeaderBoard()
    {
        container.SetActive(false);
    }
}
