using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    private Transform player;
    private Animator anim;

    private SphereCollider detectionRange;
    private float distance;

    public float attackRange = 0.5f;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        detectionRange = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        anim.SetBool("Chase", other.gameObject.CompareTag("Player"));
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LookAtPlayer();
            AttackPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Chase", false);
        anim.SetBool("ATTACK", false);
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
        distance = Vector3.Distance(player.position, transform.position);

        if (distance <= attackRange)
        {
            anim.SetBool("ATTACK", true);
        }
    }
}
