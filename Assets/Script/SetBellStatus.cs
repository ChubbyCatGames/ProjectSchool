using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBellStatus : MonoBehaviour
{
    public bool isActive = false;

    private GameObject ghost;
    private ghostBehaviour ghostBT;
    private SchoolScript school;
    private BellStatus bellStatus;
    // Start is called before the first frame update
    void Start()
    {
        bellStatus = BellStatus.Unactive;
    }

    public void SetBellStateActive()
    {
        bellStatus = BellStatus.Active;
        isActive = true;
        
    }

    public void SetBellStateUnactive()
    {
        bellStatus = BellStatus.Unactive;
        isActive = false;
    }


}
