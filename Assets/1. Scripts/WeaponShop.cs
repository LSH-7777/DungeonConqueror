using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public Text CashText;

    private int curCash = 0;
    private int requiredCash = 1000;
    private int payment = 100;

    private float term = 0.1f;
    private float nextTerm = 0f;

    private int weaponLevel = 1;
    private int last = 0;

    private AudioSource audioSource;

    public AudioClip upgradeClip;
    public ParticleSystem upgradeEffect;
    private void Start()
    {
        curCash = requiredCash;
        UpdateText(requiredCash);
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if (col.TryGetComponent(out Player player))
            {
                Pay(player);
            }
        }
    }

    void UpdateText(int curCash)
    {
        CashText.text = "남은 액수 : " + curCash.ToString() + " 원";
    }

    private void UpgradeWeapon(Player player)
    {
        if (weaponLevel > player.weapons.Length) return;

        if (player.weapons[weaponLevel - 1].gameObject != null && player.weapons[weaponLevel].gameObject != null)
        {
            player.weapons[weaponLevel - 1].gameObject.SetActive(false);
            player.weapons[weaponLevel].gameObject.SetActive(true);
        }
        weaponLevel++;
        PlayUpgradeEffect();
        requiredCash *= weaponLevel;

        curCash = requiredCash;

        switch(weaponLevel)
        {
            case 1 : 
                player.GetAnim().SetBool("Axe", true);
                player.GetAnim().SetBool("CrossBow", false); break;
            case 2:
                player.GetAnim().SetBool("Axe", false);
                player.GetAnim().SetBool("CrossBow", true);
                player.EquipWeapon(player.weapons[weaponLevel - 1]); break;
        }
    }

    private void Pay(Player player)
    {
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
                player.cashStack[player.cashStack.Count - 1].transform.SetParent(null);
                player.cashStack[player.cashStack.Count - 1].gameObject.SetActive(false);
                player.cashStack.RemoveAt(player.cashStack.Count - 1);
                

                curCash -= payment;

                if (curCash == 0)
                {
                    UpgradeWeapon(player);
                }
                UpdateText(curCash);
                player.PlayClip(3);

                nextTerm = Time.time + term;
            }
        }
    }

    private void PlayUpgradeEffect()
    {
        // 사운드 재생
        audioSource.PlayOneShot(upgradeClip);

        // 파티클 효과 재생
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
        }
    }
}
