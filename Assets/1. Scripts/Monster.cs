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
    public Transform spawnPoint;

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
        for(int i = 0; i < damage*1; i++)
        {
            // 생성된 고기를 참조하여 force를 더함
            GameObject m = Instantiate(meat, spawnPoint.transform.position, spawnPoint.transform.rotation);
            Rigidbody rb = m.GetComponent<Rigidbody>();
            
            Vector3 force = new Vector3(Random.Range(-5, 5), Random.Range(5, 15), Random.Range(-5, 5));
            rb.AddForce(force, ForceMode.Impulse);

            //Debug.Log("고기!");
        }
        
    }
}
