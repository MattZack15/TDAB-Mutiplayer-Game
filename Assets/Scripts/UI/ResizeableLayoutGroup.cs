using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeableLayoutGroup : MonoBehaviour
{
    public Vector2 offsetRatio;
    public Vector2 spacingRatio;

    public bool updateTransform;
    //int childrenCount = 0;

    // Update is called once per frame
    void Update()
    {
        SetChildren();
    }

    private void SetChildren()
    {
        int i = 0;
        foreach (Transform child in transform)
        {

            Rect Rect = GetComponent<RectTransform>().rect;

            print(Rect.width);

            Vector3 offset = Vector3.zero;
            offset += new Vector3(Rect.width * offsetRatio.x, Rect.height * offsetRatio.y, 0f);

            Vector3 spacing = Vector3.zero;
            spacing += new Vector3(Rect.width * spacingRatio.x, Rect.height * spacingRatio.y, 0f);


            child.localPosition = Vector3.zero;
            child.position += offset;
            child.position += spacing * i;

            print($"Pos { child.position}");
            print($"Local Pos {child.localPosition}");


            //child.position = (Vector2)transform.position + new Vector2(0f, spacing.y) * i;

            //print(child.position); 

            i++;
        }

        //childrenCount = transform.childCount;
    }
}
