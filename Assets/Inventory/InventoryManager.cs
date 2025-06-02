using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro;
public class InventoryManager : MonoBehaviour
{
    public List<GameObject> inventorySlots;
    public List<GameObject> inventoryItems;
    public Transform mergeableParent;
    public bool isInventoryEnabled;
    private void Update() {
        EnableOrDisableInventory();
    }
    public void InventorySlotInformation() {
        int count = Mathf.Min(inventorySlots.Count, inventoryItems.Count);
        for (int i = 0; i < count; i++) {
            Sprite slotSprite = inventorySlots[i].GetComponent<SpriteRenderer>().sprite;
            var sr = inventorySlots[i].GetComponent<SpriteRenderer>();
            TextMeshPro textMeshPro = inventorySlots[i].GetComponentInChildren<TextMeshPro>();
            var slot = inventoryItems[i].GetComponent<InventorySlot>();
            if (sr != null && slot != null) {
                slot.ChangeImage(sr.sprite , sr.color , textMeshPro);
                slot.GameObjectIn = inventorySlots[i];

                slot.inventoryManager = this;
            }
        }
    }
    public void EnableOrDisableInventory() {
        if(inventorySlots.Count == inventoryItems.Count)
        {
            isInventoryEnabled = false;
            InventoryItemDragHandler.Instance.GetComponent<Collider>().enabled = false;
        }
        else {
            isInventoryEnabled = true;
            InventoryItemDragHandler.Instance.GetComponent<Collider>().enabled = true;
        }
    }
}
