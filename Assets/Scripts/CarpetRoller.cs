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
    private float currentAngle = 0;
    private bool hitting;

    private bool rolling = false;
    private bool autoRolling = false;
    private IEnumerator rollingRountine;
    private bool isRollingIn = false;
    [SerializeField] public bool Rolled { private set; get; } = false;

    private Vector3 hitPoint;
    private Vector3 firstMouse;
    private bool isFirstTouch = true;
    private float firstTouchRollingOffset = 0.0f;


    private float lastRollingDistance = 0.0f;
    private float mouseButtonDownTime = 0.0f;
    private const float MOUSE_DOWN_TIME = 0.1f;

    [SerializeField] bool shouldRoll = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Carpet interactable component");
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.materials[0];

        pitch = material.GetFloat("_Pitch");
        anglePerUnit = material.GetFloat("_AnglePerUnit");
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(hitPoint, new Vector3(0.1f, 0.1f, 0.1f));
    }
    // Update is called once per frame
    void Update()
    {
        if (shouldRoll&&!autoRolling)
        {
            shouldRoll = false;
            rollingRountine = roll(0.5f, !Rolled);
            StartCoroutine(rollingRountine);
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (hitting)
            {
                bool isMouseClick = mouseButtonDownTime < MOUSE_DOWN_TIME;

                //touching and release the mouse
                isFirstTouch = true;
                if (isMouseClick)
                {
                    rollingRountine = roll(0.5f, !Rolled);
                    StartCoroutine(rollingRountine);
                }
                else
                {
                    if (!autoRolling && rolling)
                    {
                        rollingRountine = roll(0.5f, isRollingIn);
                        StartCoroutine(rollingRountine);
                    }
                }
            }
            mouseButtonDownTime = 0;
        }
        if (Input.GetMouseButton(0))
        {
            bool isMouseDrag = mouseButtonDownTime > MOUSE_DOWN_TIME;
            if (mouseButtonDownTime < MOUSE_DOWN_TIME)
                mouseButtonDownTime += Time.deltaTime;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //horizontal plane only
            Plane plane = new Plane(transform.up, -transform.position.y);
            if (plane.Raycast(ray, out float enter))
            {
                hitPoint = ray.GetPoint(enter);
                Vector3 relativeHitPoint = transform.InverseTransformPoint(hitPoint);
                hitting = checkMouseHitPoint(new Vector2(relativeHitPoint.x, relativeHitPoint.z));

                if (isMouseDrag)
                {
                    if (hitting)
                    {
                        if (isFirstTouch)
                        {
                            isFirstTouch = false;
                            firstMouse = hitPoint;
                            Vector3 pivot = (transform.TransformPoint(new Vector3(0, 0, carpet.BottomMost.y)));
                            firstTouchRollingOffset = Vector3.Dot(hitPoint - pivot, transform.forward);
                            isRollingIn = !Rolled;

                            if (autoRolling)
                            {
                                StopCoroutine(rollingRountine);
                                autoRolling = false;
                            }
                            Debug.Log("Touching");
                        }


                        float rollingDistance = Vector3.Dot(hitPoint - firstMouse, transform.forward);
                        float unrolledLength = firstTouchRollingOffset + rollingDistance;
                        float lerpValue = unrolledLength / carpetLength;

                        setUnrolledAngle(Mathf.Lerp(rolledAngle, unrolledAngle, lerpValue));
                        if (rollingDistance - lastRollingDistance != 0)
                            isRollingIn = (rollingDistance - lastRollingDistance > 0);

                        lastRollingDistance = rollingDistance;
                        rolling = (lerpValue > 0.0f && lerpValue < 1.0f);
                    }
                    else if(!isFirstTouch)
                    {
                        isFirstTouch = true;
                        if (rolling)
                        {
                            rollingRountine = roll(0.5f, isRollingIn);
                            StartCoroutine(rollingRountine);
                        }
                    }
                }
            }
        }
    }

    public void OnCarpetMeshRebuilt(Carpet carpet)
    {
        this.carpet = carpet;
        carpetLength = (carpet.TopMost.y - carpet.BottomMost.y)*transform.localScale.z;

        unrolledAngle = carpet.TopMost.y* anglePerUnit;
        rolledAngle = carpet.BottomMost.y * anglePerUnit;

        startAngle = Mathf.Abs(unrolledAngle) + Mathf.PI * 2f;
        material?.SetFloat("_StartAngle", startAngle);

        setUnrolledAngle(unrolledAngle);
        //Debug.Log("OnCarpetMeshRebuilt: " + carpet.Polygon.Length+ " "+ carpet.TopMost+" "+carpet.BottomMost+" "+carpetLength);
    }
    private void setUnrolledAngle(float angle)
    {
        currentAngle = angle;
        material?.SetFloat("_UnrolledAngle", currentAngle);
    }
    private bool checkMouseHitPoint(Vector2 relativePoint)
    {
        float radius = pitch * (startAngle-currentAngle);
        float upperBound = currentAngle / anglePerUnit + radius;
        if (relativePoint.y > upperBound) return false;
        return Utils.Instance.ContainsPoint(carpet.Polygon, relativePoint);
    }

    private IEnumerator roll(float time, bool state)
    {
        Debug.Log("Rolling Coroutine");

        float elapsedTime = 0;
        float startingAngle = currentAngle;
        float endingAngle = state ? unrolledAngle : rolledAngle;
        autoRolling = true;
        while (elapsedTime < time)
        {
            setUnrolledAngle(Mathf.Lerp(startingAngle, endingAngle, (elapsedTime / time)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Rolled = state;
        autoRolling = false;
    }


}
