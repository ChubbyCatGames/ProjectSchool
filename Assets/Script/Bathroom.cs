using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathroom : MonoBehaviour
{
    [SerializeField] private List<ThroneWC> thronesList = new List<ThroneWC>();




    private ThroneWC SelectThrone()
    {
        ThroneWC throne;
        int random;
        do
        {
            Random.InitState(System.Environment.TickCount);
            random = Random.Range(0, thronesList.Count);
            throne = thronesList[random];

            if (!CheckIfRoom())
            {
                return null;
            }

        } while (thronesList[random].occupied);

        return throne;
    }

    public ThroneWC OccupeThrone()
    {
        ThroneWC throne = SelectThrone();
        if (throne != null)
        {
            throne.occupied = true;
        }

        return throne;
    }

    public bool CheckIfRoom()
    {
        bool b = false;

        foreach (ThroneWC t in thronesList)
        {
            if (!t.occupied) b = true;
        }

        return b;
    }
}
