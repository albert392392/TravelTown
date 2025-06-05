using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPaid : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Button button;
    private InventorySlot inventorySlot;
    [SerializeField] private string nameSavePaid;
    
    private void Start() {
        inventorySlot = GetComponent<InventorySlot>();
        button = GetComponent<Button>();
        if ((inventorySlot.IsPaidStage ? 1 : 0) == PlayerPrefs.GetInt(nameSavePaid, inventorySlot.IsPaidStage ? 1 : 0)) {
            if (PlayerPrefs.GetInt(nameSavePaid, inventorySlot.IsPaidStage ? 1 : 0) == 0)
            {
                inventorySlot.IsPaidStage = false;
            }
            else
            {
                inventorySlot.IsPaidStage = true;
            }
        }
        OnEnablePricer();
    }
    private void Update() {
        OnEnablePricer();
        DestoyDiamond();
    }
    private void OnEnablePricer() {
        if(text == null) {
            Debug.LogError("TextMeshProUGUI reference is missing. Please assign it in the Inspector.");
            return;
        }
        if(DiamondManager.Instance == null) {
            Debug.LogError("DiamondManager.Instance is null. Ensure the DiamondManager singleton is properly initialized.");
            return;
        }
        if(int.TryParse(text.text , out int price)) {
            if (DiamondManager.Instance.totalDiamonds >= price) {
                button.onClick.AddListener(BuyFunc);
            }
            else {
                button.onClick.AddListener(NoDiamondPanel);
            }
        }
    }
    public void NoDiamondPanel() {

    }
    public void BuyWithMarket() {

    }
    public void BuyFunc() {
        if(int.TryParse(text.text , out int price)) {
            DiamondManager.Instance.totalDiamonds -= price;
            inventorySlot.IsPaidStage = false;
            PlayerPrefs.SetInt(nameSavePaid , 1);
            PlayerPrefs.Save();
        }
    }
    private void DestoyDiamond() {
        if(!inventorySlot.IsPaidStage) {
            GameObject child = transform.GetChild(1).gameObject;
            Destroy(child);
        }
    }
}
