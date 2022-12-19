using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolScript : MonoBehaviour
{
    [SerializeField] int numStudent = 30;
    [SerializeField] int numTeacher= 10;

    [Header("Prefabs")]
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
    
    // Start is called before the first frame update
    void Start()
    {
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
            studentList.Add(Instantiate(prefabStudent));
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
            yield return new WaitForSeconds(Random.Range(20f,40f));
            Debug.Log("A CLASE");
            
            //IF BELLSTATUS DICE QUE TOCA CLASE
            BoxCollider selectedClass = SelectRandomClass();
            //Foreach pj => lanzar una percepcion de que ha sonado la campana

            //IF BELLSTATUS DICE QUE NO TOCA CLASE
            //A WANDEREAR JEFES
        }
    }

    public BoxCollider SelectRandomClass()
    {
        int randomClass = Random.Range(0, classList.Count);
        return classList[randomClass];
    }
}
