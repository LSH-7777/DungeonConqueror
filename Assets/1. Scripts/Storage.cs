using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] Transform[] slots = new Transform[4];   // 1~4
    Transform[] nextAnchor;   // 슬롯별 "다음 붙일 곳"

    List<Resource>[] stacks = new List<Resource>[4];

    int addCursor = 0;           // 현재 사용할 슬롯 인덱스
    int removeCursor = 0;

    int meatCount;

    void Awake()
    {
        nextAnchor = new Transform[4];
        
        for (int i = 0; i < 4; i++) 
            nextAnchor[i] = slots[i];

        for (int i = 0; i < 4; i++)
            stacks[i] = new List<Resource>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player))
                StartCoroutine(Unload(player));
        }
    }
    void OnTriggerExit(Collider col)
    {
        if(col.TryGetComponent(out Player player))
            StopCoroutine(Unload(player));

        player.ClearBackpackState();
        //player = null;
    }


    IEnumerator Unload(Player player)
    {
        foreach (Resource res in player.EnumerateBackpackTopDown())
        {
            Transform slotRoot = slots[addCursor];

            res.transform.SetParent(slotRoot, true);
            res.transform.localPosition = Vector3.zero;

            stacks[addCursor].Add(res);        // ★ 칸별 스택 push
            meatCount++;

            addCursor = (addCursor + 1) % 4;   // 다음 칸
            yield return new WaitForSeconds(0.3f);
        }
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
        while (checkedSlots < 4)
        {
            if (stacks[removeCursor].Count > 0)
            {
                int last = stacks[removeCursor].Count - 1;
                meat = stacks[removeCursor][last];
                stacks[removeCursor].RemoveAt(last);

                meatCount--;
                removeCursor = (removeCursor + 1) % 4;
                return true;
            }

            // 현재 칸이 비었으면 다음 칸 검사
            removeCursor = (removeCursor + 1) % 4;
            checkedSlots++;
        }

        meat = null;
        return false;   // 창고에 고기가 없음
    }
}
