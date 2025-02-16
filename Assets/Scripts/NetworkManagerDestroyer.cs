using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerDestroyer : MonoBehaviour
{
    private void Awake()
    {
        NetworkManagerDestroyer[] networkmanagers = FindObjectsOfType<NetworkManagerDestroyer>();
        if (networkmanagers.Count() > 1)
        {
            Destroy(gameObject);
        }
    }
}
