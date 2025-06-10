using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject CustomerNPC;
    public Transform NPCSpawnPoint;

    private MeatShop meatShop;
    private GameObject meatShopLine;

    private int spawnCount = 0;

    private void Awake()
    {
        meatShopLine = GameObject.Find("CustomerLine");
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
            if(meatShop.waypoints.Length > spawnCount)
            {
                //Instantiate(CustomerNPC, NPCSpawnPoint.position, NPCSpawnPoint.rotation);
                CustomerNPC npc = Instantiate(CustomerNPC, NPCSpawnPoint.position, NPCSpawnPoint.rotation).GetComponent<CustomerNPC>();

                meatShop.AddNPC(npc);

                spawnCount++;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
    public int GetSpawnCount()
    {
        return spawnCount;
    }
    public int SetSpawnCount()
    {
        return spawnCount - 1;
    }
}
