using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float attackPower = 2f;
    public float speed = 100f;
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("ENEMY"))
        {
            Monster monster = col.GetComponent<Monster>();
            if (monster != null)
            {
                if (monster.IsDead()) return;

                float damage = attackPower;
                monster.curHP -= damage;
                monster.UpdateProgressBar();
                monster.SpawnMeat(damage);
                monster.PlayHitEffect();
            }
            Destroy(gameObject);
        }
    }
}
