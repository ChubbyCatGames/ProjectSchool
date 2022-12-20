using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignsController : MonoBehaviour
{
    [SerializeField] private List<Sprite> signsList = new List<Sprite>();

    private SpriteRenderer sr;

    private float animVelocity;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = null;

        animVelocity = 1;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);

        //Animation
        transform.localScale = new Vector3(
            transform.localScale.x + 0.1f * animVelocity * Time.deltaTime, 
            transform.localScale.y + 0.1f * animVelocity * Time.deltaTime, 
            transform.localScale.z + 0.1f * animVelocity * Time.deltaTime);

        if (transform.localScale.x < 0.5f || transform.localScale.x > 0.6f) animVelocity = -animVelocity;
    }

    public void ShowNewSign(int i)
    {
        sr.sprite = signsList[i];
    }

    public void RemoveSign()
    {
        sr.sprite = null;
    }
}
