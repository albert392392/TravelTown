using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorScript : MonoBehaviour {
    private GridManager gridManager;

    private void Start() {
        gridManager = FindObjectOfType<GridManager>();  // Get reference to the GridManager
        if (gridManager == null) {
            Debug.LogError("GridManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other) {
        ObjectMerge objectMerge = other.GetComponent<ObjectMerge>();
        if (objectMerge == null) {
            Debug.LogError("ObjectMerge component not found on the colliding object.");
            return;
        }

        if (objectMerge.LastObject != null) {
            // Ensure there are enough empty positions
            if (gridManager.emptytargetPotations.Count < 2) {
                Debug.LogWarning("Not enough empty target positions available!");
                return;
            }
            Transform targetPosition = gridManager.emptytargetPotations[1].transform;
            GameObject newObject = Instantiate(objectMerge.gameObject, targetPosition.position, Quaternion.identity);
            newObject.transform.SetParent(targetPosition);  // Set parent to maintain grid hierarchy
        }
        else {
            Debug.LogWarning("LastObject is null on ObjectMerge.");
        }
        Destroy(this.gameObject,2f);
    }
}
