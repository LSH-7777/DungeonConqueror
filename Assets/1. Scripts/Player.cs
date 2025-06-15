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

    [HideInInspector]
    public Transform stack1Tr;
    [HideInInspector]
    public Transform stack2Tr;

    private Resource curRes;

    private Resource meat;
    private Resource curMeat;

    private Resource cash;
    private Resource curCash;

    private Animator animator;
    private GameObject enemy;
    private Transform enemyTr;

    private float distance;
    private readonly float term = 0.05f;
    private float nextTerm = 0f;

    public List<Resource> meatStack = new List<Resource>();
    public List<Resource> cashStack = new List<Resource>();

    public GameObject[] weapons;
    private void Start()
    {
        stack1Tr = stack1.transform;
        stack2Tr = stack2.transform;
        animator = GetComponent<Animator>();
        animator.SetBool("Axe", true);
        EquipWeapon(weapons[0]);
    }

    void Update()
    {
        PlayerMove();
        UpadateAnim();
        StopToWall();
        OnSight();
    }

    void FixedUpdate()
    {
        if (!StopToWall())
        {
            rbody.MovePosition(rbody.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }
    public void EquipWeapon(GameObject weaponObject)
    {
        Weapon weapon = weaponObject.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.SetOwner(this);
        }
    }

    void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal") + fixedJoystick.Horizontal;
        float z = Input.GetAxisRaw("Vertical") + fixedJoystick.Vertical;
        
        moveDir = new Vector3(x, 0, z).normalized;

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

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("ENEMY"))
        {
            enemy = col.gameObject;
        }
        else if(col.CompareTag("MEAT"))
        {
            meat = col.GetComponent<Resource>();
            
            if(Time.time >= nextTerm)
            {
                StackResource(meat, stack1Tr, meatStack);
                nextTerm = Time.time + term;
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("ENEMY"))
        {
            LookAtEnemy();
            AttackEnemy();
        }
        Debug.Log("Stay");
    }

    private void OnTriggerExit(Collider col)
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

    public void StackResource(Resource res, Transform stack, List<Resource> resStack)
    {
        if (res == null) return;

        if (curRes == null)     // 첫 자원
        {
            res.transform.SetParent(stack, true);
            res.transform.position = stack.position;
            res.transform.rotation = stack.rotation;
            curRes = res;

            resStack.Add(res);
            Debug.Log(resStack.Count);
            if (res.gameObject.GetComponent<Rigidbody>() != null)
                res.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            return;
        }

        res.transform.SetParent(curRes.chain, true);
        res.transform.position = curRes.chain.position;
        res.transform.rotation = curRes.chain.rotation;
        curRes = res;

        resStack.Add(res);

        if (res.gameObject.GetComponent<Rigidbody>() != null)
            res.gameObject.GetComponent<Rigidbody>().isKinematic = true;

    }


    bool StopToWall()
    {
        return Physics.Raycast(transform.position + transform.up, transform.forward, 3, LayerMask.GetMask("Structure"));
    }

    public void ClearBackpackState(Resource curResource)
    {
        // num = 0;        // 스택 카운터 초기화
        curResource = null; // 마지막 자원 참조 해제
    }
    public Resource GetCurResource()
    {
        Debug.Log("현재 자원 : " + curRes.name);
        return curRes;
    }

    public Animator SetAnim()
    {
        return animator;
    }

    public bool OnSight()
    {
        return Physics.Raycast(transform.position + transform.up, transform.forward, 20, LayerMask.GetMask("ENEMY"));
    }

    public void RangeAttack()
    {
        CrossBow weapon = weapons[1].GetComponent<CrossBow>();
        if (weapon != null)
        {
            weapon.Fire();
        }
    }
}
