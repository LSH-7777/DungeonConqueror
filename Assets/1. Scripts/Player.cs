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

    private Transform curStack;
    private int num = 0;

    [HideInInspector]
    public Transform stack1Tr;
    [HideInInspector]
    public Transform stack2Tr;
    
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
        // RangeAttack();
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

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("ENEMY"))
        {
            enemy = col.gameObject;
            //animator.SetBool("Attack", other.CompareTag("ENEMY"));
            Debug.Log(enemy.name);
            Debug.Log("Enter");
        }
        else if(col.CompareTag("MEAT"))
        {
            meat = col.GetComponent<Resource>();
            
            if(Time.time >= nextTerm)
            {
                Stack(meat);
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


    public void Stack(Resource resource)
    {
        Transform stack = stack1Tr;

        //if (meat == null) return;

        //if (curMeat == null)     // 첫 고기
        //{
        //    meat.transform.SetParent(stack, true);
        //    meat.transform.position = stack.position;
        //    meat.transform.rotation = stack.rotation;
        //    curMeat = meat;

        //    meatStack.Add(meat);
        //    Debug.Log(meatStack.Count);
        //if (meat.gameObject.GetComponent<Rigidbody>() != null)
        //    meat.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //    return;
        //}

        //meat.transform.SetParent(curMeat.chain, true);
        //meat.transform.position = curMeat.chain.position;
        //meat.transform.rotation = curMeat.chain.rotation;
        //curMeat = meat;

        //meatStack.Add(meat);

        //if (meat.gameObject.GetComponent<Rigidbody>() != null)
        //    meat.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        if (num == 0 && resource != null)
        {
            resource.gameObject.transform.SetParent(stack, true);

            //StartCoroutine(MoveToStack(resource));

            resource.gameObject.transform.position = new Vector3(stack.position.x, stack.position.y, stack.position.z);
            resource.gameObject.transform.rotation = stack.rotation;

            if (resource.gameObject.GetComponent<Rigidbody>() != null)
                resource.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            num++;

            curMeat = resource;
        }
        else if (num > 0 && resource != null)
        {
            resource.count = num;
            resource.gameObject.transform.SetParent(curMeat.chain, true);

            //StartCoroutine(MoveToStack(resource));

            resource.gameObject.transform.position = new Vector3(curMeat.chain.position.x, curMeat.chain.position.y, curMeat.chain.position.z);
            resource.gameObject.transform.rotation = curMeat.chain.rotation;

            if (resource.gameObject.GetComponent<Rigidbody>() != null)
                resource.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            num++;

            curMeat = resource;
        }
    }

    public void StackCash(Resource cash)
    {
        Transform stack = stack2Tr;

        if (cash == null) return;
        
        if (curCash == null)     // 첫 돈
        {
            cash.transform.SetParent(stack, true);
            cash.transform.position = stack.position;
            cash.transform.rotation = stack.rotation;
            curCash = cash;

           cashStack.Add(cash);
           Debug.Log(cashStack.Count);
            return;
        }

        cash.transform.SetParent(curCash.chain, true);
        cash.transform.position = curCash.chain.position;
        cash.transform.rotation = curCash.chain.rotation;
        curCash = cash;
        
        cashStack.Add(cash);

        Debug.Log(cashStack.Count);
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

    public void ClearBackpackState(Resource curResource)
    {
        num = 0;        // 스택 카운터 초기화
        curResource = null; // 마지막 고기 참조 해제
    }

    public Resource GetCurMeat()
    {
        return curMeat;
    }
    public Resource GetCurCash()
    {
        return curCash;
    }

    public Animator SetAnim()
    {
        return animator;
    }

    public bool OnSight()
    {
        Debug.DrawRay(transform.position, transform.forward * 20 + Vector3.up, Color.red);
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
