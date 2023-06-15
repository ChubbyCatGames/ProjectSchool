using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPee : MonoBehaviour
{
    public static List<int> peeRands = new List<int>();

    private void Awake()
    {
        for(int i = 0; i < 20; i++)
        {
            peeRands.Add((int)Random.Range(0.02f, 0.1f));
        }
    }
}
