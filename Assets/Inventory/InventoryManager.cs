using NUnit;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public List<GameObject> inventorySlots;
    public List<GameObject> inventoryItems;
    public Transform mergeableParent;
    public bool isInventoryEnabled;
    public int countPaidStage;
    public InventoryFull inventoryFullPanel;

    private IEnumerator Start() {
        if (inventoryFullPanel != null && inventoryFullPanel.PanelFull != null) {
            inventoryFullPanel.PanelFull.SetActive(false);
        }
        yield return null; // Wait one frame
        yield return null;
        InitializeInventory();
    }
    private void Update() {
        UpdateInventoryState();
    }

    public void UpdateInventoryState() {
        isInventoryEnabled = inventorySlots.Count != inventoryItems.Count - countPaidStage;
    }

    public void InitializeInventory() {
        // Ensure all InventoryPaid scripts are loaded and IsPaidStage is up-to-date
        foreach (var item in inventoryItems) {
            var paid = item.GetComponent<InventoryPaid>();
            if (paid != null)
                paid.InitAndLoad();
        }
        UpdateInventorySlotInformation();
    }
    public void UpdateInventorySlotInformation()
    {

        countPaidStage = inventoryItems.Count(item =>
        {
            var slot = item.GetComponent<InventorySlot>();
            return slot != null && slot.IsPaidStage;
        });

        if (inventoryFullPanel != null) {
            inventoryFullPanel.countInventoryPriceInt = countPaidStage;

            List<string> diamondValues = new List<string>();

            foreach (var item in inventoryItems) {
                var inventorySlot = item.GetComponent<InventorySlot>();
                var inventoryPaid = item.GetComponent<InventoryPaid>();

                if (inventorySlot != null && inventorySlot.IsPaidStage && inventoryPaid != null) {
                    var diamondText = inventoryPaid.text;
                    if (diamondText != null) {
                        if (int.TryParse(diamondText.text, out int diamondValue)) {
                            diamondValues.Add(diamondValue.ToString());
                        }
                        else {
                            Debug.LogWarning("Failed to parse diamond value from text: " + diamondText.text);
                        }
                    }
                }
            }

            // مقدار نهایی به صورت رشته‌ای با + بین مقادیر
            if (inventoryFullPanel.number_InventoryDescription != null) {
                inventoryFullPanel.number_InventoryDescription.text = string.Join("+", diamondValues);
            }
        }

        var unpaidItems = inventoryItems
            .Where(item => { var slot = item.GetComponent<InventorySlot>();return slot != null&& !slot.IsPaidStage; })
            .ToList();

        // Update inventory slots
        int availableSlots = Mathf.Min(inventorySlots.Count,unpaidItems.Count);

        for (int i = 0; i < availableSlots; i++)
        {
            var slot = unpaidItems[i].GetComponent<InventorySlot>();

            if (i < unpaidItems.Count) {

                var slotRenderer = inventorySlots[i]?.GetComponent<SpriteRenderer>();
                var slotText = inventorySlots[i]?.GetComponentInChildren<TextMeshPro>();
                var itemSlot = unpaidItems[i]?.GetComponent<InventorySlot>();

                if (slotRenderer != null && itemSlot != null) {
                    itemSlot.ChangeImage(slotRenderer.sprite, slotRenderer.color, slotText);
                    itemSlot.GameObjectIn = inventorySlots[i];
                    itemSlot.inventoryManager = this;
                }
            }
            else if(slot != null) {
                slot.BackObjectInScreen();
            }

        }
    }
    public void ShowPanel() {
        if(inventoryFullPanel != null) {
            inventoryFullPanel.ShowPanel();
        }
    }
    [System.Serializable]
    public class InventoryFull {
        public GameObject PanelFull;
        public int countInventoryPriceInt;
        public TextMeshProUGUI countInventoryPrice;
        public TextMeshProUGUI number_InventoryDescription;

        public void ShowPanel() {

            if (PanelFull != null) {
                PanelFull.SetActive(true);
            }

            if (countInventoryPrice != null) {
                countInventoryPrice.text = countInventoryPriceInt.ToString();
            }
            
            if (PanelFull != null) {
                PanelFull.GetComponent<MonoBehaviour>().StartCoroutine(TimerDeactivatePanel());
            }
        }

        private IEnumerator TimerDeactivatePanel() {
            yield return new WaitForSeconds(4f);
            if (PanelFull != null) {
                PanelFull.SetActive(false);
            }
        }
    }
}