using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebornVFX : MonoBehaviour
{
    // A bunch of voxels orbiting around the Unit
    
    [SerializeField] private GameObject voxel;
    [SerializeField] private Material material;

    [SerializeField] private float radius;
    [SerializeField] private float orbitTime;
    [SerializeField] private int density;
    [SerializeField] private float floatHeight;
    [SerializeField] private float floatTime;
    //[SerializeField] private float offsetTime;

    private float timer;
    List<GameObject> particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = new List<GameObject>();

        for (int i = 0; i < density; i++)
        {

            float angle = (360f / density) * i;

            float xPos = radius * Mathf.Cos(angle * (3.14f/180f));
            float yPos = radius * Mathf.Sin(angle * (3.14f / 180f));


            Transform newVoxelTransform = Instantiate(voxel, transform).transform;
            newVoxelTransform.localPosition = new Vector3 (xPos, .5f, yPos);

            newVoxelTransform.gameObject.GetComponent<MeshRenderer>().material = material;

            StartCoroutine(MoveVoxel(newVoxelTransform, i));

            particles.Add(newVoxelTransform.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float timeOffset = timer % orbitTime;
        float angleOffset = (timeOffset / orbitTime) * 360f;

        int i = 0;
        foreach (GameObject particle in particles)
        {
            float angle = (360f / density) * i;
            angle += angleOffset;

            float xPos = radius * Mathf.Cos(angle * (3.14f / 180f));
            float yPos = radius * Mathf.Sin(angle * (3.14f / 180f));

            particle.transform.localPosition = new Vector3(xPos, particle.transform.localPosition.y, yPos);

            i++;
        }

    }

    IEnumerator MoveVoxel(Transform voxelTransform, float offset)
    {
        float maxY = floatHeight/2 + voxelTransform.localPosition.y;
        float minY = -floatHeight / 2 + voxelTransform.localPosition.y;

        // Random Rotaion
        voxelTransform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        float offsetTime = 0f;
        if ((float)offset < (density / 2f))
        {
            offsetTime = floatTime * (offset / (density - 1));
        }
        else
        {
            offsetTime = floatTime - (floatTime * (offset / (density - 1)));
        }
        

        yield return MoveToYPos(voxelTransform, minY, maxY, offsetTime);

        while (voxelTransform)
        {
            yield return MoveToYPos(voxelTransform, maxY, minY, 0f);
            yield return MoveToYPos(voxelTransform, minY, maxY, 0f);
        }
    }

    IEnumerator MoveToYPos(Transform voxelTransform, float originY, float targetY, float currentTime)
    {
        float timer = currentTime;
        while (Mathf.Abs(voxelTransform.localPosition.y - targetY) != 0f) 
        {

            float yPos = Mathf.SmoothStep(originY, targetY, timer / floatTime);

            voxelTransform.localPosition = new Vector3(voxelTransform.localPosition.x, yPos, voxelTransform.localPosition.z);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
