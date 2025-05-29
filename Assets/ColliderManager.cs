using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    private Collider objCollider;

    private void Start()
    {
        objCollider = GetComponent<Collider>();
    }

    public void SetColliderState(bool state)
    {
        if (objCollider != null)
        {
            objCollider.enabled = state;
        }
    }
}
