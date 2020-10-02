using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonVertex : MonoBehaviour
{

    public int Index { private set; get; } = -1;
    private Vector3 oldPosition;

    public Action<PolygonVertex> PositionChangedEvent = null;
    // Start is called before the first frame update
    void Start()
    {
        oldPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position != oldPosition)
        {
            oldPosition = transform.position;
            PositionChangedEvent?.Invoke(this);
        }
    }
    public void Init(int index)
    {
        this.Index = index;
    }

}
