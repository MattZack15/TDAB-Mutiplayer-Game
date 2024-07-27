using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUILookAtCam : MonoBehaviour
{
    // A script for in game world UI elements to always face the camera

    [SerializeField] Transform targetTrasform;
    private Transform cameraTransform;

    // Roates the UI to face the camera
    void Start()
    {
        cameraTransform = FindObjectOfType<Camera>().gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        targetTrasform.LookAt(cameraTransform);

        targetTrasform.eulerAngles = new Vector3(-targetTrasform.localEulerAngles.x, 0f, -targetTrasform.localEulerAngles.z);

    }
}
