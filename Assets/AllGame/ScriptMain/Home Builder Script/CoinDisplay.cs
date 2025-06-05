using System.Collections;
using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour {
    public static CoinDisplay Instance { get; private set; } // Singleton instance

    public TextMeshProUGUI moneyText; // رفرنس به عنصر متنی برای نمایش سکه‌ها
    /*
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // حفظ آبجکت هنگام تغییر سین
        }
        else {
            Destroy(gameObject);
        }
    }*/
    private void Start() {
        if (CoinManager.Instance != null) {
            // مقدار اولیه سکه‌ها را تنظیم کنید
          //  UpdateMoneyText(CoinManager.Instance.GetCoinCount());

            // اتصال به رویداد تغییر مقدار سکه
          //  CoinManager.Instance.OnCoinsUpdated += UpdateMoneyText;
        }
        if (CoinManager.Instance != null && CoinManager.Instance.totalCoins != int.Parse(moneyText.text)) {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    private void Update()
    {
        // Check if the coin count has changed and start the animation if needed
        if (CoinManager.Instance != null && CoinManager.Instance.totalCoins != int.Parse(moneyText.text))
        {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    public IEnumerator NumberAnimationCounter()
    {
        int currentCoins = int.Parse(moneyText.text);
        int targetCoins = CoinManager.Instance.totalCoins;

        // Animate the coin count increment or decrement
        while (currentCoins != targetCoins)
        {
            if (currentCoins < targetCoins)
            {
                currentCoins += Mathf.CeilToInt((targetCoins - currentCoins) * 0.1f); // Increase speed by incrementing faster
                if (currentCoins > targetCoins) currentCoins = targetCoins; // Ensure it doesn't overshoot
            }
            else
            {
                currentCoins -= Mathf.CeilToInt((currentCoins - targetCoins) * 0.1f); // Decrease speed by decrementing faster
                if (currentCoins < targetCoins) currentCoins = targetCoins; // Ensure it doesn't undershoot
            }

            moneyText.text = currentCoins.ToString();
            yield return new WaitForSeconds(0.02f); // Adjusted for faster animation
        }
    }
    /*
    private void UpdateMoneyText(int totalCoins) {
        if (moneyText != null) {
            moneyText.text = totalCoins.ToString(); // به‌روزرسانی مقدار نمایش داده شده
        }
    }*/
    /*
    private void OnDestroy() {
        if (CoinManager.Instance != null) {
            // جدا شدن از رویداد هنگام نابودی اسکریپت
            CoinManager.Instance.OnCoinsUpdated -= UpdateMoneyText;
        }
    }*/
}
