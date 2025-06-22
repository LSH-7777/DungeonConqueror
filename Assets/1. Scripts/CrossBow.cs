using UnityEngine;

public class CrossBow : Weapon
{
    public GameObject arrow;
    public Transform spawnPos;

    private void Start()
    {
        SetOwner(owner);   
    }

    private void Update()
    {
        AimEnemy();
        Attack();
    }

    private bool AimEnemy()
    {
        if (owner.OnSight() == true)
        {
            owner.GetAnim().SetBool("OnSight", true);
        }

        return Physics.Raycast(transform.position + transform.up, transform.forward, 40, LayerMask.GetMask("ENEMY"));
    }


    protected override void Attack()
    {
        if (AimEnemy() == true)
        {
            owner.GetAnim().SetBool("Attack", true);

        }
        else
        {
            owner.GetAnim().SetBool("Attack", false);
        }
    }
    public void Fire()
    {
        SlashEffect();
        Instantiate(arrow, spawnPos.position, spawnPos.rotation);
    }
}
