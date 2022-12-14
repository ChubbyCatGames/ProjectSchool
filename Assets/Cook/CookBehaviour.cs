using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CookBehaviour : MonoBehaviour
{

    private NavMeshAgent agent;

    //Target Positions
    [SerializeField] private Transform sinkTransform;
    [SerializeField] private Transform binTransform;
    [SerializeField] private Transform kitchenTransform;

    private Transform previousDestiny;

    //State Machine
    private StateMachineEngine wandering_fsm;
    private StateMachineEngine main_fsm;


    void Awake()
    {
        //Init
        agent = GetComponent<NavMeshAgent>();
        previousDestiny = GetRandomDestiny();


        //Create the wandering submachine--------------------------
        wandering_fsm = new StateMachineEngine(true);

        //Create the states
        State walkingState = wandering_fsm.CreateEntryState("Walking", MoveToDestiny);
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
        State wanderingState = main_fsm.CreateSubStateMachine("Wandering", wandering_fsm);
        State chairState = main_fsm.CreateState("Chair");
        State studentState = main_fsm.CreateState("Student");
        State janitorState = main_fsm.CreateState("Janitor");

        //Create perceptions
        Perception isWandering = main_fsm.CreatePerception<PushPerception>();
        Perception isWithChair = main_fsm.CreatePerception<PushPerception>();
        Perception isWithStudent = main_fsm.CreatePerception<PushPerception>();
        Perception isWithJanitor = main_fsm.CreatePerception<PushPerception>();

        //Create the transitions
        Transition idle_to_wandering = main_fsm.CreateTransition("Idle_to_wandering", idleState, isWandering, wanderingState);


        //Call the walk perception
        main_fsm.Fire("Idle_to_wandering");
    }


    void Update()
    {
        wandering_fsm.Update();
        main_fsm.Update();

        if (HasReachedDestination())
        {
            if(previousDestiny == sinkTransform)
            {
                wandering_fsm.Fire("Walk_to_sink");
            }
            else if(previousDestiny == binTransform)
            {
                wandering_fsm.Fire("Walk_to_bin");
            }
            else if(previousDestiny == kitchenTransform)
            {
                wandering_fsm.Fire("Walk_to_kitchen");
            }

        }
    }


    //Action functions
    private void MoveToDestiny()
    {
        Transform destinySelected = SelectNewDestiny();
        previousDestiny = destinySelected;
        agent.destination = destinySelected.position;
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

    //Utilities
    private bool HasReachedDestination()
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
    }
}
