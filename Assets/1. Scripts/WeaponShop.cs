using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public Text CashText;

    private int requiredCash = 1000;
    private int payment = 0;

    private readonly float term = 0.1f;
    private float nextTerm = 0f;

    private void Start()
    {
        UpdateText();
    }


    private void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            Debug.Log("플레이어 접촉");
            if (col.TryGetComponent(out Player player))
            {
                Debug.Log("플레이어 컴포넌트 확인");
                if(player.cashStack == null) return;

                if (player.cashStack != null && player.cashStack.Count > 0)
                {
                    Debug.Log("돈 지불");
                    if (Time.time >= nextTerm)
                    {
                        Debug.Log(player.cashStack.Count - 1);

                        player.cashStack[player.cashStack.Count - 1].transform.SetParent(null);
                        Destroy(player.cashStack[player.cashStack.Count - 1].gameObject);
                        player.cashStack.RemoveAt(player.cashStack.Count - 1);
                        player.ClearBackpackState(player.GetCurCash());

                        payment = 10;
                        requiredCash -= payment;

                        UpdateText();
                        nextTerm = Time.time + term;
                    }
                }
            }
        }
    }

    void UpdateText()
    {
        int CurCash = requiredCash - payment;
        CashText.text = "남은 액수 : " + CurCash.ToString() + " 원";
    }
}
