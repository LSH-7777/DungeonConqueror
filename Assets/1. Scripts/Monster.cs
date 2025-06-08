using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    public float maxHP = 100.0f;
    public float curHP;

    public float power = 1f;
    public int Lv = 1;
    
    public Slider HPprogressBar;
    public GameObject meat;

    private MonsterAI MonsterAI;

    private Vector3 scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHP *= Lv;
        power *= Lv;
        MonsterAI = GetComponent<MonsterAI>();

        MonsterAI.attackRange *= Lv;

        scale = transform.localScale;
        transform.localScale = new Vector3(scale.x, scale.y, scale.z) * Lv;

        curHP = maxHP;
        HPprogressBar.value = curHP / maxHP;
    }

    public void UpdateProgressBar()
    {
        if(HPprogressBar != null)
        {
            HPprogressBar.value = curHP / maxHP;
        }
    }

    public void SpawnMeat(float damage)
    {
        for(int i = 0; i < damage*3; i++)
        {
            Instantiate(meat, transform.position, transform.rotation);
            Rigidbody rb = meat.GetComponent<Rigidbody>();
            Vector3 force = new Vector3(Random.Range(-10, 10), Random.Range(5, 15), Random.Range(-10, 3));
            rb.AddForce(force, ForceMode.Impulse);

            Debug.Log("고기!");
        }
        
    }
}
