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
    private StateMachineEngine fsm;


    void Awake()
    {
        //Init
        agent = GetComponent<NavMeshAgent>();
        previousDestiny = GetRandomDestiny();


        //Create the FSM
        fsm = new StateMachineEngine(false);

        //Create the statesç
        State idleState = fsm.CreateEntryState("Idle");
        State walkingState = fsm.CreateState("Walking", MoveToDestiny);
        State sinkState = fsm.CreateState("Sink", SinkEvent);
        State binState = fsm.CreateState("Bin", BinEvent);
        State kitchenState = fsm.CreateState("Kitchen", KitchenEvent);

        //Create perceptions
        Perception isWalking = fsm.CreatePerception<PushPerception>();
        Perception isInSink = fsm.CreatePerception<PushPerception>();
        Perception isInBin = fsm.CreatePerception<PushPerception>();
        Perception isInKitchen = fsm.CreatePerception<PushPerception>();

        //Create transitions
        Transition idle_walk_transition = fsm.CreateTransition("Idle_to_walk", idleState, isWalking, walkingState);
        Transition walk_sink_transition = fsm.CreateTransition("Walk_to_sink", walkingState, isInSink, sinkState);
        Transition walk_bin_transition = fsm.CreateTransition("Walk_to_bin", walkingState, isInBin, binState);
        Transition walk_kitchen_transition = fsm.CreateTransition("Walk_to_kitchen", walkingState, isInKitchen, kitchenState);
        Transition sink_walk_transition = fsm.CreateTransition("Sink_to_walk", sinkState, isWalking, walkingState);
        Transition bin_walk_transition = fsm.CreateTransition("Bin_to_walk", binState, isWalking, walkingState);
        Transition kitchen_walk_transition = fsm.CreateTransition("Kitchen_to_walk", kitchenState, isWalking, walkingState);


        //Call the walk perception
        fsm.Fire("Idle_to_walk");
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();

        if (HasReachedDestination())
        {
            if(previousDestiny == sinkTransform)
            {
                fsm.Fire("Walk_to_sink");
            }
            else if(previousDestiny == binTransform)
            {
                fsm.Fire("Walk_to_bin");
            }
            else if(previousDestiny == kitchenTransform)
            {
                fsm.Fire("Walk_to_kitchen");
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
        if(fsm.GetCurrentState() == fsm.GetState("Sink"))
        {
            fsm.Fire("Sink_to_walk");
        }
        else if (fsm.GetCurrentState() == fsm.GetState("Bin"))
        {
            fsm.Fire("Bin_to_walk");
        }
        else if (fsm.GetCurrentState() == fsm.GetState("Kitchen"))
        {
            fsm.Fire("Kitchen_to_walk");
        }
    }
}
