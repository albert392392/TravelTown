using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotalLevelDisplay : MonoBehaviour {
    public Image totalLevelImage; // نوار پر شدن
    public TextMeshProUGUI totalLevelText; // نمایش سطح
    public TextMeshProUGUI fillPercentageText; // نمایش درصد مقدار

    private void Start() {
        if (TotalLevelManager.Instance != null) {
            // مقدار‌های اولیه را تنظیم کنید
            UpdateTotalLevelFill(TotalLevelManager.Instance.GetTotalLevelCountFill());
            UpdateTotalLevelIndex(TotalLevelManager.Instance.GetTotalLevelCountIndex());

            // اتصال به رویدادها
            TotalLevelManager.Instance.OnTotalLevelsUpdatedFill += UpdateTotalLevelFill;
            TotalLevelManager.Instance.OnTotalLevelsUpdatedIndex += UpdateTotalLevelIndex;
        }
        else {
            Debug.LogError("TotalLevelManager.Instance is null.");
        }
    }

    private void Update() {
        if (TotalLevelManager.Instance != null) {
            // به‌روزرسانی نمایش مقدار filltotalLevels
            UpdateTotalLevelFill(TotalLevelManager.Instance.GetTotalLevelCountFill());

            // به‌روزرسانی نمایش سطح totalLevelIndex
            UpdateTotalLevelIndex(TotalLevelManager.Instance.GetTotalLevelCountIndex());
        }
    }

    private void UpdateTotalLevelFill(float totalLevelsFill) {
        if (totalLevelImage != null) {
            totalLevelImage.fillAmount = totalLevelsFill / 100f; // تبدیل مقدار به مقیاس 0 تا 1
        }

        if (fillPercentageText != null) {
            fillPercentageText.text = $"{Mathf.FloorToInt(totalLevelsFill)}%"; // نمایش مقدار به صورت درصد
        }
    }

    private void UpdateTotalLevelIndex(int totalLevelsIndex) {
        if (totalLevelText != null) {
            totalLevelText.text = totalLevelsIndex.ToString(); // نمایش لول
        }
    }

    private void OnDestroy() {
        if (TotalLevelManager.Instance != null) {
            // حذف از رویدادها هنگام نابودی
            TotalLevelManager.Instance.OnTotalLevelsUpdatedFill -= UpdateTotalLevelFill;
            TotalLevelManager.Instance.OnTotalLevelsUpdatedIndex -= UpdateTotalLevelIndex;
        }
    }
}
