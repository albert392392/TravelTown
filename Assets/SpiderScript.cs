using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScript : MonoBehaviour
{
    private GameObject objectMerge;
    public int DiamondCount;
    private void Update() {
        HandleObjectClick();
    }
    private void HandleObjectClick() {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
            Vector3 inputPosition = Input.touchCount > 0
                ? (Vector3)Input.GetTouch(0).position
                : Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == this.gameObject) {
                SendSpiderToUIManager();
                FindObjectMerge();
            }
        }
    }

    private void SendSpiderToUIManager() {
        UIManager.Instance.SetCurrentSpider(this);
        Debug.Log("Spider assigned to UIManager.");
    }
    public void FindObjectMerge() {
        var parentTransform = this.transform.parent;
        for (int i = 0; i < parentTransform.childCount; i++) {
            var child = parentTransform.GetChild(i);
            if (child.GetComponent<ObjectMerge>().gameObject) {
                objectMerge = child.gameObject;
                DiamondCount = objectMerge.GetComponent<ObjectMerge>().DiamondCountSpider;
            }
            else {
                print("Not Found ObjectMerge !");
            }
        }
    }
}
