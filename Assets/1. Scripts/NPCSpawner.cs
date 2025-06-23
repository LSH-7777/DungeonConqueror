using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] CustomerNPCs;
    public Transform NPCSpawnPoint;

    private MeatShop meatShop;
    private GameObject meatShopObject;


    private CustomerNPC npc = null;
    private int spawnCount = 0;

    private void Awake()
    {
        meatShopObject = GameObject.Find("MeatShop");
    }

    void Start()
    {
        meatShop = meatShopObject.GetComponent<MeatShop>();
        StartCoroutine(SpawnNPC());
    }

    IEnumerator SpawnNPC()
    {
        while (true)
        {
            if(meatShop.waypoints.Length > GetSpawnCount())
            {
                npc = Instantiate(FindCustomer(), NPCSpawnPoint.position, NPCSpawnPoint.rotation).GetComponent<CustomerNPC>();
                meatShop.AddNPC(npc);
            }
            yield return new WaitForSeconds(0.75f);
        }
    }
    public int GetSpawnCount()
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
