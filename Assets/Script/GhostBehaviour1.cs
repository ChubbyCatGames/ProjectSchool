using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour1 : MonoBehaviour
{
    private StateMachineEngine fsm;
    UtilitySystemEngine us;

    private NavMeshAgent agent;

    private ClassroomChair chairAux = null;

    [SerializeField] private List<Transform> targets;
    [SerializeField] private List<Classroom> classList;

    //Utility System variables
    public float timePee;
    public float needPee;

    public float timeEat;
    public float needEat;
    public const float thresholdEat = 75;

    public bool activeNeed;

    //scaring for alumni, teachers room for teachers
    public float timeGhosting;
    public float needGhosting;
    public const float a = 20;
    public const float b = 50;

    float minNeed = 0;
    float maxNeed = 1;

    private void Awake()
    {

        agent = GetComponent<NavMeshAgent>();

        Random.InitState(System.Environment.TickCount);
        agent.speed = Random.Range(2, 5);
        Random.InitState(System.Environment.TickCount);
        agent.acceleration = Random.Range(7, 12);

        //Machine
        fsm = new StateMachineEngine(false);

        //Create the states
        State idleState = fsm.CreateEntryState("Idle");
        State wanderingState = fsm.CreateState("Wandering", Wandering);
        State goingClassState = fsm.CreateState("GoingClass", GoingClass);
        State attendingClassState = fsm.CreateState("AttendingClass", AttendingClass);

        //Create perception
        Perception isWandering = fsm.CreatePerception<PushPerception>();
        Perception isGoingClass = fsm.CreatePerception<PushPerception>();
        Perception isAttendingClass = fsm.CreatePerception<PushPerception>();

        //Create Transitions
        Transition idle_to_wandering = fsm.CreateTransition("Idle_to_wander", idleState, isWandering, wanderingState);
        Transition wandering_to_goClass = fsm.CreateTransition("Wander_to_goClass", wanderingState, isGoingClass, goingClassState);
        Transition goClass_to_attendClass = fsm.CreateTransition("GoClass_to_attendClass", goingClassState, isAttendingClass, attendingClassState);
        Transition attendClass_to_wandering = fsm.CreateTransition("AttendClass_to_wander", attendingClassState, isWandering, wanderingState);

        CreateUtilitySystem();

        fsm.Fire("Idle_to_wander");
    }


    private void Update()
    {
        fsm.Update();

        if(fsm.GetCurrentState().Name == "Wandering")
        {
            us.Update();
            Wandering();
        }

        if (fsm.GetCurrentState().Name == "GoingClass")
        {
            if (HasReachedDestination())
            {
                agent.enabled = false;
                transform.position = new Vector3(chairAux.transform.position.x, chairAux.transform.position.y + 0.7f, chairAux.transform.position.z);
                fsm.Fire("GoClass_to_attendClass");
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            BellRings();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClassEnds();
        }
    }



    private void Wandering()
    {
        // Check if the character has reached its destination
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            // Generate a random position within the limits of the stage
            //Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

            Vector3 randomPosition = SelectRandomPosition();

            // Find the closest point on the NavMesh to the random position
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas);

            // Set the character's destination to the closest point on the NavMesh
            agent.SetDestination(randomPosition);
        }

        // Check if the character is still on the NavMesh
        if (!agent.isOnNavMesh)
        {
            // Move the character back onto the NavMeshsa
            agent.Warp(transform.position);
        }
    }

    private void GoingClass()
    {
        Classroom c = SelectRandomClass();
        ClassroomChair chair;

        //Select class
        do {
            Random.InitState(System.Environment.TickCount);
            while (!c.CheckIfRoom())
            {
                c = SelectRandomClass();
            }
            
            //Select chair
            chair = c.OccupeChair();
        } while (c == null);

        c.isHappeningClass = true;

        agent.destination = chair.transform.position;
        chairAux = chair;
    }

    private void AttendingClass()
    {

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


    //Methods

    private void BellRings()
    {
        fsm.Fire("Wander_to_goClass");
    }

    private void ClassEnds()
    {
        agent.enabled = true;
        chairAux.LeaveChair();
        chairAux = null;

        foreach(Classroom c in classList)
        {
            c.isHappeningClass = false;
        }

        fsm.Fire("AttendClass_to_wander");
    }

    private Vector3 SelectRandomPosition()
    {
        Random.InitState(System.Environment.TickCount);
        int random = Random.Range(0, targets.Count);
        Debug.Log(random);
        Vector3 position = targets[random].position;

        return position;
    }

    private Classroom SelectRandomClass()
    {
        Random.InitState(System.Environment.TickCount);
        int randomClass = Random.Range(0, classList.Count);
        return classList[randomClass];
    }

    //Utility system
    private void CreateUtilitySystem()
    {
        us = new UtilitySystemEngine(true);

        activeNeed = false;

        needEat = minNeed;
        needPee = minNeed;
        needGhosting = minNeed;
        

        Factor factorPee = new LeafVariable(() => needPee, maxNeed, minNeed);//linear
        Factor factorEat = new LeafVariable(() => needEat, maxNeed, minNeed);//sigmoide
        Factor factorGhosting = new LeafVariable(() => needGhosting, maxNeed, minNeed);//umbral/threshold

        Factor curvePee = new LinearCurve(factorPee, 0.01f);
        Factor curveGhosting = new Sigmoide(factorGhosting, a, b);
        Factor curveEating = new Threshold(factorEat, thresholdEat);

        us.CreateUtilityAction("Pee", UrinatingAction, curvePee);
        us.CreateUtilityAction("Eat", OrderingFoodAction, curveEating);
        us.CreateUtilityAction("Ghosting", GhostingAction, curveGhosting);
    }

    //ACCIONES DEL SISTEMA DE UTILIDAD


    void GenerateMovement()
    {
        // Set a new random destination within the bounds of the NavMesh
        agent.SetDestination(Random.insideUnitSphere * agent.areaMask);
    }


    void UrinatingAction()
    {
        print("Pipi");
    }

    void OrderingFoodAction()
    {
        print("food");
    }

    void EatingAction()
    {
        print("eat");
    }

    protected virtual void GhostingAction()
    {
        print("ghost");
    }

}

public class Sigmoide : Curve
{
    #region variables

    private float m, c;

    #endregion

    #region constructors
    /// <summary>
    /// Creates a linear function factor that modify the value of the factor provided.
    /// </summary>
    /// <param name="f">The <see cref="Factor"/> provided to get a new value from it.</param>
    /// <param name="pend">The slope of the curve. Optional paramenter.</param>
    /// <param name="ind">The vertical displacement of the curve. Optional paramenter.</param>
    public Sigmoide(Factor f, float pend = 1, float ind = 0) : base(f)
    {
        this.m = pend;
        this.c = ind;


    }

    #endregion

    public override float getValue()
    {
        return (float)(1 / (1 + System.Math.Exp(-m * (factor.getValue() - c))));
    }

    /// <summary>
    /// Sets a new value to the slope of the curve.
    /// </summary>
    public void setA(float _m)
    {
        this.m = _m;
    }

    /// <summary>
    /// Sets a new value to the vertical displacement.
    /// </summary>
    public void setB(float _c)
    {
        this.c = _c;
    }


}

public class Threshold : Curve
{
    #region variables

    private float t;

    #endregion

    #region constructors
    /// <summary>
    /// Creates a linear function factor that modify the value of the factor provided.
    /// </summary>
    /// <param name="f">The <see cref="Factor"/> provided to get a new value from it.</param>
    /// <param name="pend">The slope of the curve. Optional paramenter.</param>
    /// <param name="ind">The vertical displacement of the curve. Optional paramenter.</param>
    public Threshold(Factor f, float th) : base(f)
    {
        this.t = th;

    }

    #endregion

    public override float getValue()
    {
        float u;

        if (factor.getValue() < t) u = 0;
        else u = 1;

        return u;
    }

    /// <summary>
    /// Sets a new value to the threshold.
    /// </summary>
    public void setT(float _m)
    {
        this.t = _m;
    }


}
