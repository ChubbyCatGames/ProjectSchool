using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestJanitor : MonoBehaviour
{
    [SerializeField] private SignsController scJan;
    [SerializeField] private SchoolScript sc;
    private NavMeshAgent meshAgent;
    private StateMachineEngine fsm;
    private string interruptedState;

    private ArriveToDestination arriveToDestination;

    //destinos
    private Vector3 posDest;
    [SerializeField] private List<GameObject> targets;
    private int n = 0;
    private Vector3 origin;


    [SerializeField] CookBehaviour cook;

    //para avisar a la cocinera
    public bool flagForCook;
    public bool endFlirt = false;
    public bool classRn = false;


    // Start is called before the first frame update
    void Start()
    {

        fsm = new StateMachineEngine();
        meshAgent = GetComponent<NavMeshAgent>();
        origin = meshAgent.transform.position;
        posDest = targets[n].transform.position;

        //relacion con campana
        sc.bellEvent.AddListener(()=> classRn = true);
        sc.bellEventEnd.AddListener(() => classRn = false);

        //STATES
        State idle = fsm.CreateEntryState("idle");
        State walk = fsm.CreateState("walk", WalkAction);
        State walkToPA = fsm.CreateState("walkToPA", WalkToPAAction);
        State walkToPB = fsm.CreateState("walkToPB", WalkToPBAction);
        State cleanS = fsm.CreateState("cleanS", CleanAction);
        State flirt = fsm.CreateState("flirt", FlirtAction);
        State originAgain = fsm.CreateState("originAgain", OriginAgainAction);

        //Perceptions
        arriveToDestination = fsm.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        Perception started = fsm.CreatePerception<PushPerception>();
        Perception stainClean = fsm.CreatePerception<PushPerception>();
        Perception stainClose = fsm.CreatePerception<PushPerception>();
        Perception cookArrives = fsm.CreatePerception<PushPerception>();
        Perception cookLeaves = fsm.CreatePerception<PushPerception>();
        Perception atDestination = fsm.CreatePerception<PushPerception>();

        //Transitions
        fsm.CreateTransition("started", idle, started, walk);
        fsm.CreateTransition("second", walk, atDestination, walkToPA);

        //para flirtear
        fsm.CreateTransition("flirt", walkToPA, cookArrives, flirt);
        //si flirtea
        fsm.CreateTransition("third", flirt, cookLeaves, walkToPB);
        //si no flirtea
        fsm.CreateTransition("thirdNoFlirt", walkToPA, atDestination, walkToPB);

        //para limpiar
        fsm.CreateTransition("clean", walk, stainClose, cleanS);
        fsm.CreateTransition("continue", cleanS, stainClean, walk);

        fsm.CreateTransition("originAgain", walkToPB, atDestination, originAgain);
        fsm.CreateTransition("restart", originAgain, started, idle);

    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        arriveToDestination.SetPosition(transform.position);

        if (posDest != null && fsm.GetCurrentState().Name =="idle")
        {
            fsm.Fire("started");
        }

        if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.8f && fsm.GetCurrentState().Name == "walk")
        {
            fsm.Fire("second");
        }

        //Si están en clase flirtea, sino sale del almacen
        if (classRn)
        {
            if (fsm.GetCurrentState().Name == "walkToPA" && Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f)
            {
                Debug.Log("EN segundo destino");
                if (endFlirt == false)
                {
                    fsm.Fire("flirt");
                }
            }
        }
        else
        {
            if (fsm.GetCurrentState().Name == "walkToPA" && Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f)
            {
                    fsm.Fire("thirdNoFlirt");
            }
        }


        if (fsm.GetCurrentState().Name == "flirt" && Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f)
        {

            if (endFlirt == true)
            {
                fsm.Fire("third");
            }
        }


        if (fsm.GetCurrentState().Name == "walkToPB" && Vector3.Distance(transform.position, targets[2].transform.position) < 2.0f)
        {
            fsm.Fire("originAgain");
        }

        
        if (fsm.GetCurrentState().Name == "originAgain" && Vector3.Distance(transform.position, origin) < 2.0f)
        {
            fsm.Fire("restart");
        }

    }
    
    private void OnEnable()
    {
        controlEvento.current.ManchaCercaPj.AddListener(FireClean);
    }
    void WalkAction()
    {

        meshAgent.speed = 3.5f;
        meshAgent.destination = posDest;
        arriveToDestination.SetDestination(meshAgent.destination);

    }

    //se va a dinning
    void WalkToPAAction()
    {
        n++;
        meshAgent.destination = targets[n].transform.position;

        meshAgent.speed = 3.5f;
        arriveToDestination.SetDestination(meshAgent.destination);

    }
    void WalkToPBAction()
    {
        n++;
        meshAgent.destination = targets[n].transform.position;

        meshAgent.speed = 3.5f;
        arriveToDestination.SetDestination(meshAgent.destination);

    }

    void FireClean()
    {
        interruptedState = fsm.GetCurrentState().Name;
        fsm.Fire("clean");
    }
    void CleanAction()
    {
        StartCoroutine(Clean());
    }

    void FlirtAction()
    {     
        flagForCook = true;
        StartCoroutine(Flirt());
    }

    void OriginAgainAction()
    {
        meshAgent.destination = origin;

        meshAgent.speed = 3.5f;
        arriveToDestination.SetDestination(meshAgent.destination);
        endFlirt = false;
        n = 0;
    }

    bool CookNear()
    {
        if (Vector3.Distance(transform.position, cook.transform.position) <= 5f)
        {
            return true;
        }
        return false;
    }

    IEnumerator Flirt()
    {
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => (cook.HasReachedDestination() && cook.flagJanitor && CookNear()));
        scJan.ShowNewSign(1);   
        yield return new WaitForSeconds(4.0f);
        cook.EndJanitor();
        scJan.RemoveSign();
        flagForCook = false;
        endFlirt = true;
    }

    IEnumerator Clean()
    {
        scJan.ShowNewSign(0);
        meshAgent.speed = 0;
        yield return new WaitForSeconds(3.0f);
        scJan.RemoveSign();
        meshAgent.speed = 3.5f;
        fsm.Fire("continue");
    }
}
