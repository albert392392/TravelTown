using UnityEngine;
using System;

public class CoinManager : MonoBehaviour {
    public static CoinManager Instance { get; private set; }

    public int totalCoins;

    // رویداد برای اطلاع‌رسانی تغییر مقدار سکه
    public event Action<int> OnCoinsUpdated;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // حفظ آبجکت هنگام تغییر سین
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // بارگذاری مقدار سکه‌ها از PlayerPrefs
        totalCoins = PlayerPrefs.GetInt("MoneyCount", totalCoins);
        OnCoinsUpdated?.Invoke(totalCoins); // اطلاع‌رسانی مقدار اولیه
    }

    public void AddCoins(int amount) {
        if (amount > 0) {
            totalCoins += amount;
            SaveCoins();
            OnCoinsUpdated?.Invoke(totalCoins); // اطلاع‌رسانی مقدار جدید
        }
    }

    public void SpendCoins(int amount) {
        if (amount > 0 && totalCoins >= amount) {
            totalCoins -= amount;
            SaveCoins();
            OnCoinsUpdated?.Invoke(totalCoins); // اطلاع‌رسانی مقدار جدید
        }
    }

    public int GetCoinCount() {
        return totalCoins; // بازگرداندن مقدار کنونی سکه‌ها
    }

    private void SaveCoins() {
        PlayerPrefs.SetInt("MoneyCount", totalCoins);
        PlayerPrefs.Save();
    }
}
