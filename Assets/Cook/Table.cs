using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private bool hasTray;

    //Chairs
    [SerializeField] private List<Transform> chairs;

    //Tray 
    [SerializeField] private GameObject tray;

    public void SetHasTray(bool f)
    {
        hasTray = f;

        if (hasTray)
        {
            tray.SetActive(true);
        }
        else
        {
            tray.SetActive(false);
        }
    }

    public bool GetHasTray()
    {
        return hasTray;
    }

    private void Awake()
    {
        tray.gameObject.SetActive(false);
    }

}
