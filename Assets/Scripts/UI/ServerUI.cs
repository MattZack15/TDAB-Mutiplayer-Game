using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerUI : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitObject());
    }

    IEnumerator InitObject()
    {
        while (true)
        {
            if (IsServer)
            {
                yield break;
            }
            if (IsClient && !IsServer)
            {
               gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}
