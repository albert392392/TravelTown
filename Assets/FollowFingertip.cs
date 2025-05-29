using UnityEngine;

public class FollowFingertip : MonoBehaviour {
    private bool isDragging = true;
    private Camera mainCamera;

    private void Start() {
        if (UiManagerMosque.instance != null && UiManagerMosque.instance.mainCamera != null) {
            mainCamera = UiManagerMosque.instance.mainCamera;
        }
        else {
            Debug.LogError("UiManagerMosque or mainCamera is missing. Ensure UiManagerMosque is set up correctly.");
        }
    }

    private void Update() {
        if (mainCamera == null) return; // Prevent errors if no main camera

        if (isDragging) {
            if (Input.touchCount > 0) {
                HandleTouch(Input.GetTouch(0));
            }
            else if (Input.GetMouseButton(0)) {
                HandleMouse();
            }
        }
    }

    private void HandleTouch(Touch touch) {
        switch (touch.phase) {
            case TouchPhase.Began:
            case TouchPhase.Moved:
                // Update the object's position to follow the touch
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
                transform.position = new Vector3(worldPosition.x, worldPosition.y, 0); // Keep in 2D plane
                break;

            case TouchPhase.Ended:
                isDragging = false; // Stop dragging when the touch ends
                Destroy(this); // Remove this script after dragging ends
                break;
        }
    }

    private void HandleMouse() {
        // Update the object's position to follow the mouse
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0); // Keep in 2D plane

        if (Input.GetMouseButtonUp(0)) {
            isDragging = false; // Stop dragging when the mouse button is released
            Destroy(this); // Remove this script after dragging ends
        }
    }
}
