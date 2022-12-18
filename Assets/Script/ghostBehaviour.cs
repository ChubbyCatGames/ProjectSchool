using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ghostBehaviour : MonoBehaviour
{
    //Navigation Agent
    public NavMeshAgent agent;

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
    public float thresholdEat = 75;

    //scaring for alumni, teachers room for teachers
    public float timeGhosting;
    public float needGhosting;

    float minNeed = 0;
    float maxNeed = 1;

    //Signs controller
    [SerializeField] private SignsController sc;

    //BT
    private void CreateBehaviourTree()
    {
        ghostBT = new BehaviourTreeEngine();

        root = ghostBT.CreateSequenceNode("Root selector", false);
        selectorGoClass = ghostBT.CreateSelectorNode("Go to class");
        wander = ghostBT.CreateSubBehaviour("Wander", us);
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

        selectorGoClass.AddChild(wander);
        selectorGoClass.AddChild(sequenceClass);

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
    public virtual void WalkToClass() { }

    public ReturnValues ArriveToClass() { return ReturnValues.Succeed; } 
    
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
    
        Factor factorPee= new LeafVariable(()=>needPee, maxNeed, minNeed);//linear
        Factor factorEat = new LeafVariable(()=>needEat, maxNeed, minNeed);//sigmoide
        Factor factorGhosting = new LeafVariable(()=>needGhosting, maxNeed, minNeed);//umbral/threshold

        Factor curvePee = new LinearCurve(factorPee, 0.01f);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateBehaviourTree();
        CreateUtilitySystem();
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

    //ACCIONES DEL BT

}
