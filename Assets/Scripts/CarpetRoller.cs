using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CarpetRoller : MonoBehaviour
{
    private Carpet carpet = null;
    private Material material = null;
    private MeshRenderer meshRenderer = null;
    private float carpetLength;

    private float pitch;
    private float startAngle;
    private float anglePerUnit;

    private float unrolledAngle = 0;
    private float rolledAngle = 0;
    public float angle = 0;
    private bool hit;

    private bool rolling = false;
    private bool autoRolling = false;
    private IEnumerator rollingRountine;
    public bool Rolled { get; private set; } = false;

    private Vector3 hitPoint;
    private Vector3 firstMouse;
    private bool isFirstTouch = true;
    private float firstTouchRollingOffset = 0.0f;
    private float lastRollingDifference = 0.0f;
    private float touchTime = 0.0f;
    private const float TOUCH_TIME = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Carpet interactable component");
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.materials[0];

        pitch = material.GetFloat("_Pitch");
        anglePerUnit = material.GetFloat("_AnglePerUnit");
    }

    // Update is called once per frame
    void Update()
    {


        //material?.SetFloat("_UnrolledAngle", angle);
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            //horizontal plane only
            Plane plane = new Plane(transform.up, -transform.position.y);
            hitPoint = new Vector3();

            if (plane.Raycast(ray, out float enter))
            {
                hitPoint = ray.GetPoint(enter);
                Vector3 relativeHitPoint = transform.InverseTransformPoint(hitPoint);
                hit = Utils.Instance.ContainsPoint(carpet.Polygon, new Vector2(relativeHitPoint.x, relativeHitPoint.z) );
                if (hit)
                {
                    if (isFirstTouch)
                    {
                        isFirstTouch = false;
                        firstMouse = hitPoint;
                        Vector3 pivot = (transform.position + transform.TransformPoint(new Vector3(0, 0, carpet.BottomMost.y)));
                        firstTouchRollingOffset = 
                            Vector3.Dot(hitPoint - pivot, transform.forward);
                        //isRollingIn = !Rolled;
                        if (autoRolling)
                        {
                            StopCoroutine(rollingRountine);
                            autoRolling = false;
                        }
                    }


                    Vector3 diff = hitPoint - firstMouse;
                    float rollingDistance = Vector3.Dot(diff, transform.forward);
                    float unrolledLength = firstTouchRollingOffset + rollingDistance;
                    float lerpValue = unrolledLength / carpetLength;
                    angle = Mathf.Lerp(rolledAngle, unrolledAngle, lerpValue);
                    material?.SetFloat("_UnrolledAngle", angle);

                    //if (rollingDistance - lastRollingDifference != 0)
                    //    isRollingIn = (rollingDistance - lastRollingDifference > 0);

                    lastRollingDifference = rollingDistance;
                    rolling = (lerpValue > 0.0f && lerpValue < 1.0f);
                }
            }
        }
    }

    public void OnCarpetMeshRebuilt(Carpet carpet)
    {
        Debug.Log("OnCarpetMeshRebuilt: " + carpet.Polygon.Length+ " "+ carpet.TopMost);
        this.carpet = carpet;
        carpetLength = carpet.TopMost.y - carpet.BottomMost.y;

        unrolledAngle = (carpet.TopMost.y - carpet.BottomMost.y)* anglePerUnit;
        rolledAngle = carpet.BottomMost.y * anglePerUnit;
        material?.SetFloat("_UnrolledAngle", unrolledAngle);
        angle = unrolledAngle;

        startAngle = Mathf.Abs(unrolledAngle) + Mathf.PI * 2f;// material.GetFloat("_StartAngle");
        material.SetFloat("_StartAngle", startAngle);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(hitPoint, new Vector3(0.1f, 0.1f, 0.1f));
    }


}
