using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetTreeController : MonoBehaviour
{
    // Start is called before the first frame update

    private CarpetTree carpetTree;

    private Node<Carpet> rootNodeToRoll = null;
    private CarpetRoller rollerToRoll = null;

    private int rollTillMatchedTrigger = 0;
    private bool rollOutNextTrigger = false;

    void Start()
    {
        carpetTree = GetComponent<CarpetTree>();

        InputManager.Current.MouseButtonDown += HandleMouseButtonDown;
    }
    private void Update()
    {
        if (rootNodeToRoll != null)
        {
            if (rollerToRoll == null)
            {
                var nodeToRoll = RollInOnce(rootNodeToRoll);

                if (nodeToRoll == null)
                {
                    rollerToRoll = null;
                    rootNodeToRoll = null;
                }
                else
                {
                    rollerToRoll = nodeToRoll.obj.carpetRoller;
                }
            }
            else
            {
                if (!rollerToRoll.rollingIn)
                {
                    rollerToRoll = null;
                }
            }
        }
        if (rollTillMatchedTrigger>0)
        {
            if (rollerToRoll == null)
            {
                if (!carpetTree.IsUnrolledCarpetsInOrder)
                {
                    var nodeToRoll = RollInOnce(carpetTree.CurrentTree);

                    if (nodeToRoll == carpetTree.CurrentTree || nodeToRoll == null)
                    {
                        rollTillMatchedTrigger--;
                        rollerToRoll = null;
                    }
                    else
                    {
                        rollerToRoll = nodeToRoll.obj.carpetRoller;
                    }
                }
                else
                {
                    rollTillMatchedTrigger--;// = false;

                    RollOutNextHint();
                }
            }
            else
            {
                if (!rollerToRoll.rollingIn)
                {
                    rollerToRoll = null;
                }
            }
        }
    }
    public void HandleMouseButtonDown(int mouseId, Vector3 mousePos)
    {
        if (IsRolling()) return;
        //horizontal plane only
        var ray = Camera.main.ScreenPointToRay(mousePos);
        Plane plane = new Plane(transform.up, 0);
        if (plane.Raycast(ray, out float enter))
        {
            var hitpoint = ray.GetPoint(enter);

            CarpetRoller topHitRoller = null;

            float topMost = float.MinValue;

            if (carpetTree.CarpetRollers == null) return;

            foreach (var roller in carpetTree.CarpetRollers)
            {
                Vector3 relativeHitPoint = roller.transform.InverseTransformPoint(hitpoint);
                Vector2 hitpoint2 = new Vector3(relativeHitPoint.x, relativeHitPoint.z);
                if (roller.CheckMouseHitPoint(hitpoint2))
                {
                    if (roller.RolledIn)
                    {
                        topHitRoller = roller;

                        if (!topHitRoller.RolledIn)
                        {
                            break;
                        }
                    }
                    if (topMost < roller.transform.position.y)
                    {
                        topMost = roller.transform.position.y;

                        topHitRoller = roller;
                    }

                }
            }
            if (topHitRoller != null)
            {
                if (topHitRoller.RolledIn)
                {
                    topHitRoller.RollOut();
                }
                else
                {
                    RollFromNode(topHitRoller.carpet.node);
                }
            }
        }
    }

    public void RollFromNode(Node<Carpet> node)
    {
        rootNodeToRoll = node;
    }

    private Node<Carpet> RollInOnce(Node<Carpet> root)
    {
        var nodeToRoll = root.TraverseWithTermination((n) =>
        {
            var roller = n.obj.carpetRoller;

            return (!roller.RolledIn || roller.rollingOut);
        });

        if (nodeToRoll != null)
        {
            if (nodeToRoll.obj.hintFlag)
            {
                return null;
            }

            var r = nodeToRoll.obj.carpetRoller;

            if (r != null)
            {
                if (r.rollingOut)
                {
                    StopCoroutine(r.RollingCoroutine);
                }
                r.RollIn();
            }
        }
        return nodeToRoll;
    }

    public void PerformHint(int count = 1)
    {
        Debug.Log("Doing hint $count actions");

        RollInTillMatched(count);
    }

    public void RollInTillMatched(int count)
    {
        rollTillMatchedTrigger = count;
    }

    public void RollOutNextHint()
    {
        var roller = carpetTree.GetNextRoller();

        if (roller != null)
        {
            roller.RollOut();
        
            roller.carpet.hintFlag = true;
        }
    }

    public void RollOutNextImmediate(bool hintFlag, bool correctFlag)
    {
        var roller = carpetTree.GetNextRoller();

        if (roller != null)
        {
            roller.RollOutImmediate();

            roller.carpet.hintFlag = hintFlag;

            //roller.carpet.correctFlag = correctFlag;
        }
    }

    public bool IsRolling()
    {
        return (rootNodeToRoll != null);
    }
}
