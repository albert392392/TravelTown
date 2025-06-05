using NUnit;
using NUnit.Framework;
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

    private void Start() {
        if (inventoryFullPanel != null && inventoryFullPanel.PanelFull != null) {
            inventoryFullPanel.PanelFull.SetActive(false);
        }
    }

    private void Update() {
        UpdateInventoryState();
    }

    public void UpdateInventoryState() {
        isInventoryEnabled = inventorySlots.Count != inventoryItems.Count - countPaidStage;
    }

    public void UpdateInventorySlotInformation()
    {
        // Reset countPaidStage
        countPaidStage = inventoryItems.Count(item =>
        {
            var slot = item.GetComponent<InventorySlot>();
            return slot != null && slot.IsPaidStage;
        });

        // Update inventory panel data
        if (inventoryFullPanel != null)
        {
            inventoryFullPanel.countInventoryPriceInt = countPaidStage;
            inventoryFullPanel.number_InventoryDescriptionInt = new List<int>();

            foreach (var item in inventoryItems)
            {
                var inventorySlot = item.GetComponent<InventorySlot>();
                var inventoryPaid = item.GetComponent<InventoryPaid>();

                if (inventorySlot != null && inventorySlot.IsPaidStage && inventoryPaid != null)
                {
                    var diamondText = inventoryPaid.text;
                    if (diamondText != null && item.CompareTag("DiamondPrice"))
                    {
                        if (int.TryParse(diamondText.text, out int diamondValue))
                        {
                            inventoryFullPanel.number_InventoryDescriptionInt.Add(diamondValue);
                        }
                        else
                        {
                            Debug.LogWarning("Failed to parse diamond value from text: " + diamondText.text);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Item is not a paid stage or missing InventoryPaid component: " + item.name);
                }
            }

            // Update the number_InventoryDescription field to show the sum of all diamond values
            if (inventoryFullPanel.number_InventoryDescription != null)
            {
                int totalDiamondValue = inventoryFullPanel.number_InventoryDescriptionInt.Sum();
                inventoryFullPanel.number_InventoryDescription.text = totalDiamondValue.ToString();
            }

            Debug.Log("Updated number_InventoryDescriptionInt: " + string.Join(", ", inventoryFullPanel.number_InventoryDescriptionInt));
        }

        // Update inventory slots
        int availableSlots = Mathf.Min(inventorySlots.Count, inventoryItems.Count - countPaidStage);

        for (int i = 0; i < availableSlots; i++)
        {
            var slotRenderer = inventorySlots[i]?.GetComponent<SpriteRenderer>();
            var slotText = inventorySlots[i]?.GetComponentInChildren<TextMeshPro>();
            var itemSlot = inventoryItems[i]?.GetComponent<InventorySlot>();

            if (slotRenderer != null && itemSlot != null)
            {
                itemSlot.ChangeImage(slotRenderer.sprite, slotRenderer.color, slotText);
                itemSlot.GameObjectIn = inventorySlots[i];
                itemSlot.inventoryManager = this;
            }
        }
    }
    public void ShowPanel() {
        if(inventoryFullPanel != null) {
            inventoryFullPanel.ShowPanel();
            Debug.Log("Panel shown with values: " + string.Join(", ", inventoryFullPanel.number_InventoryDescriptionInt));
        }
    }
    [System.Serializable]
    public class InventoryFull {
        public GameObject PanelFull;
        public int countInventoryPriceInt;
        public List<int> number_InventoryDescriptionInt = new List<int>();
        public TextMeshProUGUI countInventoryPrice;
        public TextMeshProUGUI number_InventoryDescription;

        public void ShowPanel() {

            if (PanelFull != null) {
                PanelFull.SetActive(true);
            }

            if (countInventoryPrice != null) {
                countInventoryPrice.text = countInventoryPriceInt.ToString();
            }

            if (number_InventoryDescription != null) {
                number_InventoryDescription.text = string.Join("+", number_InventoryDescriptionInt) + "+";
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