using UnityEngine;

public class ObjectDragManager : MonoBehaviour {
    public static ObjectDragManager Instance { get; private set; }
    [SerializeField] private GameObject currentlyDraggingObject;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public bool IsDraggingSomething() {
        return currentlyDraggingObject != null;
    }

    public void SetDragging(GameObject obj) {
        currentlyDraggingObject = obj;
    }

    public void ClearDragging() {
        currentlyDraggingObject = null;
    }

    public GameObject GetCurrentlyDraggingObject() {
        return currentlyDraggingObject;
    }
}