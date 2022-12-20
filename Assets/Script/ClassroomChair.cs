using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassroomChair : MonoBehaviour
{
    public bool occupied = false;

    public void LeaveChair()
    {
        occupied = false;
    }
}
