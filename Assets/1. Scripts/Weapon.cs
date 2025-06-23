using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float attackRange = 1f;
    private float attackPower = 1f;

    protected Player owner;

    float term = 0.5f;
    float nextTerm = 0f;

    private AudioSource audioSource;
    [SerializeField] private ParticleSystem slashEffect;
    [SerializeField] private AudioClip slashClip;

    public void SetOwner(Player player)
    {
        this.owner = player;
        audioSource = player.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (Time.time >= nextTerm)
        {
            if (col.gameObject.CompareTag("ENEMY"))
            {
                Attack();
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (Time.time >= nextTerm)
        {
            if (col.gameObject.CompareTag("ENEMY"))
            {
                Attack();
                nextTerm = Time.time + term;
            }
        }
    }

    protected virtual void Attack()
    {
        SlashEffect();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ENEMY"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                if (monster != null)
                {
                    if(monster.IsDead()) return;

                    float damage = attackPower;
                    monster.curHP -= damage;
                    monster.UpdateProgressBar();
                    monster.SpawnMeat(damage);
                    monster.PlayHitEffect();
                }
            }
        }
    }

    protected void SlashEffect()
    {
       // 사운드 재생
        audioSource.PlayOneShot(slashClip, 0.3f);

        // 파티클 효과 재생
        if (slashEffect != null)
        {
            slashEffect.Play();
        }
    }
}

