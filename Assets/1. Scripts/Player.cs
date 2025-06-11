using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rbody;
    public FixedJoystick fixedJoystick;
    public Vector3 moveDir;  
    public float moveSpeed;
    public float attackRange = 5f;

    public GameObject stack1;
    public GameObject stack2;
    public int num = 0;

    [HideInInspector]
    public Transform stack1Tr;
    [HideInInspector]
    public Transform stack2Tr;
    
    private Resource meat;
    private Resource curResource;

    private Animator animator;
    private GameObject enemy;
    private Transform enemyTr;

    private float distance;
    private readonly float term = 0.05f;
    private float nextTerm = 0f;

    private void Start()
    {
        stack1Tr = stack1.transform;
        stack2Tr = stack2.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerMove();
        UpadateAnim();
        StopToWall();
    }

    void FixedUpdate()
    {
        if (!StopToWall())
        {
            rbody.MovePosition(rbody.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal") + fixedJoystick.Horizontal;
        float z = Input.GetAxisRaw("Vertical") + fixedJoystick.Vertical;
        
        moveDir = new Vector3(x, 0, z).normalized;
        // moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // W,A,S,D
        // moveDir.Normalize();

        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        rbody.linearVelocity = Vector3.zero;
    }

    void UpadateAnim()
    {
        animator.SetBool("isMove", moveDir.magnitude > 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ENEMY"))
        {
            enemy = other.gameObject;
            //animator.SetBool("Attack", other.CompareTag("ENEMY"));
            Debug.Log(enemy.name);
            Debug.Log("Enter");
        }
        else if(other.CompareTag("MEAT"))
        {
            meat = other.GetComponent<Resource>();
            
            if(Time.time >= nextTerm)
            {
                Stack(meat, stack1Tr);
                nextTerm = Time.time + term;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            LookAtEnemy();
            AttackEnemy();
        }
        Debug.Log("Stay");
    }

    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("Attack", false);
        enemy = null;
        enemyTr = null;
        Debug.Log("Exit");
    }

    protected void LookAtEnemy()
    {
        if (enemy != null)
        {
            enemyTr = enemy.transform;
            Vector3 direction = enemyTr.position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void AttackEnemy()
    {
        if (enemy != null)
        distance = Vector3.Distance(enemyTr.position, transform.position);

        if (distance <= attackRange)
        {
            animator.SetBool("Attack", true);
            Debug.Log("공격!");
        }
    }

    public void Stack(Resource resource, Transform stack)
    {
        if (num == 0 && resource != null)
        {
            resource.gameObject.transform.SetParent(stack, true);

            //StartCoroutine(MoveToStack(resource));

            resource.gameObject.transform.position = new Vector3(stack.position.x, stack.position.y, stack.position.z);
            resource.gameObject.transform.rotation = stack1Tr.rotation;
            resource.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            num++;

            curResource = resource;
        }
        else if (num > 0 && resource != null)
        {
            resource.count = num;

            resource.gameObject.transform.SetParent(curResource.chain, true);

            //StartCoroutine(MoveToStack(resource));

            resource.gameObject.transform.position = new Vector3(curResource.chain.position.x, curResource.chain.position.y, curResource.chain.position.z);
            resource.gameObject.transform.rotation = curResource.chain.rotation;
            resource.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            num++;

            curResource = resource;
        }
    }

    bool StopToWall()
    {
        //Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        return Physics.Raycast(transform.position + transform.up, transform.forward, 3, LayerMask.GetMask("Structure"));
    }

    public IEnumerable<Resource> EnumerateBackpackTopDown()
    {
        Transform cursor = stack1Tr;           // 첫 고기가 붙어 있는 위치
        while (cursor.childCount > 0)
        {
            var child = cursor.GetChild(0);
            var res = child.GetComponent<Resource>();
            if (res == null) yield break;

            yield return res;                  // 위 → 아래
            cursor = res.chain;               // 다음 고기의 부모 지점
        }
    }

    public void ClearBackpackState()
    {
        num = 0;            // 스택 카운터 초기화
        curResource = null; // 마지막 고기 참조 해제
    }

}
