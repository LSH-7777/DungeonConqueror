using UnityEngine;

public class CrossBow : Weapon
{
    public GameObject arrow;
    public Transform spawnPos;

    private float term = 0.5f;
    private float nextTerm;

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

    private Vector3 EnemyPosition()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position + transform.up * 3, transform.forward, out hit, 15, LayerMask.GetMask("ENEMY")))
        {
            GameObject monsterObject = hit.collider.gameObject;
            Monster monster = monsterObject.GetComponent<Monster>();

            if (monster != null)
            {
                Vector3 hitPoint = hit.point;
                return hitPoint;
            }
        }
        return Vector3.zero;
    }    
    protected override void Attack()
    {

        if(AimEnemy() == true)
        {
            owner.SetAnim().SetBool("Attack", true);
            //if(Time.time > nextTerm)
            //{
            //    Instantiate(arrow, spawnPos.position, spawnPos.rotation);
            //    nextTerm = Time.time + term;
            //}
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
