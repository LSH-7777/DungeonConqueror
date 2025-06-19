using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float attackRange = 1f;
    private float attackPower = 1f;

    protected Player owner;

    float term = 1f;
    float nextTerm = 0f;

    public void SetOwner(Player player)
    {
        this.owner = player;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("ENEMY"))
        {
            Attack();
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if(Time.time >= nextTerm)
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ENEMY"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                Debug.Log(monster);
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

