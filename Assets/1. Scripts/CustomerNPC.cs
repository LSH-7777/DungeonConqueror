using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CustomerNPC : MonoBehaviour
{
    private MeatShop meatShop;
    private GameObject meatShopLine;
    
    private GameObject waypoint;
    private Waypoint spot;
    private Waypoint leavePoint;
    
    private NavMeshAgent agent;

    private int curMeat = 0;
    private int MaxMeat = 5;

    private int cashIdx = 0;

    private Animator animator;
    public int QueueIndex => currentIndex;
    public Slider progress;
    public int currentIndex = 0;

    public GameObject cashObject; 

    public event System.Action<CustomerNPC> OnArrivedSpot;

    private void Awake()
    {
        meatShopLine = GameObject.Find("MeatShop");
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        meatShop = meatShopLine.GetComponent<MeatShop>();
        animator = GetComponent<Animator>();

        if (meatShop.waypoints.Length > 0)
        {
            SetNextDestination();
        }
    }

    void Update()
    {
        MoveToDestination();
    }

    // 현재 currentIndex에 해당하는 웨이포인트로 경로 설정
    void SetNextDestination()
    {
        agent.SetDestination(meatShop.waypoints[meatShop.waypoints.Length - currentIndex - 1].position);
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
        //if (agent.pathPending) return; // 경로 계산 중이면 대기

        if (!HasArrived()) return;

        // 목표 지점에 거의 도착했는지 확인
        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
            // 다음 목표 설정
            if (currentIndex < meatShop.waypoints.Length - 1 && currentIndex >= 0)
            {
                SetQueueIndex(currentIndex);
                // currentIndex++;
                SetNextDestination();
            }
            else if(currentIndex == -1)
            {
                SetFinalDestination();
            }
            else
            {
                // 루프하지 않고 마지막 지점이라면: 멈춤·애니메이션 등 처리
                agent.isStopped = true;
                animator.SetBool("isMove", false);
            }
       //}
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

        // 외부에 알림
        OnArrivedSpot?.Invoke(this);
    }

    public bool NeedMoreMeat()
    {
        return curMeat < MaxMeat;
    }
    public void ReceiveMeat(Resource m)
    { 
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

    public void GiveMoney()
    {
        Vector3 spwanPos = transform.position + transform.forward * 0.2f + Vector3.up * 1.0f;
        Resource cash = Instantiate(cashObject, spwanPos, transform.rotation).GetComponent<Resource>();
        
        meatShop.cashSafe.AddCash(cash);
    }
}
