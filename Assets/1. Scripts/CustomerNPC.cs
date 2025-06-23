using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CustomerNPC : MonoBehaviour
{
    protected MeatShop meatShop;
    private GameObject meatShopObject;
    
    private GameObject waypoint;
    private Waypoint spot;
    private Waypoint leavePoint;
    
    private NavMeshAgent agent;

    private int curMeat = 0;
    private int MaxMeat = 5;

    // private int cashIdx = 0;

    private Animator animator;
    public int QueueIndex => currentIndex;
    public Slider progress;
    public int currentIndex = 0;

    public GameObject cashObject; 

    private void Awake()
    {
        meatShopObject = GameObject.Find("MeatShop");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meatShop = meatShopObject.GetComponent<MeatShop>();

    }

    void Start()
    {
        if (meatShop.waypoints.Length > 0)
        {
            SetNextDestination();
        }
        progress.value = (float)curMeat / MaxMeat;
    }

    void Update()
    {
        MoveToDestination();
    }

    // 현재 currentIndex에 해당하는 웨이포인트로 경로 설정
    public void SetNextDestination()
    {
        int idx = currentIndex;
        
        agent.SetDestination(meatShop.waypoints[idx ].position);
        animator.SetBool("isMove", true);

        // 새로운 목표로 이동할 때마다 도착 감시 시작
        StartCoroutine(WatchArrival());
    }
    void SetFinalDestination()
    {
        agent.isStopped = false;

        agent.SetDestination(meatShop.leavePoint.position);
        animator.SetBool("isMove", true);

        // 새로운 목표로 이동할 때마다 도착 감시 시작
        StartCoroutine(WatchArrival());
    }

    public void SetQueueIndex(int idx)
    {
        currentIndex = idx;
    }

    void MoveToDestination()
    {

        if (!HasArrived()) return;

          // 다음 목표 설정
        if (currentIndex < meatShop.waypoints.Length - 1 && currentIndex >= 0)
        {
            SetQueueIndex(currentIndex);
            SetNextDestination();
        }
        else if(currentIndex < 0)
        {
             SetFinalDestination();
        }
        else
        {
            // 루프하지 않고 마지막 지점이라면: 멈춤·애니메이션 등 처리
            agent.isStopped = true;
            animator.SetBool("isMove", false);
        }
    }

   public bool HasArrived()
    {
        if (agent.pathPending) return false;                // 아직 경로 계산 중
        if (agent.remainingDistance > agent.stoppingDistance) return false;

        // 남은 거리가 충분히 작고, 더 이상 움직이지 않으면 → 도착
        return !agent.hasPath || agent.velocity.sqrMagnitude == 0f;
    }

    IEnumerator WatchArrival()
    {
        // 코루틴으로 한 번만 체크
        while (!HasArrived()) yield return null;

        // 도착 처리
        agent.isStopped = true;
        animator.SetBool("isMove", false);
    }

    public virtual bool NeedMoreMeat()
    {
        return curMeat < MaxMeat;
    }
    public virtual void ReceiveMeat(Resource m)
    { 
        if(m == null) return;

        curMeat++;
        progress.value = (float)curMeat / MaxMeat;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return agent;
    }

    public void BuyMeatAnimation(bool isBuying)
    {
        animator.SetBool("Buy", isBuying);
    }

    public virtual void GiveMoney()
    {
        Vector3 spawnPos = transform.position + transform.forward * 0.2f + Vector3.up * 1.0f;
        Resource cash = Instantiate(cashObject, spawnPos, transform.rotation).GetComponent<Resource>();
        
        cash.state = Resource.State.Cash;

        meatShop.cashSafe.AddCash(cash);
    }
}
