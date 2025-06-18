using UnityEngine;

public class Resource : MonoBehaviour
{
    public Transform chain;
    public int count;

    public enum meatState
    {
        Raw, Cooked
    }
    public meatState state = meatState.Raw;

}
