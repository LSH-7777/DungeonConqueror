using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public Text CashText;

    private int curCash = 0;
    private int requiredCash = 1000;
    private int payment = 100;

    private readonly float term = 0.1f;
    private float nextTerm = 0f;

    private int weaponLevel = 1;

    private void Start()
    {
        curCash = requiredCash;
        UpdateText(requiredCash);
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

                        //player.cashStack[player.cashStack.Count - 1].transform.SetParent(null);
                        Destroy(player.cashStack[player.cashStack.Count - 1].gameObject);
                        player.cashStack.RemoveAt(player.cashStack.Count - 1);
                        player.ClearBackpackState(player.GetCurCash());

                        curCash -= payment;

                        if (curCash == 0)
                        {
                            UpgradeWeapon(player);
                        }
                        UpdateText(curCash);
                        
                        nextTerm = Time.time + term;
                    }
                }
            }
        }
    }

    void UpdateText(int curCash)
    {
        CashText.text = "남은 액수 : " + curCash.ToString() + " 원";
    }

    private void UpgradeWeapon(Player player)
    {      
        if (player.weapons[weaponLevel - 1].gameObject != null && player.weapons[weaponLevel].gameObject != null)
        {
            player.weapons[weaponLevel - 1].gameObject.SetActive(false);
            player.weapons[weaponLevel].gameObject.SetActive(true);
        }
        weaponLevel++;
        requiredCash *= weaponLevel;

        curCash = requiredCash;

        switch(weaponLevel)
        {
            case 1 : 
                player.SetAnim().SetBool("Axe", true);
                player.SetAnim().SetBool("CrossBow", false); break;
            case 2:
                player.SetAnim().SetBool("Axe", false);
                player.SetAnim().SetBool("CrossBow", true); break;

        }
    }
}
