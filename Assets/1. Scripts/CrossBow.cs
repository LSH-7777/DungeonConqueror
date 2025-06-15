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
            owner.SetAnim().SetBool("OnSight", true);
        }

        return Physics.Raycast(transform.position + transform.up, transform.forward, 15, LayerMask.GetMask("ENEMY"));
    }


    protected override void Attack()
    {

        if(AimEnemy() == true)
        {
            owner.SetAnim().SetBool("Attack", true);

        }
        else
        {
            owner.SetAnim().SetBool("Attack", false);
        }
    }
    public void Fire()
    {
        Instantiate(arrow, spawnPos.position, spawnPos.rotation);
    }
}
