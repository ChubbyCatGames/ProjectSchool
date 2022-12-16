using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiningRoomController : MonoBehaviour
{
    class Table
    {
        public int id;
        public Transform transform;
        public bool hasTray;

        public Table(int id, Transform transform, bool hasTray)
        {
            this.id = id;
            this.transform = transform;
            this.hasTray = hasTray;
        }
    }

    [SerializeField] private List<GameObject> initialTables = new List<GameObject>();
    private List<Table> listTables = new List<Table>();

    [SerializeField] private CookBehaviour cookReference;

    [SerializeField] private Transform traysShelf;

    private void Awake()
    {
        //initialize the tables
        int id = 0;
        foreach(GameObject o in initialTables)
        {
            listTables.Add(new Table(id, o.transform, false));
            id++;
        }
    }

    private void Update()
    {
        //For testing purposes:
        if (Input.GetKeyDown(KeyCode.A))
        {
            listTables[Random.Range(0, listTables.Count)].hasTray = true;
        }
    }



    //Methods

    public bool CheckIfMustBeCleaned()
    {
        bool b = false;

        foreach (Table t in listTables)
        {
            if (t.hasTray)
            {
                b = true;
                break;
            }
        }

        return b;
    }

    public void MustCleanTray()
    {
        //Construct the path of tables to clean
        List<Table> path = new List<Table>();
        foreach(Table t in listTables)
        {
            if (t.hasTray) path.Add(t);
        }

        StartCoroutine(CleaningTrayRoutine(path));
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
            t.hasTray = false;

            MustCleanTray();

            yield break;
        }
        

        //Now the cook moves back to put the trays in the self
        cookReference.Move(traysShelf);

        yield return new WaitUntil(() => cookReference.HasReachedDestination());

        cookReference.EndCleaningTrays();
    }
}
