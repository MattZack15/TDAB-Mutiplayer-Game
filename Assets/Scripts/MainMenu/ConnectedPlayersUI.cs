using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectedPlayersUI : MonoBehaviour
{
    [SerializeField] UsernameCollector UsernameCollector;
    [SerializeField] GameObject nameTagPrefab;
    [SerializeField] GameObject layoutGroup;
    List<GameObject> nametags = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (UsernameCollector.playerNames.Count != nametags.Count)
        {
            // Delete Everything
            foreach (GameObject nametag in nametags) 
            {
                Destroy(nametag);
            }
            nametags = new List<GameObject>();

            // Remake Everything
            foreach (ulong clientID in UsernameCollector.playerNames.Keys)
            {
                GameObject newNameTag = Instantiate(nameTagPrefab, layoutGroup.transform);
                newNameTag.GetComponent<TMP_Text>().SetText(UsernameCollector.playerNames[clientID]);
                nametags.Add(newNameTag);
            }
        }
    }
}
