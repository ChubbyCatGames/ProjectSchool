using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SchoolScript : MonoBehaviour
{
    [SerializeField] int numStudent = 30;
    [SerializeField] int numTeacher= 10;
    public Bell bell;

    [Header("Prefabs")]
    [SerializeField] GameObject setBell;
    [SerializeField] GameObject prefabStudent;
    [SerializeField] GameObject prefabTeacher;
    [SerializeField] GameObject prefabJanitor;
    [SerializeField] GameObject prefabCook;

    //AQUI HAY QUE CAMBIAR LOS GAMEOBJECTS POR EL TIPO DE CADA SCRIPT 
    //STUDENT A SCRIPT DE STUDENT /// TEACHER AL SUYO Y ESO
    private List<GameObject> studentList = new List<GameObject>();
    private List<GameObject> teacherList = new List<GameObject>();
    private GameObject janitor;
    private GameObject cook;

    [Header("Positions")]
    [SerializeField] Transform initialPosStudent;
    [SerializeField] Transform initialPosTeacher;
    [SerializeField] Transform initialPosJanitor;
    [SerializeField] Transform initialPosCook;

    [Header("Classes")]
    [SerializeField]public List<BoxCollider> classList = new List<BoxCollider>();

    public UnityEvent bellEvent;
    public UnityEvent bellEventEnd;
    bool bellbool = false;

    // Start is called before the first frame update
    void Start()
    {
        bell = new Bell();
        InitializeWorld();


    }
    /// <summary>
    /// FUNCION QUE INICIALIZA EL MUNDO Y TODOS SUS FANTASMAS
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    private void InitializeWorld()
    {
        for (int i = 0; i < numStudent; i++)
        {

            studentList.AddRange(GameObject.FindGameObjectsWithTag("Student"));
            studentList[i].transform.position = initialPosStudent.position;
        }
        for (int i = 0; i < numTeacher; i++)
        {
            teacherList.Add(Instantiate<GameObject>(prefabTeacher));
            teacherList[i].transform.position = initialPosTeacher.position;
        }
        janitor= Instantiate(prefabJanitor);
        janitor.transform.position= initialPosJanitor.position;
        cook = Instantiate(prefabCook);
        cook.transform.position= initialPosCook.position;

        StartCoroutine(CountdownBell());

    }


    ///<summary>
    /// FUNCION QUE GESTIONA UN EVENTO NUEVO CADA X TIEMPO
    /// </summary>
    /// <returns> 
    /// Cuando se llama inicia una cuenta atrás y al llegar al final sonará la campana y el estado de los fantasmas deberá cambiar
    /// </returns>

    IEnumerator CountdownBell()
    {
        while(true) {
            yield return new WaitForSeconds(Random.Range(50f,70f));
            Debug.Log("A CLASE");
            //IF BELLSTATUS DICE QUE TOCA CLASE
            bell.setValue(true);
            BoxCollider selectedClass = SelectRandomClass();

            if (!bellbool)
            {
                bellEvent?.Invoke();
            }
            else
            {
                bellEventEnd?.Invoke();
            }
            
            //Foreach pj => lanzar una percepcion de que ha sonado la campana
            //foreach(var ghost in studentList)
            //{
            //    ghost.GetComponent<ghostBehaviour>().bellRinging = true;
            //}

            //IF BELLSTATUS DICE QUE NO TOCA CLASE
            //A WANDEREAR JEFES
        }

    }

    public BoxCollider SelectRandomClass()
    {
        int randomClass = Random.Range(0, classList.Count-1);
        return classList[randomClass];
    }
}
