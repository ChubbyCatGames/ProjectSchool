using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SchoolScript : MonoBehaviour
{
    [SerializeField] int numStudent = 45;
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
            yield return new WaitForSeconds(Random.Range(70f,90f));
            BoxCollider selectedClass = SelectRandomClass();

            if (!bell.getValue())
            {
                bellEvent?.Invoke();
                bell.setValue(true);
            }
            else
            {
                bellEventEnd?.Invoke();
                bell.setValue(false);
            }

        }

    }

    public BoxCollider SelectRandomClass()
    {
        int randomClass = Random.Range(0, classList.Count-1);
        return classList[randomClass];
    }
}
