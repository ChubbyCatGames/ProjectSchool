using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestJanitor : MonoBehaviour
{
    [SerializeField] private SignsController scJan;
    private NavMeshAgent meshAgent;
    private StateMachineEngine fsm;
    private string interruptedState;

    private ArriveToDestination arriveToDestination;
    //destinos
    private Vector3 posDest;
    [SerializeField] private List<GameObject> targets;
    private int n = 0;
    private int tope;
    private Vector3 origin;
    //ref cocinera
    [SerializeField] private GameObject rcook;
    

    //para avisar a la cocinera
    private bool enAlmacen = false;
    public bool flagForCook;
    public bool endFlirt = false;


    // Start is called before the first frame update
    void Start()
    {

        fsm = new StateMachineEngine();
        meshAgent = GetComponent<NavMeshAgent>();
        //posDest = Dest.transform.position;
        origin = meshAgent.transform.position;
        posDest = targets[n].transform.position;
        //tope = targets.Count;

        //STATES
        State idle = fsm.CreateEntryState("idle");
        State walk = fsm.CreateState("walk", WalkAction);
        State walkToPA = fsm.CreateState("walkToPA", WalkToPAAction);
        State walkToPB = fsm.CreateState("walkToPB", WalkToPBAction);
        State flirt = fsm.CreateState("flirt", FlirtAction);
        State originAgain = fsm.CreateState("originAgain", OriginAgainAction);

        // State waitingForCook = fsm.CreateState("waitingForCook", waitingForCookAction);
        //State flirt = fsm.CreateState("flirt", BlueAction);
        State cleanS = fsm.CreateState("cleanS", CleanAction);

        //Perception
        arriveToDestination = fsm.CreatePerception<ArriveToDestination>(new ArriveToDestination());
        Perception started = fsm.CreatePerception<PushPerception>();
    

        //Transition
        fsm.CreateTransition("started", idle, started, walk);
        fsm.CreateTransition("second", walk, started, walkToPA);

        fsm.CreateTransition("flirt", walkToPA, started, flirt);

        fsm.CreateTransition("third", flirt, started, walkToPB);

        fsm.CreateTransition("clean", walk, started,cleanS);
        fsm.CreateTransition("continue", cleanS, started, walk);

        fsm.CreateTransition("originAgain", walkToPB, started, originAgain);
        fsm.CreateTransition("restart", originAgain, started, idle);
        //fsm.CreateTransition("nearWarehuse", walk, nearWarehuse, flirt);
        //fsm.CreateTransition("cookEvent", flirt, cookEvent, walk);

        //fsm.CreateTransition("stainAppears", walk, stainAppears, clean);
        //fsm.CreateTransition("staincCleaned", clean, staincCleaned, walk);
        //Debug.Log(fsm.GetCurrentState().Name);
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        arriveToDestination.SetPosition(transform.position);

        /*Debug.Log(arriveToDestination.GetPosition());
        Debug.Log(arriveToDestination.GetDestination());
        Debug.Log(Vector3.Distance(arriveToDestination.GetPosition(), arriveToDestination.GetDestination()));
        
        if (arriveToDestination.Check2() == true)
        {
            //Debug.Log(n);
            //Debug.Log(tope);
            
            if (n < tope - 1)
            {

                n++;
                meshAgent.destination = targets[n].transform.position;

            }

        }*/

        Debug.Log(fsm.GetCurrentState().Name);
        if (posDest != null && fsm.GetCurrentState().Name =="idle")
        {
            fsm.Fire("started");
            //Debug.Log(fsm.GetCurrentState().Name);
        }

        //Debug.Log(fsm.GetCurrentState().Name);
        //Debug.Log(arriveToDestination.Check2());
        //Debug.Log(Vector3.Distance(transform.position, targets[0].transform.position));
        if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.8f && fsm.GetCurrentState().Name == "walk")
        {
            Debug.Log("EjecutaSegundo");

            fsm.Fire("second");


            //Debug.Log("HACIA segundo destino");
        }

        //Debug.Log(Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f);
        if (fsm.GetCurrentState().Name == "walkToPA" && Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f)
        {
            Debug.Log("EN segundo destino");
            if (endFlirt == false)
            {
                fsm.Fire("flirt");
            }
        }

        if (fsm.GetCurrentState().Name == "flirt" && Vector3.Distance(transform.position, targets[1].transform.position) < 2.0f)
        {
            //Debug.Log("xd");
            if (endFlirt == true)
            {
                fsm.Fire("third");
                
            }
        }

        if(fsm.GetCurrentState().Name == "walkToPB" && Vector3.Distance(transform.position, targets[2].transform.position) < 2.0f)
        {
            fsm.Fire("originAgain");
        }

        
        if (fsm.GetCurrentState().Name == "originAgain" && Vector3.Distance(transform.position, origin) < 2.0f)
        {
            fsm.Fire("restart");
        }

        /*

        if (fsm.GetCurrentState().Name == "walkToPA" && endFlirt == false)
        {
            Debug.Log("EN segundo destino");
            WaitingForCookAction();
            
        }
        if (endFlirt == true && Vector3.Distance(transform.position, targets[1].transform.position) < 0.8f)
        {
            fsm.Fire("third");
        }
            */
        //if(arriveToDestination.Check2() && fsm.GetCurrentState().Name=="")
        /*
        if (arriveToDestination.Check2() && flagForCook == false)
        {
            fsm.Fire("third");
        }*/
        //if llega a almcacen
        //enAlmacen = true

    }
    
    private void OnEnable()
    {
        //Debug.Log("Enable");
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
        //meshAgent.destination = posDest;
        arriveToDestination.SetDestination(meshAgent.destination);

    }
    void WalkToPBAction()
    {
        n++;
        meshAgent.destination = targets[n].transform.position;

        meshAgent.speed = 3.5f;
       // meshAgent.destination = posDest;
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
        //Debug.Log("ESPERANDO");
        
        flagForCook = true;
       // while (Vector3.Distance(transform.position, rcook.transform.position)>1.0f) {}
        StartCoroutine(Flirt());
        //StopCoroutine(Flirt());
        //flirt();
        //flagForCook = false;
    }

    void OriginAgainAction()
    {
        meshAgent.destination = origin;

        meshAgent.speed = 3.5f;
        // meshAgent.destination = posDest;
        arriveToDestination.SetDestination(meshAgent.destination);
        endFlirt = false;
        n = 0;
    }

    IEnumerator Flirt()
    {
        //Debug.Log("FLIRTEANDO");
        
        scJan.ShowNewSign(0);
        yield return new WaitForSeconds(3.0f);
        //Debug.Log("FIN");
        scJan.RemoveSign();
        flagForCook = false;
        endFlirt = true;
    }

    IEnumerator Clean()
    {
        Debug.Log("LIMPIANDO");

        scJan.ShowNewSign(1);
        meshAgent.speed = 0;
        yield return new WaitForSeconds(3.0f);
        //Debug.Log("FIN");
        scJan.RemoveSign();
        meshAgent.speed = 3.5f;
        fsm.Fire("continue");
        //endClean = true;
    }
}
