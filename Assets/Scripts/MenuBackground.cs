using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private HexagonGridGenerator HexagonGridGenerator;

    [SerializeField] Transform cameraTrans;
    [SerializeField] Vector3 camStartPos;
    [SerializeField] Vector3 camEndPos;
    [SerializeField] float camTransTime = 10f;

    [SerializeField] List<Color> ColorRotation = new List<Color>();

    public float transTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // Create Grid
        Transform HexagonGridTransform = HexagonGridGenerator.SpawnHexagonGrid(1, Vector2.zero, new Vector2(20, 60));
        Dictionary<Vector2, GameObject> Tiles = HexagonGridTransform.gameObject.GetComponent<HexagonGrid>().Tiles;
        
        foreach (Vector2 tileID in Tiles.Keys) 
        {
            float offset = 2*tileID.x + tileID.y;
            offset = offset / 3.5f;
            StartCoroutine(ColorAnimation(Tiles[tileID].GetComponent<HexagonTile>(), offset));
        }

        StartCoroutine(CameraAnim(camStartPos, camEndPos));
    }

    IEnumerator CameraAnim(Vector3 startPos, Vector3 endPos)
    {
        float timer = 0f;
        float progress = 0f;
        while (progress < 1f)
        {
            timer += Time.deltaTime;
            progress = timer / camTransTime;
            float ease = easeInOutSine(progress);
            cameraTrans.position = Vector3.Lerp(startPos, endPos, ease);
            
            yield return null;
        }

        StartCoroutine(CameraAnim(endPos, startPos));
    }


    private float easeInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI* x) - 1) / 2;
    }

IEnumerator ColorAnimation(HexagonTile tile, float offset)
    {
        float timer = offset;

        while (true)
        {
            timer += Time.deltaTime;
            int colorPos = (int)(timer / transTime);

            while (colorPos >= ColorRotation.Count - 1)
            {
                colorPos -= (ColorRotation.Count - 1);
                timer -= (transTime * (ColorRotation.Count - 1));
            }

            Color color1 = ColorRotation[colorPos];
            Color color2 = ColorRotation[colorPos+1];

            float progress = (timer - (colorPos * transTime)) / transTime;

            Color CalcColor = Color.Lerp(color1, color2, progress);
            tile.SetBaseColo(CalcColor);

            yield return null;
        }
    }


}
