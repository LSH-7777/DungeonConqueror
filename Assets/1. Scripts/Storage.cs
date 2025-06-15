using Mono.Cecil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] Transform[] slots = new Transform[4];   // 1~4
    Transform[] nextAnchor;   // 슬롯별 "다음 붙일 곳"

    List<Resource>[] stacks = new List<Resource>[4];

    int addCursor = 0;           // 현재 사용할 슬롯 인덱스
    int removeCursor = 0;
    int meatCount;

    private int last = 0;

    private readonly float term = 0.1f;
    private float nextTerm = 0f;

    void Awake()
    {
        nextAnchor = new Transform[4];
        
        for (int i = 0; i < 4; i++) 
            nextAnchor[i] = slots[i];

        for (int i = 0; i < 4; i++)
            stacks[i] = new List<Resource>();
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if(col.CompareTag("Player"))
    //    {
    //        if (col.TryGetComponent(out Player player))
    //        {

    //        }

    //            //StartCoroutine(Unload(player));
    //    }
    //}

    //void OnTriggerExit(Collider col)
    //{
    //    if(col.TryGetComponent(out Player player))
    //    {
    //        //StopCoroutine(Unload(player));
    //        player.ClearBackpackState(player.GetCurMeat());
    //    }
    //}

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player))
            {
                UnloadMeat(player);
            }
        }
    }

    private void UnloadMeat(Player player)
    {
        if (player.meatStack.Count > 0)
        {
            last = player.meatStack.Count - 1;
            if (player.meatStack[last].gameObject == null)
            {
                player.meatStack.RemoveAt(last);
                return;
            }
        }

        if (player.meatStack == null)
            return;


        if (player.meatStack != null && player.meatStack.Count > 0)
        {
            if (Time.time >= nextTerm)
            {
                Debug.Log(player.meatStack.Count - 1);

                //player.cashStack[player.cashStack.Count - 1].transform.SetParent(null);
                ReStackMeat(player.meatStack[player.meatStack.Count - 1]);
                
                //Destroy(player.meatStack[player.meatStack.Count - 1].gameObject);
                player.meatStack.RemoveAt(player.meatStack.Count - 1);
                
                // player.ClearBackpackState(player.GetCurResource());

                nextTerm = Time.time + term;
            }
        }
    }

    void ReStackMeat(Resource res)
    {
        Transform anchor = nextAnchor[addCursor];

        res.transform.SetParent(anchor, true);
        res.transform.localPosition = Vector3.zero;
        res.transform.localRotation = Quaternion.identity;

        stacks[addCursor].Add(res);
        nextAnchor[addCursor] = res.chain;
        meatCount++;

        addCursor = (addCursor + 1) % slots.Length;
    }

    public int GetMeatCount() 
    { 
        return meatCount; 
    }

    public void SetMeatCount(int count)
    {
        meatCount = count;
    }

    public bool TryPopMeat(out Resource meat)
    {
        int checkedSlots = 0;
        while (checkedSlots < slots.Length)
        {
            if (stacks[removeCursor].Count > 0)
            {
                int last = stacks[removeCursor].Count - 1;
                meat = stacks[removeCursor][last];
                stacks[removeCursor].RemoveAt(last);

                if (stacks[removeCursor].Count == 0)
                {
                    nextAnchor[removeCursor] = slots[removeCursor];
                }
                else
                {
                    nextAnchor[removeCursor] = stacks[removeCursor][stacks[removeCursor].Count - 1].chain;
                }

                meat.transform.SetParent(null);
                meat.gameObject.SetActive(false);
                Destroy(meat.gameObject);

                meatCount--;
                removeCursor = (removeCursor + 1) % stacks.Length;
                return true;
            }

            // 현재 칸이 비었으면 다음 칸 검사
            removeCursor = (removeCursor + 1) % stacks.Length;
            checkedSlots++;
        }

        meat = null;
        return false;   // 창고에 고기가 없음
    }
}