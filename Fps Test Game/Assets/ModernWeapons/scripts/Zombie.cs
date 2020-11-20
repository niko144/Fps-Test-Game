using UnityEngine;
using System.Collections;
using UnityEngine.AI;
[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class Zombie : MonoBehaviour {

	public float wanderRadius;
	//private float wanderTimer;
    public float wanderinterval;
    public AudioSource footaudiosource;
    public AudioClip[] footnormal;
    private Transform target;
	private NavMeshAgent agent;
    private bool takesbreak;
    private Animator anim;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    private Quaternion lookrot;



    void Start () {
		agent = GetComponent<NavMeshAgent> ();
        //wanderTimer = Time.time + wanderinterval;
        anim = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }
   
    // Update is called once per frame
    void Update()
    {
        
        if (AgentDone() )
        {
            agent.isStopped = true;
            anim.SetFloat("speed", 0f, .5f, Time.deltaTime * 2f);

            if (!takesbreak)
            {
                StartCoroutine(takeBreak(wanderinterval));
            }
           


        } 
        
        else
        {
            float speed = agent.desiredVelocity.magnitude;

            Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;

            float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;

            
            anim.SetFloat("speed", speed, .5f, Time.deltaTime* 2f);
        }
        


    }
    IEnumerator takeBreak (float waittime)
    {
        takesbreak = true;
        anim.SetFloat("speed", 0f, .5f, Time.deltaTime*2f);
        agent.isStopped = true;
        yield return new WaitForSeconds(waittime);
        takesbreak = false;
        agent.isStopped = false;
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.destination = (newPos);

        

    }
    void OnAnimatorMove()
    {

       
        agent.velocity = anim.deltaPosition / Time.deltaTime;
        //overrride rootrotation here
        if (agent.steeringTarget - transform.position != Vector3.zero && agent.velocity.magnitude >=0.5f && !takesbreak)
        {
            lookrot = Quaternion.LookRotation(agent.steeringTarget - transform.position, Vector3.up);
            
        }

        GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookrot, Time.deltaTime * 3f);
  }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);

		return navHit.position;

	}
    
    public void playfootsound()
    {
        int n = Random.Range(1, footnormal.Length);
        footaudiosource.clip = footnormal[n];
        footaudiosource.pitch = Random.Range(0.8f, 1.2f);
        footaudiosource.Play();
        footnormal[n] = footnormal[0];
        footnormal[0] = footaudiosource.clip;
    }
    protected bool AgentDone()
    {
        return !agent.pathPending && AgentStopping();
    }

    protected bool AgentStopping()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }
}

