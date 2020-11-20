using UnityEngine;
using System.Collections;

public class rpgrocket : MonoBehaviour
{
    public Transform explosion;
    public float speed = 1000f;
    public float waitTime = 10.0f;

    void Start()
    {
        StartCoroutine(waitanddestroy());
    }
    private void Update()
    {

        transform.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, speed);
    }
    void explode()
    {

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    IEnumerator waitanddestroy()
    {
        yield return new WaitForSeconds(waitTime);
        explode();
    }
    private void OnCollisionEnter(Collision collision)
    {

       
        explode();
    }


}
