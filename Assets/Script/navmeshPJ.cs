using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshPJ : MonoBehaviour
{

    [SerializeField] private Transform position;

    private NavMeshAgent navMesh;
    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navMesh.destination = position.position;
    }
}
