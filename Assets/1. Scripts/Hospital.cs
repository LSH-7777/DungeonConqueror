using UnityEngine;
using UnityEngine.UI;

public class Hospital : MonoBehaviour
{
    public Text CashText;

    private int curCash = 0;
    private int requiredCash = 500;
    private int payment = 100;

    private float term = 0.1f;
    private float nextTerm = 0f;

    private int last = 0;


    private void Start()
    {
        curCash = requiredCash;
        UpdateText(requiredCash);
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player))
            {
                Pay(player);
            }
        }
    }

    private void Pay(Player player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (!playerHealth.HealthCheck()) return;

        if (player.cashStack.Count > 0)
        {
            last = player.cashStack.Count - 1;
            if (player.cashStack[last].gameObject == null)
            {
                player.cashStack.RemoveAt(last);
                return;
            }
        }

        if (player.cashStack == null)
            return;



        if (player.cashStack != null && player.cashStack.Count > 0)
        {
            if (Time.time >= nextTerm)
            {
                Debug.Log(player.cashStack.Count - 1);

                player.cashStack[player.cashStack.Count - 1].transform.SetParent(null);
                player.cashStack[player.cashStack.Count - 1].gameObject.SetActive(false);
                player.cashStack.RemoveAt(player.cashStack.Count - 1);


                curCash -= payment;
                player.PlayClip(3);

                if (curCash == 0)
                {
                    playerHealth.Healing(playerHealth.GetMaxHealth());
                    curCash = requiredCash;
                }
                UpdateText(curCash);
                nextTerm = Time.time + term;
            }
        }
    }

    void UpdateText(int curCash)
    {
        CashText.text = "남은 액수 : " + curCash.ToString() + " 원";
    }
}
