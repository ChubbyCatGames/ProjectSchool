using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehaviour1 : MonoBehaviour
{
    private StateMachineEngine fsm;
    UtilitySystemEngine us;
    int indexBestAction;
    UtilityAction bestAction;

    public NavMeshAgent agent;

    private ClassroomChair chairAux = null;
    private ThroneWC throneAux = null;

    [SerializeField] private List<Transform> targets;
    [SerializeField] private List<Classroom> classList;
    [SerializeField] private List<Bathroom> bathList;

    [SerializeField] public DiningRoomController dc;
    [SerializeField] private SchoolScript sc;

    bool goingToEat = false;

    //Utility System variables
    private int eatIncreaseRate = 0; // Tasa de incremento para la necesidad de comer
    private float peeIncreaseRate; // Tasa de incremento para la necesidad de hacer pipí
    private float ghostingIncreaseRate = 0.2f; // Tasa de incremento para la necesidad de asustar

    public float timePee;
    public float needPee;

    public float timeEat;
    public float needEat;
    public float thresholdEat = 75;

    public bool activeNeed;
    public bool auxNeed;

    //scaring for alumni, teachers room for teachers
    public float timeGhosting;
    public float needGhosting;
    public float a = 20;
    public float b = 50;



    float minNeed = 0;
    float maxNeed = 100;

    private void Awake()
    {
        int seed;
        string str = gameObject.name;
        string str2 = string.Empty;

        for (int i = 0; i < str.Length; i++)
        {
            if (char.IsDigit(str[i]))
            {
                str2 += str[i];
            }
        }
        seed =int.Parse(str2);
        sc.bellEvent.AddListener(BellRings);
        sc.bellEventEnd.AddListener(ClassEnds);

        agent = GetComponent<NavMeshAgent>();
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        Random.InitState(seed);
        agent.speed = Random.Range(2, 5);
        Random.InitState(seed*2);
        agent.acceleration = Random.Range(7, 12);
        maxNeed = Random.Range(75, 1000);
        peeIncreaseRate = Random.Range(0.02f, 0.1f);
        //Machine
        fsm = new StateMachineEngine(false);

        CreateUtilitySystem();

        //Create the states
        State idleState = fsm.CreateEntryState("Idle");
        //State wanderingState = fsm.CreateState("Wandering", Wandering);
        State wanderingState = fsm.CreateSubStateMachine("Wandering", us);
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
        Transition wandering_to_idle = fsm.CreateTransition("Wander_to_idle", wanderingState, isGoingClass, idleState);

        

        //Transition wandering_to_us = fsm.CreateExitTransition("Exit_Transition",wanderingState,isWandering, us);

        

        fsm.Fire("Idle_to_wander");
    }


    private void Update()
    {
 
        fsm.Update();
        

        if (fsm.GetCurrentState().Name == "Wandering")
        {
            Wandering();
            if (activeNeed == false)
            {
                IncreaseNeeds();
            }
            us.Update();

            
        }

        if (fsm.GetCurrentState().Name == "GoingClass")
        {
            if (HasReachedDestination())
            {
                agent.enabled = false;
                transform.position = new Vector3(chairAux.transform.position.x, chairAux.transform.position.y + 0.7f, chairAux.transform.position.z);
                transform.rotation = chairAux.transform.rotation;
                fsm.Fire("GoClass_to_attendClass");
            }
        }
        else if (goingToEat)
        {
            if (HasReachedDestination())
            {
                if (dc.ghostsList.Count < 6)
                {
                    dc.ghostsList.Add(this);
                    goingToEat = false;
                }
            }
        }





        //bestAction = us.GetBestAction();

        //if (bestAction != null)
        //{
        //    bestAction.Execute();
        //}



        Random.InitState(System.Environment.TickCount * (int)transform.position.x);

        if (Random.Range(0, 7000) == 1)
        {
           // GoToEat();
        }

        if (Input.GetKeyDown(KeyCode.X))//This is to force all the students to eat
        {
            GoToEat();
        }

    }


    public static void PrintLine(string s) { 
        Debug.Log(s);
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
        do
        {
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

    private void GoingBath()
    {
        Bathroom b = SelectRandomBath();
        ThroneWC throne;

        do
        {
            Random.InitState(System.Environment.TickCount);
            while (!b.CheckIfRoom())
            {
                b = SelectRandomBath();
            }

            throne = b.OccupeThrone();

        }while (b== null);

        agent.destination = throne.transform.position;
        throneAux = throne;
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

        foreach (Classroom c in classList)
        {
            c.isHappeningClass = false;
        }

        fsm.Fire("AttendClass_to_wander");
    }

    private void GoToEat()
    {
        if (dc.ghostsList.Count < 6)
        {
            fsm.Fire("Wander_to_idle");
            GoToDiningRoom();
        }
    }

    private void GoToDiningRoom()
    {
        agent.destination = dc.transform.position;
        goingToEat = true;
    }

    public void EndOrder()
    {
        Table table = dc.SelectRandomTable();

        Random.InitState(System.Environment.TickCount);
        int random = Random.Range(0, table.chairs.Count);
        Vector3 chair = table.chairs[random].position;
        agent.destination = chair;

        StartCoroutine(EatRoutine(table, chair));
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

    private Bathroom SelectRandomBath()
    {
        Random.InitState(System.Environment.TickCount);
        int randomBath = Random.RandomRange(0,bathList.Count);
        return bathList[randomBath];
    }

    //Coroutines

    IEnumerator EatRoutine(Table table, Vector3 chair)
    {
        yield return new WaitUntil(() => HasReachedDestination());

        agent.enabled = false;
        transform.position = chair;

        yield return new WaitForSeconds(30f);

        agent.enabled = true;
        table.SetHasTray(true);
        fsm.Fire("Idle_to_wander");

    }


    //Utility system
    private void CreateUtilitySystem()
    {
        us = new UtilitySystemEngine(true);

        activeNeed = false;

        needEat = minNeed;
        needPee = 0;
        needGhosting = minNeed;



        Factor factorPee = new LeafVariable(() => needPee, maxNeed, minNeed);//linear
        Factor factorEat = new LeafVariable(() => needEat, maxNeed, minNeed);
        Factor factorGhosting = new LeafVariable(() => needGhosting, maxNeed, minNeed);
        Factor factorWander = new LeafVariable(() => needGhosting, maxNeed, minNeed);

        Factor curvePee = new LinearCurve(factorPee, 1, 0);
        Factor curveGhosting = new Sigmoide(factorGhosting, a, b);
        Factor curveEating = new Threshold(factorEat, thresholdEat);

        Factor curveWander = new LinearCurve(factorWander, 0, 20);

        us.CreateUtilityAction("Pee", UrinatingAction, curvePee);
        us.CreateUtilityAction("Eat", OrderingFoodAction, curveEating);
        us.CreateUtilityAction("Ghosting", GhostingAction, curveGhosting);
        us.CreateUtilityAction("Wander", Wandering, curveWander);
    }



    //ACCIONES DEL SISTEMA DE UTILIDAD
    void IncreaseNeeds()
    {


        if (needEat < thresholdEat)
        {
            needEat += eatIncreaseRate;
        }
        if (needEat == thresholdEat){
            activeNeed = true;
        }


        // Incremento de la necesidad de hacer pipí (lineal)
        if (!activeNeed)
        {
            needPee += peeIncreaseRate;
        }
        if (needPee >= maxNeed)
        {
            activeNeed = true;
            Debug.Log("activenedd trusita");
        }


        // Incremento de la necesidad de asustar (sigmoide)

        //float ghostingSigmoid = 1 / (1 + Mathf.Exp(-a * (needGhosting - b)));
        //needGhosting += ghostingIncreaseRate * ghostingSigmoid;

    }

 



    


    void UrinatingAction()
    {
        
        print("Pipi");
        GoingBath();
        needPee = 0;
        activeNeed = false;
        Debug.Log("falsita acitva nedd");
        us.Reset();
        Debug.Log("Resetao el us");
        
        
        
    }

    void OrderingFoodAction()
    {
        GoToEat();
        print("food");
        us.Reset();
        activeNeed = false;
    }

   

    void GhostingAction()
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
