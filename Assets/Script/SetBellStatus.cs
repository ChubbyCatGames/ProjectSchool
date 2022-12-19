using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBellStatus : MonoBehaviour
{


    private GameObject ghost;
    private ghostBehaviour ghostBT;
    private Slider bellStatusSlider;
    private SchoolScript school;
    // Start is called before the first frame update
    void Start()
    {
        bellStatusSlider = GetComponent<Slider>();
    }

    public void SetBellState()
    {
        if (school.isRinging)
        {
            ghostBT.bellState = BellStatus.Active;
        }
        else if(!school.isRinging) { ghostBT.bellState = BellStatus.Unactive; }

       
    }

  
}
