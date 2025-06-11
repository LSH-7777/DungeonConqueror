using UnityEngine;

public class Discard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("CUSTOMER") || other.CompareTag("MEAT"))
        {
            Destroy(other.gameObject);
        }
    }
}
