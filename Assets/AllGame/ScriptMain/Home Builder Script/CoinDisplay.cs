using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour {
    public TextMeshProUGUI moneyText; // رفرنس به عنصر متنی برای نمایش سکه‌ها

    private void Start() {
        if (CoinManager.Instance != null) {
            // مقدار اولیه سکه‌ها را تنظیم کنید
            UpdateMoneyText(CoinManager.Instance.GetCoinCount());

            // اتصال به رویداد تغییر مقدار سکه
            CoinManager.Instance.OnCoinsUpdated += UpdateMoneyText;
        }
    }

    private void UpdateMoneyText(int totalCoins) {
        if (moneyText != null) {
            moneyText.text = totalCoins.ToString(); // به‌روزرسانی مقدار نمایش داده شده
        }
    }

    private void OnDestroy() {
        if (CoinManager.Instance != null) {
            // جدا شدن از رویداد هنگام نابودی اسکریپت
            CoinManager.Instance.OnCoinsUpdated -= UpdateMoneyText;
        }
    }
}
