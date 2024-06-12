using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 10f;
    public float moveRegionRatio = .1f;

    private bool cameraLocked;

    Vector3 OriginalPos = Vector3.zero;

    private void Start()
    {
        cameraLocked = true;
        OriginalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            cameraLocked = !cameraLocked;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SnapToBoard();
        }
        
        
        Vector2 InputVector = Vector2.zero;
        float moveRegionSizeY = moveRegionRatio * Screen.height;
        float moveRegionSizeX = moveRegionRatio * Screen.width;


        // Up/Down
        if (Input.mousePosition.y >= Screen.height - moveRegionSizeY)
        {
            InputVector += Vector2.up;
        }
        if (Input.mousePosition.y < 0 + moveRegionSizeY)
        {
            InputVector += Vector2.down;
        }

        // X
        if (Input.mousePosition.x >= Screen.width - moveRegionSizeX)
        {
            InputVector += Vector2.right;
        }
        if (Input.mousePosition.x < 0 + moveRegionSizeX)
        {
            InputVector += Vector2.left;
        }

        InputVector = InputVector.normalized;

        //Move cam
        if (!cameraLocked)
        {
            float deltaZ = InputVector.y * camSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + deltaZ);

            float deltaX = InputVector.x * camSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x + deltaX, transform.position.y, transform.position.z);
        }

    }

    private void SnapToBoard()
    {
        transform.position = OriginalPos;
    }
}
