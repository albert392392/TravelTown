using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 offset;
    private Transform nearestPlatform = null;

    [SerializeField] private float snapRadius = 2f;
    [SerializeField] private float snapSpeed = 5f;
    private float startY;

    private bool moveToPlatform = false;
    private bool isDragging;

    void Update()
    {
        if (isDragging)
        {
            // Setting Of Drag and Drop Object
            Vector3 newPos = GetMouseWorldPos() + offset;
            transform.position = newPos;
        }

        if (moveToPlatform && nearestPlatform != null)
        {
            // Moving to Center Of Platform
            Vector3 platformCenter = nearestPlatform.GetComponent<Collider>().bounds.center;
            transform.position = Vector3.Lerp(transform.position, platformCenter, snapSpeed * Time.deltaTime);

            // Distance to Center Of Platform
            if (Vector3.Distance(transform.position, platformCenter) < 0.1f)
            {
                transform.position = platformCenter;
                moveToPlatform = false;
            }
        }
    }
    void OnMouseDown()
    {
        Vector3 mousePosition = GetMouseWorldPos();
        offset = (Vector3)gameObject.transform.position - mousePosition;
        isDragging = true;
        startY = transform.position.y;
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (nearestPlatform != null)
        {
            moveToPlatform = true; // Start Moving to Platform
        }
    }

    // Give Mouse in WorldPosition
    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}