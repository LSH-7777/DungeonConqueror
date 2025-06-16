using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CashSafe : MonoBehaviour
{
    public Transform[] slots = new Transform[4]; 

    private List<Resource>[] stacks = new List<Resource>[4];
    private Transform[] nextAnchor = new Transform[4];

    private int addCursor = 0;
    private int removeCursor = 0;
    private int cashCount = 0;

    //private readonly float term = 0.1f;
    //private float nextTerm = 0f;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            nextAnchor[i] = slots[i];
            stacks[i] = new List<Resource>();
        }
    }

    public void AddCash(Resource cash)
    {
        Transform anchor = nextAnchor[addCursor];

        cash.transform.SetParent(anchor, true);
        cash.transform.localPosition = Vector3.zero;
        cash.transform.localRotation = Quaternion.identity;

        stacks[addCursor].Add(cash);
        nextAnchor[addCursor] = cash.chain;
        cashCount++;

        addCursor = (addCursor + 1) % slots.Length;
    }

    public bool TryPopCash(out Resource cash)
    {
        int checkedSlots = 0;
        while(checkedSlots < slots.Length)
        {
            if (stacks[removeCursor].Count > 0)
            {
                int last = stacks[removeCursor].Count - 1;
                cash = stacks[removeCursor][last];
                stacks[removeCursor].RemoveAt(last);

                nextAnchor[removeCursor] = (stacks[removeCursor].Count == 0) 
                                            ? slots[removeCursor] : stacks[removeCursor][stacks[removeCursor].Count - 1].chain;


                cash.transform.SetParent(null);
                cash.gameObject.SetActive(false);

                cashCount--;
                removeCursor = (removeCursor + 1) % stacks.Length;
                
                return true;
            }
            removeCursor = (removeCursor + 1) % stacks.Length;
            checkedSlots++;
        }

        cash = null;
        return false;
    }

    public int GetCashCount()
    {
        return cashCount;
    }

    private void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if(col.TryGetComponent(out Player player) && TryPopCash(out Resource cash))
            {
                cash.gameObject.SetActive(true);
                player.StackResource(cash, false);
            }
        }
    }

}
