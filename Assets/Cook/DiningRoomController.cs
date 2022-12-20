using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiningRoomController : MonoBehaviour
{
    [SerializeField] private List<Table> tablesList = new List<Table>();

    [SerializeField] public List<GhostBehaviour1> ghostsList = new List<GhostBehaviour1>();
    [SerializeField] private Transform queue;

    [SerializeField] private CookBehaviour cookReference;

    [SerializeField] private Transform traysShelf;

    //Food
    [SerializeField] private Transform watermelons;
    [SerializeField] private Transform hotDogs;
    [SerializeField] private Transform cherries;
    [SerializeField] private Transform burgers;
    [SerializeField] private Transform cheese;
    [SerializeField] private Transform bananas;


    private void Update()
    {
        //For testing purposes:
        if (Input.GetKeyDown(KeyCode.M))
        {
            tablesList[Random.Range(0, tablesList.Count)].SetHasTray(true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ghostsList.Add(new GhostBehaviour1());
        }

        DisplayQueue();
    }


    //Methods
    public bool CheckIfMustBeCleaned()
    {
        bool b = false;

        foreach (Table t in tablesList)
        {
            if (t.GetHasTray())
            {
                b = true;
                break;
            }
        }

        return b;
    }

    public bool CheckIfStudentIsWaiting() //Change eventually
    {
        return ghostsList.Count > 0;
    }

    public void MustCleanTray()
    {
        //Construct the path of tables to clean
        List<Table> path = new List<Table>();
        foreach(Table t in tablesList)
        {
            if (t.GetHasTray()) path.Add(t);
        }

        StartCoroutine(CleaningTrayRoutine(path));
    }

    public void MustAttendStudent()
    {
        //Do things
        //Interact with the first studet in the list and ask for the command: AskCommand()

        string command = "cheese";

        //Now go for the command; always the tray first and then the food.
        StartCoroutine(AttendingStudentRoutine(GetFoodTransform(command)));
    }

    private Transform GetFoodTransform(string word)
    {
        switch (word)
        {
            case "watermelon":
                return watermelons;
                break;
            case "hotDog":
                return hotDogs;
                break;
            case "cherry":
                return cherries;
                break;
            case "burger":
                return burgers;
                break;
            case "cheese":
                return cheese;
                break;
            case "banana":
                return bananas;
                break;
            default:
                return watermelons;
                break;
        }
    }

    private void DisplayQueue()//Call when necesary to show or update the queue of ghosts
    {
        float distance = 3f;
        for (int i=0; i<ghostsList.Count; i++)
        {
            ghostsList[i].agent.destination = new Vector3(queue.position.x, ghostsList[i].transform.position.y, queue.position.z - i * distance);
        }
    }

    public Table SelectRandomTable()
    {
        Random.InitState(System.Environment.TickCount);
        int random = Random.Range(0, tablesList.Count);
        return tablesList[random];
    }

    //Coroutines
    IEnumerator CleaningTrayRoutine(List<Table> list)
    {
        if (list.Count > 0)
        {
            Table t = list[0];
            //Move
            cookReference.Move(t.transform);

            //Wait
            yield return new WaitUntil(() => cookReference.HasReachedDestination());

            //Update the State of the table
            t.SetHasTray(false);

            MustCleanTray();

            yield break;
        }
        

        //Now the cook moves back to put the trays in the self
        cookReference.Move(traysShelf);

        yield return new WaitUntil(() => cookReference.HasReachedDestination());

        cookReference.EndCleaningTrays();
    }

    IEnumerator AttendingStudentRoutine(Transform foodTransform)
    {
        yield return new WaitUntil(() => ghostsList[0].HasReachedDestination());
        yield return new WaitForSeconds(2f);

        cookReference.Move(traysShelf);

        yield return new WaitUntil(() => cookReference.HasReachedDestination());

        cookReference.Move(foodTransform);

        yield return new WaitUntil(() => cookReference.HasReachedDestination());

        cookReference.Move(cookReference.studentTransform);

        yield return new WaitUntil(() => cookReference.HasReachedDestination());
        yield return new WaitForSeconds(1f);

        ghostsList[0].EndOrder();

        ghostsList.RemoveAt(0);

        cookReference.EndStudent();
    }
}
