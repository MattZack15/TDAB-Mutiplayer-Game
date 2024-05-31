using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUIOverlay : MonoBehaviour
{
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

        targetTrasform.localEulerAngles = new Vector3 (-targetTrasform.localEulerAngles.x, 0f, -targetTrasform.localEulerAngles.z);

    }
}
