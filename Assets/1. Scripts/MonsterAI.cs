using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MonsterAI : MonoBehaviour
{
    private Transform playerTr;
    private PlayerHealth playerHealth;
    private Animator anim;
    private Monster monster;
    private AudioSource audioSource;

    private float detectionRange = 20f;
    private float distance;

    private bool isAttacking = false;

    private float attackCooldown = 1f;
    private float lastAttackTime;

    private NavMeshAgent agent;

    private bool isDead = false;

    public AudioClip attackClip;

    public float attackRange = 1.0f;
    public float damage = 1f;
    public float stopDistance = 1.0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        monster = GetComponent<Monster>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, playerTr.position);

        if (distance <= detectionRange && monster.IsDead() == false)
        {
            if(playerHealth.PlayerDead() == true) return;
            
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

            if (monster.IsDead() == true) return;
            agent.SetDestination(transform.position);
        }
    }

    protected void LookAtPlayer()
    {
        if (monster.IsDead() == true) return;

        if (playerTr != null)
        {
            Vector3 direction = playerTr.position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }
    void AttackPlayer()
    {
        if (monster.IsDead() == true) return;

        // Debug.Log(distance);
        anim.SetBool("ATTACK", true);
        anim.SetBool("Chase", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PlayerHealth playerHealth = playerTr.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        AttackSound();
    }

    void ChasePlayer()
    {
        if (monster.IsDead() == true) return;

        //Debug.Log(distance);
        anim.SetBool("Chase", true);
        anim.SetBool("ATTACK", false);
        agent.SetDestination(playerTr.position);
    }

    public void Dead()
    {
        if (monster.IsDead() == false) return;

        if (monster.IsDead() == true && isDead == false)
        {
            GetComponent<Collider>().enabled = false;
            agent.enabled = false;
            anim.SetTrigger("Dead");
            isDead = true;
        }

        Destroy(monster.gameObject, 5.0f);
    }

    void AttackSound()
    {
        if (!isAttacking)
            StartCoroutine(PlayAttackSoundOnce());
    }

    IEnumerator PlayAttackSoundOnce()
    {
        isAttacking = true;
        audioSource.PlayOneShot(attackClip);
        yield return new WaitForSeconds(attackClip.length);
        isAttacking = false;
    }
}