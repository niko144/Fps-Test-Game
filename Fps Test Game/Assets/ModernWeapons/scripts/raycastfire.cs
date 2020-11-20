using UnityEngine;
using System.Collections;

public class raycastfire : MonoBehaviour {

	public float force = 500f;
	public float damage = 50f;
	public float range = 100f;
	
	public LayerMask mask;
	public int projectilecount = 1;
	public float inaccuracy = 0.1f;
	
	public GameObject impactnormal;
	public GameObject impactconcrete;
	public GameObject impactwood;
	public GameObject impactblood;
	public GameObject impactwater;
	public GameObject impactmetal;
	public GameObject impactglass;
    public GameObject impactdust;


    public GameObject impactmeleenormal;

    public void fireMelee ()
	{
		Vector3 fwrd = transform.forward;
		
		
		Ray ray = new Ray (transform.position, fwrd);
		RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 3f, mask))
        {

            if (hit.rigidbody) hit.rigidbody.AddForceAtPosition(500 * fwrd, hit.point);
            hit.transform.SendMessageUpwards("Damage", 100f, SendMessageOptions.DontRequireReceiver);
            GameObject decal;
            if (hit.transform.tag == "flesh")
            {



                decal = Instantiate(impactblood, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
           
            else if (hit.transform.tag == "Untagged")
            {
                decal = Instantiate(impactmeleenormal, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
            else if (hit.transform.tag == "dirt")
            {
                decal = Instantiate(impactdust, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
            else if (hit.transform.tag == "concrete")
            {
                decal = Instantiate(impactconcrete, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
           
            else if (hit.transform.tag == "metal")

            {
                decal = Instantiate(impactmeleenormal, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }

            else if (hit.transform.tag == "wood")
            {
                decal = Instantiate(impactmeleenormal, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
        }
	}

	public void fire () 
	{
		for(int i = 0; i < projectilecount; i++)
		{
			firebullet();
		}
	}

	void firebullet()
	{

		Vector3 fwrd = transform.forward;

		Vector3 camUp = transform.up;
		Vector3 camRight = transform.right;
		
		Vector3 wantedvector = fwrd;
		wantedvector += Random.Range( -inaccuracy, inaccuracy ) * camUp + Random.Range( -inaccuracy, inaccuracy ) * camRight;
		Ray ray = new Ray (transform.position, wantedvector);
		RaycastHit hit = new RaycastHit();
       
        if (Physics.Raycast(ray,out hit, range,mask))
		{   
			


			hit.transform.SendMessage("Damage",damage, SendMessageOptions.DontRequireReceiver);

			GameObject decal;
			
			if (hit.transform.tag == "Untagged")  
			{	
				decal =Instantiate(impactnormal, hit.point,Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject ;
               
                decal.transform.parent = hit.transform;
			}
            else if (hit.transform.tag == "dirt")
            {
                decal = Instantiate(impactdust, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

                decal.transform.parent = hit.transform;
            }
            else if (hit.transform.tag  == "concrete") 
			{
				decal =Instantiate(impactconcrete, hit.point,Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject ;
               
                decal.transform.parent = hit.transform;
			}
			
			else if (hit.transform.tag  == "metal") 
				
			{
				decal =Instantiate(impactmetal, hit.point,Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject ;
                
                decal.transform.parent = hit.transform;
			}
			
			else if (hit.transform.tag  == "wood") 
			{
				decal =Instantiate(impactwood, hit.point,Quaternion.FromToRotation(-Vector3.forward, hit.normal))as GameObject ;
                
                decal.transform.parent = hit.transform;
			}
			else if (hit.transform.tag  == "water") 
			{
				decal =Instantiate(impactwater, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject ;
               
                decal.transform.parent = hit.transform;
			}
			else if (hit.transform.tag  == "glass") 
			{
				decal =Instantiate(impactglass, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject ;
                
                decal.transform.parent = hit.transform;
			}
			else if (hit.transform.tag  == "flesh") 
			{
				
				if (hit.transform.GetComponent<bodypart> () != null)
				{
					
					hit.transform.GetComponent<bodypart> ().hitvector = force * fwrd;

				}
                decal = Instantiate(impactblood, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal)) as GameObject;

            }
			if(hit.rigidbody) hit.rigidbody.AddForceAtPosition (force * fwrd , hit.point);
		}
	}
}
