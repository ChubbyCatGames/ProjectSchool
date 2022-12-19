using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ghostBehaviour : MonoBehaviour
{
    public BellStatus bellState { get; set; }
    public bool bellRinging;
    //Navigation Agent
    public NavMeshAgent agent;

    //MovimientodDelPersonaje
    public Vector3 destination;
    public float speed = 5.0f;
    public float maxStep = 1.0f;
    public Vector3 bellDestination;

    //Behaviour Tree
    BehaviourTreeEngine ghostBT;
    SequenceNode root;
    SelectorNode selectorGoClass;
    LeafNode wander;
    SequenceNode sequenceClass;
    LeafNode goToClass;
    SelectorNode selectClassAction;
    LeafNode learn;
    SequenceNode sequenceTurn;
    LeafNode isturnAround;
    LeafNode joking;
    //TimerDecoratorNode timerNode;

    //Utility System
    UtilitySystemEngine us;
    public float timePee;
    public float needPee;

    public float timeEat;
    public float needEat;
    public const float thresholdEat = 75;

    //scaring for alumni, teachers room for teachers
    public float timeGhosting;
    public float needGhosting;
    public const float a = 20;
    public const float b = 50;

    float minNeed = 0;
    float maxNeed = 1;

    //Signs controller
    [SerializeField] private SignsController sc;

    //BT
    private void CreateBehaviourTree()
    {
        

        root = ghostBT.CreateSequenceNode("Root selector", false);
        selectorGoClass = ghostBT.CreateSelectorNode("Go to class");
        sequenceClass = ghostBT.CreateSequenceNode("Actions in class", false);
        goToClass = ghostBT.CreateLeafNode("Walk to class", WalkToClass, ArriveToClass);
        selectClassAction = ghostBT.CreateSelectorNode("Select class action");
        learn = ghostBT.CreateLeafNode("Learn in class", learnAction, checkLearning);
        sequenceTurn = ghostBT.CreateSequenceNode("Teacher sequence", false);
        isturnAround = ghostBT.CreateLeafNode("Is tecaher turned around", isTurning, checkTurning);
        joking = ghostBT.CreateLeafNode("Joke", jokingAction, checkJoking);
        //timerNode = ghostBT.CreateTimerNode("Timer", isturnAround, 5);

        //Children
        root.AddChild(selectorGoClass);

        selectorGoClass.AddChild(sequenceClass);
        selectorGoClass.AddChild(wander);
        

        sequenceClass.AddChild(goToClass);
        sequenceClass.AddChild(selectClassAction);

        selectClassAction.AddChild(learn);
        selectClassAction.AddChild(sequenceTurn);

        sequenceTurn.AddChild(isturnAround);
        sequenceTurn.AddChild(joking);

        //Root
        ghostBT.SetRootNode(root);
    }

    //ACTIONS BT

    //Go to class
    public virtual void WalkToClass() {
        if (bellState == BellStatus.Active)
        {
            agent.destination = new Vector3()
            bellRinging = true;


        }
        else
        {
            bellRinging = false;
        }
       
    }
   

    public ReturnValues ArriveToClass() {
        if (bellRinging = true)
        {
            return ReturnValues.Succeed;
        } 
        else return{
            return ReturnValues.Failed;
        }
    } 
    
    //Learning
    public virtual void learnAction() { }

    public ReturnValues checkLearning() { return ReturnValues.Succeed; }

    //Is turning?
    public virtual void isTurning() { }

    public ReturnValues checkTurning() { return ReturnValues.Succeed; }
    
    //Joking
    public virtual void jokingAction() { }

    public ReturnValues checkJoking() { return ReturnValues.Succeed; }

    private void CreateUtilitySystem()
    {
        us = new UtilitySystemEngine(true);

        needEat = minNeed;
        needPee = minNeed;
        needGhosting = minNeed;

        Wandering();
    
        Factor factorPee= new LeafVariable(()=>needPee, maxNeed, minNeed);//linear
        Factor factorEat = new LeafVariable(()=>needEat, maxNeed, minNeed);//sigmoide
        Factor factorGhosting = new LeafVariable(()=>needGhosting, maxNeed, minNeed);//umbral/threshold

        Factor curvePee = new LinearCurve(factorPee, 0.01f);
        Factor curveGhosting = new Sigmoide(factorGhosting, a, b);
        Factor curveEating = new Threshold(factorEat, thresholdEat);

        us.CreateUtilityAction("Pee", UrinatingAction, curvePee);
        us.CreateUtilityAction("Eat", OrderingFoodAction, curveEating);
        us.CreateUtilityAction("Ghosting", GhostingAction, curveGhosting);

        wander = ghostBT.CreateSubBehaviour("Wander", us);

    }

    private void Awake()
    {
        ghostBT = new BehaviourTreeEngine();
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {

        bellRinging = false;

        CreateUtilitySystem();
        CreateBehaviourTree();
        
    }

    // Update is called once per frame
    void Update()
    {
        ghostBT.Update();
        us.Update();

        
    }


    //ACCIONES DEL SISTEMA DE UTILIDAD
    void Wandering()
    {
        Vector3 destination = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));

        // Calculate the direction to the destination
        Vector3 direction = (destination - transform.position).normalized;

        // Move the character towards the destination
        transform.position = transform.position + direction * Mathf.Min(speed * Time.deltaTime, maxStep);
    }

    void UrinatingAction()
    {

    }

    void OrderingFoodAction()
    {

    }

    void EatingAction()
    {

    }

    protected virtual void GhostingAction()
    {

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
