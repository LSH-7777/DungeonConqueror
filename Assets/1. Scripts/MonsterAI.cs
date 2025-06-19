using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    private Transform player;
    private Animator anim;

    private float detectionRange = 20f;
    private float distance;

    private NavMeshAgent agent;

    public float attackRange = 1.0f;
    public float damage = 1f;
    public float stopDistance = 1.0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            LookAtPlayer();

            if (distance > stopDistance)
            {
                ChasePlayer();
            }
            else
            {
                anim.SetBool("Chase", false);
                agent.SetDestination(transform.position);
            }

            if (distance <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                anim.SetBool("ATTACK", false);
            }
        }
        else
        {
            anim.SetBool("ATTACK", false);
            anim.SetBool("Chase", false);
            agent.SetDestination(transform.position);
        }
    }

    protected void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }
    void AttackPlayer()
    {
       // Debug.Log(distance);
       anim.SetBool("ATTACK", true);
       anim.SetBool("Chase", false);
    }

    void ChasePlayer()
    {
        //Debug.Log(distance);
        anim.SetBool("Chase", true);
        anim.SetBool("ATTACK", false);
        agent.SetDestination(player.position);
    }
}
