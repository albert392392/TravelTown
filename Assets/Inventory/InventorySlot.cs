using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image image;
    private TextMeshProUGUI textMeshPro;
    private Button button;
    public GameObject GameObjectIn;

    public InventoryManager inventoryManager;

    private void Awake() {
        if(button == null)
            button = GetComponent<Button>();
        if (image == null)
            image = GetComponent<Image>();
        if (textMeshPro == null)
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start() {
        button.onClick.AddListener(BackObjectInScreen);
    }
    public void BackObjectInScreen() {
        if (GameObjectIn != null) {
            if (GridManager.Instance.emptytargetPotations.Count > 0) {
                GridManager.Instance.GameObjectToEmptyPos(GameObjectIn);
                var merge = GameObjectIn.GetComponent<MergeableBase>();
                if (merge != null && merge.originalScale != Vector3.zero) {
                    GameObjectIn.transform.localScale = merge.originalScale;
                }
                GameObjectIn.SetActive(true);
                UIManager.Instance.chooseOver.transform.position = GameObjectIn.transform.parent.position;
                UIManager.Instance.chooseOver.SetActive(true);

                if(inventoryManager != null && inventoryManager.inventorySlots.Contains(GameObjectIn)) {
                    inventoryManager.inventorySlots.Remove(GameObjectIn);
                }


                GameObjectIn = null;
                image.sprite = null;
                image.color = Color.clear;
                textMeshPro.text = string.Empty;
            }
        }
    }
    public void ChangeImage(Sprite sprite , Color color , TextMeshPro textMeshProMain) {
        if (image != null && sprite != null) {
            image.sprite = sprite;
            image.color = color;
            textMeshPro.text = textMeshProMain.text;
        }
    }
}
