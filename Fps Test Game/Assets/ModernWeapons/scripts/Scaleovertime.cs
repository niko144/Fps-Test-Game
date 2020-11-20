using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaleovertime : MonoBehaviour {
    public Vector3 originalscale;
    public float fadespeed;




	
    // Update is called once per frame
    void Update()
    {
        float step = fadespeed * Time.deltaTime;
        //Vector3 temp = Vector3.Lerp(originalscale, Vector3.zero, step);
        transform.localScale -= Vector3.one * step;

        if (transform.localScale == Vector3.zero)
        {
            Destroy(gameObject);
        }

    }
}
