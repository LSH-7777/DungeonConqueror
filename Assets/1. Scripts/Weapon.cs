using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float attackRange = 1f;
    private float attackPower = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("ENEMY"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ENEMY"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                if (monster != null)
                {
                    float damage = attackPower;
                    monster.curHP -= damage;
                    monster.UpdateProgressBar();
                    monster.SpawnMeat(damage);
                }
            }

        }
    }
}

