using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableChangeEventPublisher<T>
{
    T variable;
    T oldValue;
    public void Check()
    {

    }
    public void SetVariableRef(ref T v)
    {
        variable = v;
    }
    
}
