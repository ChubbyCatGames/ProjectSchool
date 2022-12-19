using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBellStatus : MonoBehaviour
{


    private GameObject ghost;
    private ghostBehaviour ghostBT;
    private Slider bellStatusSlider;
    // Start is called before the first frame update
    void Start()
    {
        bellStatusSlider = GetComponent<Slider>();
    }

    public void SetBellState()
    {
        if (ghost == null)
        {
            ghost = GameObject.FindGameObjectWithTag("Ghost");
            ghostBT = ghost.GetComponent<ghostBehaviour>();
        }

        switch (bellStatusSlider.value)
        {
            case 0:
                ghostBT.bellState = BellStatus.Unactive;
                break;

            case 1:
                ghostBT.bellState = BellStatus.Active;
                
                break;
        }
    }

  
}
