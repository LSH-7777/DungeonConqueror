using Mono.Cecil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

                ReStackMeat(player.meatStack[player.meatStack.Count - 1]);
                
                player.meatStack.RemoveAt(player.meatStack.Count - 1);

                player.PlayClip(2);

                nextTerm = Time.time + term;
            }
        }
    }

    void ReStackMeat(Resource meat)
    {
        Transform anchor = nextAnchor[addCursor];

        meat.transform.SetParent(anchor, true);
        meat.transform.localPosition = Vector3.zero;
        meat.transform.localRotation = Quaternion.identity;

        if (meat.GetComponent<Rigidbody>() != null)
            meat.GetComponent<Rigidbody>().isKinematic = true;

        if (meat.GetComponent<BoxCollider>() != null)
            meat.GetComponent<BoxCollider>().enabled = false;

        stacks[addCursor].Add(meat);
        nextAnchor[addCursor] = meat.chain;

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
            List<Resource> stack = stacks[removeCursor];

            while (stack.Count > 0 && stack[^1] == null)
                stack.RemoveAt(stack.Count - 1);

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


                meatCount--;
                removeCursor = (removeCursor + 1) % stacks.Length;
                return true;
            }

            removeCursor = (removeCursor + 1) % stacks.Length;
            checkedSlots++;
        }

        meat = null;
        return false;   // 창고에 고기가 없음
    }
}