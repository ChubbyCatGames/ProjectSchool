using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell 
{

    private bool isActive;

    public Bell()
    {
        isActive = false;
    }
   
    public bool getValue()
    {
        return isActive;
    }

    public void setValue(bool b)
    {
        this.isActive = b;
    }
    
}
