using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CashSafe : MonoBehaviour
{
    private Player player;

    public Transform[] slots = new Transform[4];
    Transform[] nextAnchor;

    List<Resource>[] stacks = new List<Resource>[4];

    // int addCursor = 0;
    int removeCursor = 0;

    int cashCount;

    void Start()
    {
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider col)
    {
        int last = stacks[removeCursor].Count - 1;

        if (col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player))
            {
                player.Stack(stacks[removeCursor][last], player.stack2Tr);
                TryPopCash(out Resource cash);
            }
        }
    }

    public bool TryPopCash(out Resource cash)
    {
        int checkedSlots = 0;
        while (checkedSlots < 4)
        {
            if (stacks[removeCursor].Count > 0)
            {
                int last = stacks[removeCursor].Count - 1;
                cash = stacks[removeCursor][last];
                stacks[removeCursor].RemoveAt(last);

                cash.transform.SetParent(null);
                cash.gameObject.SetActive(false);

                if (stacks[removeCursor].Count == 0)
                {
                    nextAnchor[removeCursor] = slots[removeCursor];
                }
                else
                {
                    nextAnchor[removeCursor] = stacks[removeCursor][stacks[removeCursor].Count - 1].chain;
                }

                cashCount--;
                removeCursor = (removeCursor + 1) % 4;
                return true;
            }

            removeCursor = (removeCursor + 1) % 4;
            checkedSlots++;
        }

        cash = null;
        return false;
    }  
}
