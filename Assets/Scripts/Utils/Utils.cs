using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /*Singleton declaration*/
    private static Utils instance = null;
    public static Utils Instance
    {
        get
        {
            if (instance == null)
                instance = new Utils();
            return instance;
        }
    }
    Utils() { }
    /*End of singleton declaration*/
    public bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
}
