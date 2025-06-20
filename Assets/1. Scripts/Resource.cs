using UnityEngine;

public class Resource : MonoBehaviour
{
    public Transform chain;
    public int count;

    public enum State
    {
        Raw, Cooked, Cash
    }
    public State state = State.Raw;

}
