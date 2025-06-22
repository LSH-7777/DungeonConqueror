using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth = 300f;
    private float curHealth;

    private bool isDead = false;

    private Player player;
    private AudioSource audioSource;

    public Slider hpProgressBar;
    
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip healClip;
    public ParticleSystem hitEffect;
    public ParticleSystem healingEffect;

    public Transform respawnPoint;

    private void Start()
    {
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
        curHealth = maxHealth;
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        curHealth = Mathf.Max(curHealth, 0);
        hpProgressBar.value = curHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(PlayerDead()) return;

        curHealth -= damage;
        PlayHitEffect();
        if (curHealth > 0)
        {
            UpdateProgressBar();
        }
        else
        {
            UpdateProgressBar();
            Dead();
        }
    }

    public void Healing(float heal)
    {
        if (PlayerDead()) return;


        if (curHealth < maxHealth)
        {
            curHealth = Mathf.Clamp(curHealth += heal, 0, maxHealth);
            UpdateProgressBar();
            PlayHealEffect();
        }
        else
        {
            return;
        }
    }

    public bool PlayerDead()
    {
        if(curHealth > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Dead()
    {
        if (PlayerDead() && !isDead)
        {
            player.GetAnim().ResetTrigger("Reset");
            isDead = true;
            audioSource.PlayOneShot(deathClip, 0.5f);
            player.GetAnim().SetTrigger("Dead");

            player.enabled = false;

            Invoke("Reset", 5.0f);
        }
    }

    private void PlayHitEffect()
    {
        if (PlayerDead() == true)
        {
            return;
        }
        // 사운드 재생
        audioSource.PlayOneShot(hitClip, 0.1f);

        // 파티클 효과 재생
        if (hitEffect != null)
        {
            hitEffect.Play();
        }
    }

    private void PlayHealEffect()
    {
        if (PlayerDead() == true)
        {
            return;
        }
        // 사운드 재생
        audioSource.PlayOneShot(healClip, 0.1f);

        // 파티클 효과 재생
        if (healingEffect != null)
        {
            healingEffect.Play();
        }
    }

    public bool HealthCheck()
    {
        return curHealth < maxHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void Reset()
    {
        isDead = false;
        player.ClearBackpack();

        player.enabled = true;
        player.transform.position = respawnPoint.position;
        curHealth = maxHealth;
        UpdateProgressBar();
        player.GetAnim().ResetTrigger("Dead");

        player.GetAnim().SetTrigger("Reset");
        PlayHealEffect();
    }
}
