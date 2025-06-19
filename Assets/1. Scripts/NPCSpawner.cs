using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] CustomerNPCs;
    public Transform NPCSpawnPoint;

    private MeatShop meatShop;
    private GameObject meatShopLine;


    private CustomerNPC npc = null;
    private int spawnCount = 0;

    private void Awake()
    {
        meatShopLine = GameObject.Find("MeatShop");
    }

    void Start()
    {
        meatShop = meatShopLine.GetComponent<MeatShop>();
        StartCoroutine(SpawnNPC());
    }

    IEnumerator SpawnNPC()
    {
        while (true)
        {
            if(meatShop.waypoints.Length > GetSpawnCount(npc))
            {
                //Instantiate(CustomerNPC, NPCSpawnPoint.position, NPCSpawnPoint.rotation);
                npc = Instantiate(FindCustomer(), NPCSpawnPoint.position, NPCSpawnPoint.rotation).GetComponent<CustomerNPC>();
                meatShop.AddNPC(npc);
            }
            yield return new WaitForSeconds(0.75f);
        }
    }
    public int GetSpawnCount(CustomerNPC npc)
    {
        spawnCount = FindObjectsOfType<CustomerNPC>().Length;
        return spawnCount;
    }

    public GameObject FindCustomer()
    {
        GameObject npc = null;
        float select = Random.Range(0f, 1f);

        if(select <= 0.8)
        {
            npc = CustomerNPCs[0];
        }
        else
        {
            npc = CustomerNPCs[1];
        }
        return npc;
    }
}
