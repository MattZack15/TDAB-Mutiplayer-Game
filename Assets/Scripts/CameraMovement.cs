using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 10f;
    public float moveRegionRatio = .1f;
    private float lerpTime = .25f;

    private bool cameraLocked;

    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] AudioZoneTracker AudioZoneTracker;

    private void Start()
    {
        cameraLocked = false;
    }

    public void MoveToStartPos()
    {
        SnapToBoard();
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
        transform.position = playerBoardsManager.GetMyBoard().camPos.position;
    }

    public void LookAtPlayersBoard(ulong playerID)
    {
        Vector3 PlayersBoardPos = playerBoardsManager.PlayerBoardTable[playerID].camPos.position;
        AudioZoneTracker.lookingAtBoardID = playerBoardsManager.PlayerBoardTable[playerID].BoardID;
        StartCoroutine(MoveToPos(PlayersBoardPos));
    }

    IEnumerator MoveToPos(Vector3 endPos)
    {
        Vector3 startPos = transform.position;

        float timer = 0;
        while (timer < lerpTime)
        {
            transform.position = Vector3.Slerp(startPos, endPos, timer/lerpTime);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
