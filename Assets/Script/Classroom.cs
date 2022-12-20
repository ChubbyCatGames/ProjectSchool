using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classroom : MonoBehaviour
{
    [SerializeField] private List<ClassroomChair> chairsList = new List<ClassroomChair>();
    [SerializeField] public Transform techerPos; 

    public bool isHappeningClass = false;

    private ClassroomChair SelectChair()
    {
        ClassroomChair chair;
        int random;
        do
        {
            Random.InitState(System.Environment.TickCount);
            random = Random.Range(0, chairsList.Count);
            chair = chairsList[random];

            if (!CheckIfRoom())
            {
                return null;
            }

        } while (chairsList[random].occupied);

        return chair;
    }

    public ClassroomChair OccupeChair()
    {
        ClassroomChair chair = SelectChair();
        chair.occupied = true;

        return chair;
    }

    public bool CheckIfRoom()
    {
        bool b = false;

        foreach(ClassroomChair c in chairsList)
        {
            if (!c.occupied) b = true;
        }

        return true;
    }

}
