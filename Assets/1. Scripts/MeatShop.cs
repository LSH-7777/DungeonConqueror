using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Collections;
using UnityEngine.AI;

public class MeatShop : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform leavePoint;

    private Queue<CustomerNPC> line = new Queue<CustomerNPC>();

    private readonly float term = 0.3f;
    //private float nextTerm = 0f;

    private const int leaveIndex = -10;

    public Storage storage;
    public CashSafe cashSafe;
    //public NPCSpawner spawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SellLoop());
    }

    public void AddNPC(CustomerNPC npc)
    {
        line.Enqueue(npc);
        npc.SetQueueIndex(line.Count - 1);
    }

    void RemoveNPC()
    {
        if (line.Count == 0)
            return;

        CustomerNPC front = line.Dequeue();
        front.currentIndex = leaveIndex;

        int idx = 0;
        foreach (CustomerNPC n in line)
        {
            n.SetQueueIndex(idx++);
            n.SetNextDestination();
            n.GetNavMeshAgent().isStopped = false;
        }
    }
    IEnumerator SellLoop()
    {
        while (true)
        {
            if (line.Count == 0)
            { 
                yield return null;
                continue; 
            }


            CustomerNPC front = line.Peek(); // 맨 앞

            bool needCooked = front is CustomerNPC_Cooked; // 조리된 고기를 구매하는 NPC 확인
            
            if (!front.HasArrived()) // 도착 확인 
            { 
                yield return null;
                continue; 
            }  

            if (front.NeedMoreMeat())        // curMeat(현재 구매량) < MaxMeat(필요 고기량)
            {
                front.BuyMeatAnimation(true); // 구매 요청 애니메이션

                if (storage.TryPopMeat(needCooked, out Resource meat))
                {
                    if(meat != null)
                        front.ReceiveMeat(meat);        // NPC 애니메이션·카운트 증가
                    
                    yield return new WaitForSeconds(term);
                }
                else
                {
                    // 재고 소진 → 판매 중단
                    yield return null;
                }
            }
            else
            {
                front.BuyMeatAnimation(false);
                front.GiveMoney();
                RemoveNPC();                        // 다 샀으면 줄에서 빠짐
                yield return null;
            }
        }
    }
}
