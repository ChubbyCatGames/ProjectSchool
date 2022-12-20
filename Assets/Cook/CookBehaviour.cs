using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CookBehaviour : MonoBehaviour
{

    private NavMeshAgent agent;

    //Signs controller
    [SerializeField] public SignsController sc;

    //Target Positions
    [SerializeField] private Transform sinkTransform;
    [SerializeField] private Transform binTransform;
    [SerializeField] private Transform kitchenTransform;
    //[SerializeField] private Transform trayTransform;
    [SerializeField] public Transform studentTransform;
    [SerializeField] private Transform janitorTransform;

    public Transform previousDestiny;

    //State Machine
    private StateMachineEngine wandering_fsm;
    private StateMachineEngine main_fsm;

    //Room reference
    [SerializeField] private DiningRoomController room;

    //Control variables
    private bool isAttendingStudent = false;

    //Janitor
    [SerializeField] TestJanitor janitor;

    void Awake()
    {
        //Init
        agent = GetComponent<NavMeshAgent>();
        previousDestiny = GetRandomDestiny();


        //Create the wandering submachine--------------------------
        wandering_fsm = new StateMachineEngine(true);

        //Create the states
        State walkingState = wandering_fsm.CreateEntryState("Walking", MoveToNewDestiny);
        State sinkState = wandering_fsm.CreateState("Sink", SinkEvent);
        State binState = wandering_fsm.CreateState("Bin", BinEvent);
        State kitchenState = wandering_fsm.CreateState("Kitchen", KitchenEvent);

        //Create perceptions
        Perception isWalking = wandering_fsm.CreatePerception<PushPerception>();
        Perception isInSink = wandering_fsm.CreatePerception<PushPerception>();
        Perception isInBin = wandering_fsm.CreatePerception<PushPerception>();
        Perception isInKitchen = wandering_fsm.CreatePerception<PushPerception>();

        //Create transitions
        Transition walk_sink_transition = wandering_fsm.CreateTransition("Walk_to_sink", walkingState, isInSink, sinkState);
        Transition walk_bin_transition = wandering_fsm.CreateTransition("Walk_to_bin", walkingState, isInBin, binState);
        Transition walk_kitchen_transition = wandering_fsm.CreateTransition("Walk_to_kitchen", walkingState, isInKitchen, kitchenState);
        Transition sink_walk_transition = wandering_fsm.CreateTransition("Sink_to_walk", sinkState, isWalking, walkingState);
        Transition bin_walk_transition = wandering_fsm.CreateTransition("Bin_to_walk", binState, isWalking, walkingState);
        Transition kitchen_walk_transition = wandering_fsm.CreateTransition("Kitchen_to_walk", kitchenState, isWalking, walkingState);

        //-------------------------------------
        //Create the main machine--------------------------------
        main_fsm = new StateMachineEngine(false);

        //States
        State idleState = main_fsm.CreateEntryState("Idle");
        State wanderingState = main_fsm.CreateSubStateMachine("Wandering", wandering_fsm, walkingState);
        State trayState = main_fsm.CreateState("Tray", TrayEvent);
        State studentState = main_fsm.CreateState("Student", StudentEvent);
        State janitorState = main_fsm.CreateState("Janitor", JanitorEvent);

        //Create perceptions
        Perception isWandering = main_fsm.CreatePerception<PushPerception>();
        Perception isWithTray = main_fsm.CreatePerception<PushPerception>();
        Perception isWithStudent = main_fsm.CreatePerception<PushPerception>();
        Perception isWithJanitor = main_fsm.CreatePerception<PushPerception>();

        //Create the transitions
        Transition idle_to_wandering = main_fsm.CreateTransition("Idle_to_wandering", idleState, isWandering, wanderingState);
        Transition wandering_to_tray = main_fsm.CreateTransition("Wandering_to_tray", wanderingState, isWithTray, trayState);
        Transition wandering_to_student = main_fsm.CreateTransition("Wandering_to_student", wanderingState, isWithStudent, studentState);
        Transition wandering_to_janitor = main_fsm.CreateTransition("Wandering_to_janitor", wanderingState, isWithJanitor, janitorState);
        Transition tray_to_wandering = main_fsm.CreateTransition("Tray_to_wandering", trayState, isWandering, wanderingState);
        Transition student_to_wandering = main_fsm.CreateTransition("Student_to_wandering", studentState, isWandering, wanderingState);
        Transition janitor_to_wandering = main_fsm.CreateTransition("Janitor_to_wandering", janitorState, isWandering, wanderingState);


        //Call the walk perception
        main_fsm.Fire("Idle_to_wandering");
    }


    void Update()
    {
        print(main_fsm.GetCurrentState().Name);
        wandering_fsm.Update();
        main_fsm.Update();

        if (main_fsm.GetCurrentState().Name == "Wandering")
        {
            if (HasReachedDestination())
            {
                if (previousDestiny == sinkTransform)
                {
                    wandering_fsm.Fire("Walk_to_sink");

                    sc.ShowNewSign(0);
                }
                else if (previousDestiny == binTransform)
                {
                    wandering_fsm.Fire("Walk_to_bin");

                    sc.ShowNewSign(1);
                }
                else if (previousDestiny == kitchenTransform)
                {
                    wandering_fsm.Fire("Walk_to_kitchen");

                    sc.ShowNewSign(2);
                }

                transform.LookAt(new Vector3(previousDestiny.position.x, transform.position.y, previousDestiny.position.z), Vector3.up);
            }
        }

        if (CheckIfTray())
        {
            main_fsm.Fire("Wandering_to_tray");
        }

        if (CheckIfStudent())
        {
            main_fsm.Fire("Wandering_to_student");
        }

        if(main_fsm.GetCurrentState().Name == "Janitor")
        {
            if (HasReachedDestination())
            {
                sc.ShowNewSign(4);
            }
        }

        if(main_fsm.GetCurrentState().Name == "Student" && !isAttendingStudent)
        {
            if (HasReachedDestination())
            {
                isAttendingStudent = true;

                sc.ShowNewSign(3);

                room.MustAttendStudent();
            }
        }

        //test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            JanitorCalls();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            EndJanitor();
        }

        if (janitor.flagForCook)
        {
            JanitorCalls();
        }
    }


    //Action functions
    private void MoveToNewDestiny()
    {
        Transform destinySelected = SelectNewDestiny();
        Move(destinySelected);
    }

    public void Move(Transform destiny)
    {
        previousDestiny = destiny;
        agent.destination = destiny.position;
    }

    private void SinkEvent()
    {
        StartCoroutine(StartTimer());
    }

    private void BinEvent()
    {
        StartCoroutine(StartTimer());
    }

    private void KitchenEvent()
    {
        StartCoroutine(StartTimer());
    }

    private void TrayEvent()
    {
        StopCoroutine(StartTimer());
        main_fsm.Fire("Wandering_to_tray");

        sc.RemoveSign();

        //Call the room controller to start controlling the path of the trays
        room.MustCleanTray();
    }

    private void StudentEvent()
    {
        StopCoroutine(StartTimer());

        Move(studentTransform);

        sc.RemoveSign();
    }

    private void JanitorEvent()
    {
        StopCoroutine(StartTimer());

        Move(janitorTransform);

        sc.RemoveSign();
    }

    //Utilities

    private void JanitorCalls()
    {
        StopCoroutine(StartTimer());

        main_fsm.Fire("Wandering_to_janitor");
    }

    private bool CheckIfTray()
    {
        return room.CheckIfMustBeCleaned();
    }

    private bool CheckIfStudent()
    {
        return room.CheckIfStudentIsWaiting();
    }

    public void EndCleaningTrays()
    {
        main_fsm.Fire("Tray_to_wandering");
        MoveToNewDestiny();
    }

    public void EndJanitor()
    {
        main_fsm.Fire("Janitor_to_wandering");

        sc.RemoveSign();

        MoveToNewDestiny();
    }

    public void EndStudent()
    {
        main_fsm.Fire("Student_to_wandering");

        isAttendingStudent = false;
        sc.RemoveSign();

        MoveToNewDestiny();
    }

    public bool HasReachedDestination()
    {
        bool destinyReached = false;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    destinyReached = true;
                }
            }
        }
        return destinyReached;
    }

    private Transform SelectNewDestiny() //This function decides a new random location among the three available
    {
        Transform selected;

        do
        {
            selected = GetRandomDestiny();
        } while (selected == previousDestiny);

        return selected;
    }

    private Transform GetRandomDestiny()
    {
        Transform randomDestiny;
        int r = Random.Range(0, 3);

        switch (r)
        {
            case 0:
                randomDestiny = sinkTransform;
                break;
            case 1:
                randomDestiny = binTransform;
                break;
            case 2:
                randomDestiny = kitchenTransform;
                break;
            default:
                randomDestiny = kitchenTransform;
                break;
        }

        return randomDestiny;
    }





    //Coroutines
    IEnumerator StartTimer() //This timer controls that the cook stays 5 seconds in each of the different labours
    {
        yield return new WaitForSeconds(5f);

        if (main_fsm.GetCurrentState().Name != "Wandering") yield break;

        //Call the walk perception after the wait is over
        if(wandering_fsm.GetCurrentState() == wandering_fsm.GetState("Sink"))
        {
            wandering_fsm.Fire("Sink_to_walk");
        }
        else if (wandering_fsm.GetCurrentState() == wandering_fsm.GetState("Bin"))
        {
            wandering_fsm.Fire("Bin_to_walk");
        }
        else if (wandering_fsm.GetCurrentState() == wandering_fsm.GetState("Kitchen"))
        {
            wandering_fsm.Fire("Kitchen_to_walk");
        }

        sc.RemoveSign();
    }
}
