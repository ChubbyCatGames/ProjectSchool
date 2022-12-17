using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ghostBehaviour : MonoBehaviour
{
    //Navigation Agent
    private NavMeshAgent agent;

    //Behaviour Tree
    BehaviourTreeEngine ghostBT;
    SequenceNode seqNode;
    LeafNode wander;
    LeafNode inClass;

    //Utility System
    UtilitySystemEngine us;
    

    //Signs controller
    [SerializeField] private SignsController sc;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ghostBT = new BehaviourTreeEngine();
        us = new UtilitySystemEngine();
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
