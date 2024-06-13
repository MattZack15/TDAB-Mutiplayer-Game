using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardNameTag : MonoBehaviour
{
    [SerializeField] PlayerBoard playerBoard;
    [SerializeField] TMP_Text text;
    

    // Update is called once per frame
    void Update()
    {
        text.SetText($"Owner: {playerBoard.owner.Value}\n BoardID: {playerBoard.BoardID}");
    }
}
