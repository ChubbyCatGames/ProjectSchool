using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classroom : MonoBehaviour
{
    [SerializeField] private List<ClassroomChair> chairsList = new List<ClassroomChair>();
    public bool isHappeningClass = false;

    public ClassroomChair SelectChair()
    {
        ClassroomChair chair;
        int random;
        do
        {
            random = Random.Range(0, chairsList.Count);
            chair = chairsList[random];

        } while (!chairsList[random].occupied);

        return chair;
    }

    public ClassroomChair OccupeChair()
    {
        ClassroomChair chair = SelectChair();
        chair.occupied = true;

        return chair;
    }

    public void LeaveChair(ClassroomChair chair)
    {
        chair.occupied = false;
    }
}
