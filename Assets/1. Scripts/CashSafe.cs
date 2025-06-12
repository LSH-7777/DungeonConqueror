using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CashSafe : MonoBehaviour
{
    public Transform[] slots = new Transform[4]; 

    private List<Resource>[] stacks = new List<Resource>[4];
    private Transform[] nextAnchor = new Transform[4];

    private int addCusor = 0;
    private int removeCusor = 0;
    private int cashCount = 0;

    private readonly float term = 0.1f;
    private float nextTerm = 0f;

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
        Transform anchor = nextAnchor[addCusor];

        cash.transform.SetParent(anchor, true);
        cash.transform.localPosition = Vector3.zero;
        cash.transform.localRotation = Quaternion.identity;

        stacks[addCusor].Add(cash);
        nextAnchor[addCusor] = cash.chain;
        cashCount++;

        addCusor = (addCusor + 1) % slots.Length;
    }

    public bool TryPopCash(out Resource cash)
    {
        int checkedSlots = 0;
        while(checkedSlots < slots.Length)
        {
            if (stacks[removeCusor].Count > 0)
            {
                int last = stacks[removeCusor].Count - 1;
                cash = stacks[removeCusor][last];
                stacks[removeCusor].RemoveAt(last);

                nextAnchor[removeCusor] = (stacks[removeCusor].Count == 0) 
                                            ? slots[removeCusor] : stacks[removeCusor][stacks[removeCusor].Count - 1].chain;


                cash.transform.SetParent(null);
                cash.gameObject.SetActive(false);

                cashCount--;
                removeCusor = (removeCusor + 1) % stacks.Length;
                
                return true;
            }
            removeCusor = (removeCusor + 1) % stacks.Length;
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
                player.StackCash(cash);
            }
        }
    }

}
