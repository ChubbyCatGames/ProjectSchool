using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroneWC : MonoBehaviour
{
    public bool occupied = false;

    public void LeaveThrone()
    {
        occupied = false;
    }
}
