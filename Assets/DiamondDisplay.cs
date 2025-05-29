using TMPro;
using UnityEngine;

public class DiamondDisplay : MonoBehaviour {
    public TextMeshProUGUI diamondText;

    private void Start() {
        if (DiamondManager.Instance != null) {
            // مقدار اولیه الماس‌ها را تنظیم می‌کنیم
            UpdateDiamondText(DiamondManager.Instance.GetDiamondCount());
            // اتصال به رویداد به‌روزرسانی
            DiamondManager.Instance.OnDiamondsUpdated += UpdateDiamondText;
        }
    }

    private void UpdateDiamondText(int totalDiamonds) {
        if (diamondText != null) {
            diamondText.text = totalDiamonds.ToString(); // به‌روزرسانی مقدار الماس در رابط کاربری
        }
    }

    private void OnDestroy() {
        if (DiamondManager.Instance != null) {
            // جدا شدن از رویداد هنگام نابودی
            DiamondManager.Instance.OnDiamondsUpdated -= UpdateDiamondText;
        }
    }
}
