using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CookerBehaviour : MonoBehaviour
{

    private NavMeshAgent agent;

    //Target Positions
    [SerializeField] private Transform sinkTransform;
    [SerializeField] private Transform binTransform;
    [SerializeField] private Transform kitchenTransform;

    //State Machine
    private StateMachineEngine fsm;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        //Create the FSM
        fsm = new StateMachineEngine(false);

        //Create the statesç
        State idleState = fsm.CreateEntryState("Idle");
        State walkingState = fsm.CreateState("Walking", moveToDestiny);
        State sinkState = fsm.CreateState("Sink", sinkEvent);
        State binState = fsm.CreateState("Bin", binEvent);
        State kitchenState = fsm.CreateState("Kitchen", kitchenEvent);

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
        fsm.Fire(isWalking);
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    //Action functions
    private void moveToDestiny()
    {
        Transform destinySelected = selectNewDestiny();
        agent.destination = destinySelected.position;
    }

    private void sinkEvent()
    {

    }

    private void binEvent()
    {

    }

    private void kitchenEvent()
    {

    }

    //Utilities
    private bool hasReachedDestination()
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

    private Transform selectNewDestiny()
    {
        Transform selected;
        selected = sinkTransform;

        return selected;
    }
}
