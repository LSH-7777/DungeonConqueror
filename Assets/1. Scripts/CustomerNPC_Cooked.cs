using UnityEngine;

public class CustomerNPC_Cooked : CustomerNPC
{
    private int curSteak = 0;
    private int MaxSteak = 5;

    public override bool NeedMoreMeat()
    {
        return curSteak < MaxSteak;
    }
    public override void ReceiveMeat(Resource m)
    {
        if (m == null) return;

        curSteak++;
        progress.value = (float)curSteak / MaxSteak;
    }

    public override void GiveMoney()
    {
        for(int i = 0; i < 2; i++)
        {
            Vector3 spwanPos = transform.position + transform.forward * 0.2f + Vector3.up * 1.0f;
            Resource cash = Instantiate(cashObject, spwanPos, transform.rotation).GetComponent<Resource>();

            meatShop.cashSafe.AddCash(cash);
        }
    }
}
