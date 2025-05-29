using UnityEngine;

public class Energy_Anim : MonoBehaviour
{
    public Animator animator;

    public void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("EnergyStart");
        }
        else
        {
            Debug.LogWarning("Animator is not assigned.");
        }
    }
    public void FinishAnimaton()
    {
        if (animator != null)
        {
            animator.SetTrigger("EnergyFinish");
        }
        else
        {
            Debug.LogWarning("Animator is not assigned.");
        }
    }
}
