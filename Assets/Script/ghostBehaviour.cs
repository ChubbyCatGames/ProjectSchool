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
    LeafNode inClass;
    SelectorNode classAction;
    LeafNode learn;
    SequenceNode turnAround;
    LeafNode isturnAround;
    LeafNode joking;
    TimerDecoratorNode timerNode;

    //Utility System
    UtilitySystemEngine us;
    

    //Signs controller
    [SerializeField] private SignsController sc;

    //BT
    private void CreateBehaviourTree()
    {
        ghostBT = new BehaviourTreeEngine();

        root = ghostBT.CreateSequenceNode("Root selector", false);
        selectorGoClass = ghostBT.CreateSelectorNode("Go to class");
        wander = ghostBT.CreateLeafNode("Wandering" );
        inClass = ghostBT.CreateLeafNode("In class", inClassAction, checkClass);
        classAction = ghostBT.CreateSelectorNode("Select class action");
        learn = ghostBT.CreateLeafNode("Learn in class", learnAction, checkLearning);
        turnAround = ghostBT.CreateSequenceNode("Teacher sequence", false);
        isturnAround = ghostBT.CreateLeafNode("Is tecaher turned around", isTurning, checkTurning);
        joking = ghostBT.CreateLeafNode("Joke", jokingAction, checkJoking);
        timerNode = ghostBT.CreateTimerNode("Timer", isturnAround, 5);

        //Children
        root.AddChild(wander);
        root.AddChild(inClass);
        root.AddChild(selectorGoClass);

        selectorGoClass.AddChild(classAction);
        selectorGoClass.AddChild(learn);
        selectorGoClass.AddChild(turnAround);

        turnAround.AddChild(isturnAround);
        turnAround.AddChild(timerNode);
        turnAround.AddChild(joking);

        //Root
        ghostBT.SetRootNode(root);
    }

    //Métodos BT
    public virtual void inClassAction() { }

    public ReturnValues checkClass() { return ReturnValues.Succeed; } 
    
    public virtual void learnAction() { }

    public ReturnValues checkLearning() { return ReturnValues.Succeed; }

    public virtual void isTurning() { }

    public ReturnValues checkTurning() { return ReturnValues.Succeed; }
    
    public virtual void jokingAction() { }

    public ReturnValues checkJoking() { return ReturnValues.Succeed; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        us = new UtilitySystemEngine(true);
    }

    // Update is called once per frame
    void Update()
    {
        ghostBT.Update();
        us.Update();
    }

    void WanderAction()//ESTE NODO ES EL SISTEMA DE UTILIDAD
    {

    }

    //ACCIONES DEL SISTEMA DE UTILIDAD
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
