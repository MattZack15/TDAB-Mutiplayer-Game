using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZoneTracker : MonoBehaviour
{
    // This keeps track of which board the camera is looking at
    // so that some sound effects can only be heard when they are relavant

    public int lookingAtBoardID = 0;

    private void Update()
    {
        print(lookingAtBoardID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("AudioZone")) { return; }

        lookingAtBoardID = other.gameObject.transform.parent.GetComponent<PlayerBoard>().BoardID;
    }
}
