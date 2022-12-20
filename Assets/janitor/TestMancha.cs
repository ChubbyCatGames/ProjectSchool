using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMancha : MonoBehaviour
{

    /// ESTOS PARAMETROS SON CONFIGURABLES DESDE EL INSPECTOR DE UNITY
    public float minSegundos = 1.0f;    // Minima cantidad de segundos que tarda en aparecer una nueva seta    
    public float maxSegundos = 5.0f;    // Maxima cantidad de segundos que tarda en aparecer una nueva seta    
    public float radioManchas = 5.0f;     // Maxima distancia al centro de la escena en la que pueden aparecer setas
    /// 

    // Variables que no se deben tocar
    private bool generandoMancha = false;
    private Vector3 posicionMancha = Vector3.zero;
    private bool hayMancha = true;
    //private bool hayMancha = false;
    [SerializeField] GameObject personaje;
    [SerializeField] int enRadio;
    bool eventoMancha = false;

    //*******************************************************

    void Start()
    {
        // Empezamos sin seta
        //HazInvisible();
    }

    //*******************************************************

    void Update()
    {
        // Si no hay seta y no estamos ya generandola, lanzamos la rutina de generacion
        //if (!generandoMancha && !hayMancha)
          //  StartCoroutine(GeneradorManchas());

        if (hayMancha)
        {
            //Debug.Log("hayMancha");
            if (checkDistance())
            {
                //Debug.Log("manchaCerca");
                //avisar
                controlEvento.current.ManchaCercaPj.Invoke();
                CogeMancha();
            }
        }

    }

    //*******************************************************

    /// <summary>
    /// Rutina de generacion de setas
    /// </summary>
    /// <returns></returns>
    /// 
    private bool checkDistance()
    {
        if ((personaje.transform.position - transform.position).magnitude < enRadio)
        {
            return true;
        }
        return false;
    }
    private IEnumerator GeneradorManchas()
    {
        generandoMancha = true;

        // Espera un tiempo aleatorio para generar una nueva seta
        yield return new WaitForSeconds(Random.Range(minSegundos, maxSegundos));

        // Genera nueva seta pasado el tiempo
        hayMancha = true;
        HazVisible();
        posicionMancha = new Vector3(Random.Range(-radioManchas, radioManchas), 0.0f, Random.Range(-radioManchas, radioManchas));
        transform.position = posicionMancha;
        generandoMancha = false;
    }

    //*******************************************************

    /// <summary>
    ///  Hace invisible la seta. 
    ///  Ante las muchas posibilidades de implementacion, hemos optado por la menos dependiente de version de Unity
    /// </summary>
    public void HazInvisible()
    {
        transform.position = new Vector3(transform.position.x, -10.0f, transform.position.z);
    }

    //*******************************************************

    /// <summary>
    ///  Hace visible la seta. 
    ///  Ante las muchas posibilidades de implementacion, hemos optado por la menos dependiente de version de Unity
    /// </summary>
    public void HazVisible()
    {
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
    }

    //*******************************************************

    /// <summary>
    /// Indica si hay una seta visible
    /// </summary>
    /// <returns></returns>
    public bool HaySeta()
    {
        return hayMancha;
    }

    //*******************************************************

    /// <summary>
    ///  Devuelve la posicion actual de la seta
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosicionMancha()
    {
        return posicionMancha;
    }

    //*******************************************************

    /// <summary>
    ///  Hace la seta invisible y reanuda el proceso de generacion de setas
    /// </summary>
    public void CogeMancha()
    {
        hayMancha = false;
        HazInvisible();
    }
}
