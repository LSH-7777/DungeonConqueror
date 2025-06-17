using UnityEngine;

public class CookerNPC : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Cooking()
    {
        if (animator != null)
            animator.SetBool("Cooking", true);
    }

    public void StopCooking()
    {
        if(animator != null)
            animator.SetBool("Cooking", false);
    }

}
