using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject CustomerNPC;
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
                npc = Instantiate(CustomerNPC, NPCSpawnPoint.position, NPCSpawnPoint.rotation).GetComponent<CustomerNPC>();
                meatShop.AddNPC(npc);

            }
            yield return new WaitForSeconds(1.0f);
        }
    }
    public int GetSpawnCount(CustomerNPC npc)
    {
        spawnCount = FindObjectsOfType<CustomerNPC>().Length;
        return spawnCount;
    }
}
