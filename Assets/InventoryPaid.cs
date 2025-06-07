using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPaid : MonoBehaviour {
    public static InventoryPaid Instance { get; private set; }
    public TextMeshProUGUI text;
    [SerializeField] private Button buttonDiamond;
    private InventorySlot inventorySlot;
    [SerializeField] private string nameSavePaid;
    private void Start() {
        // Cache components
        inventorySlot = GetComponent<InventorySlot>();
     
        if (inventorySlot == null || buttonDiamond == null) {
            Debug.LogError("Required components (InventorySlot or Button) are missing.");
            return;
        }

        buttonDiamond.onClick.AddListener(BuyFunc);
        LoadInventoryPaid();
        //  buttonDiamond.onClick.AddListener(NoDiamondPanel);
 
        // Check and destroy diamond if necessary
        CheckAndDestroyDiamond();
    }
    public void InitAndLoad() {
        // Set any required fields here if needed
        LoadInventoryPaid();
    }
    public void LoadInventoryPaid()
    {
        int saved = PlayerPrefs.GetInt(nameSavePaid, 1);
        inventorySlot.IsPaidStage = saved == 1;
     
    }
    private void CheckAndDestroyDiamond() {
        if (!inventorySlot.IsPaidStage) {
            if (transform.childCount > 1) {
                GameObject child = transform.GetChild(1).gameObject;
                Destroy(child);
            }
            else {
                Debug.LogWarning("No child found to destroy.");
            }
        }
    }

    public void NoDiamondPanel() {
        if (int.TryParse(text.text, out int price)) {
            if (DiamondManager.Instance == null) {
                Debug.LogError("DiamondManager.Instance is null. Ensure the DiamondManager singleton is properly initialized.");
                return;
            }

            if (DiamondManager.Instance.totalDiamonds < price) {
                BuyWithMarket();
            }
        }
        else {
            Debug.LogError($"Failed to parse text '{text.text}' to an integer.");
        }
    }

    public void BuyWithMarket() {
        Debug.Log("Market Diamond @@");
        // Add logic to handle market purchase if needed
    }

    public void BuyFunc() {
        Debug.Log("BuyFunc called");

        if (int.TryParse(text.text, out int price)) {
            if (DiamondManager.Instance == null) {
                Debug.LogError("DiamondManager.Instance is null. Ensure the DiamondManager singleton is properly initialized.");
                return;
            }
            if (DiamondManager.Instance.totalDiamonds >= price) {
                DiamondManager.Instance.totalDiamonds -= price;
                inventorySlot.IsPaidStage = false;
                PlayerPrefs.SetInt(nameSavePaid, inventorySlot.IsPaidStage ? 1 : 0);
                PlayerPrefs.Save();

                // Check and destroy diamond after purchase
                CheckAndDestroyDiamond();
            
            }
            else {
                Debug.LogWarning("Not enough diamonds to complete the purchase.");
            }
        }
        else {
            Debug.LogError($"Failed to parse text '{text.text}' to an integer.");
        }
    }
    private void OnApplicationPause(bool pause) {
        LoadInventoryPaid();
    }
    private void OnApplicationQuit() {
        LoadInventoryPaid();
    }
}