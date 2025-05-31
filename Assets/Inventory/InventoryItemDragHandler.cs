using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class InventoryItemDragHandler : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public List<GameObject> gameObjectOff;

    private Button button;

    public Button buttonBack;
    private void Start() {
        button = GetComponent<Button>();
        OnGameObj();
        button.onClick.AddListener(OffGameObj);
        buttonBack.onClick.AddListener(OnGameObj);
    }
    public void OffGameObj() {
        if (inventoryManager != null) {
            inventoryManager.transform.GetChild(0).gameObject.SetActive(true);
        }
        if (gameObjectOff != null && gameObjectOff.Count > 0) {
            foreach (var item in gameObjectOff) {
                item.SetActive(false);
            }
        }
        inventoryManager.InventorySlotInformation();
    }
    public void OnGameObj() {
        if (gameObjectOff != null && gameObjectOff.Count > 0) {
            foreach (var item in gameObjectOff) {
                item.SetActive(true);
            }
        }
        if (inventoryManager != null) {
            inventoryManager.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
