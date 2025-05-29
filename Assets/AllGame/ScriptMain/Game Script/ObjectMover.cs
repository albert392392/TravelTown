using UnityEngine;
using System.Collections;
using UnityEngine.Accessibility;
using TMPro;

public class ObjectMover : MonoBehaviour
{
    //Move 
   [SerializeField] private float moveSpeed = 20.0f;
   [SerializeField] private float separationDistance = 1.0f;
   [SerializeField] private float stopThreshold = 0.05f;

    //Dragging
    private bool isDragging = false;

    //Target
    private Vector3 targetPosition;
    private bool hasReachedTarget = false;

    public void Move(Vector3 pos)
    {
        targetPosition = pos;
        StartCoroutine(MoveToTarget());
    }
    IEnumerator MoveToTarget()
    {
        while (!isDragging && !hasReachedTarget)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > stopThreshold)
            {
                float speedFactor = Mathf.Clamp01(distanceToTarget / separationDistance); // Decrease When DistanceToTarget
                transform.position = Vector3.Lerp(transform.position, targetPosition, speedFactor * moveSpeed * Time.deltaTime);
            }
            else
            {
                hasReachedTarget = true;
                transform.position = targetPosition;
            }
            yield return null;
        }

        if (!isDragging && hasReachedTarget)
        {
            yield return new WaitForSeconds(1.0f);
        }
    }
    public void StopMoving()
    {
        isDragging = true;
        StopAllCoroutines();
    }
}