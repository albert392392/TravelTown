using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position + new Vector3(-34,18,65);
            transform.position = newPos;
            transform.LookAt(target);
        }
    }
}
