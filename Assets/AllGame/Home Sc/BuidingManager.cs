using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuidingManager : MonoBehaviour
{
    public GameObject pendingObject;
    private Vector3 pos;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    private void Start() {
        UiManagerMosque.instance.mainCamera = Camera.main;
    }
    void Update()
    {
        if (pendingObject != null) {
            pendingObject.transform.position = pos;

            if(Input.GetMouseButtonDown(0)) {
                PlaceObject();
            }
        }
    }
    public void PlaceObject() {
        pendingObject = null;
    }
    private void FixedUpdate() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out hit,1000,layerMask)) {
            pos = hit.point;
        }
    }
    public void SelectObject(GameObject index ,Transform transform) {
        pendingObject = Instantiate(index,transform.position,transform.rotation);
        pendingObject.transform.SetParent(transform);
    }
}
