using UnityEngine;
using TMPro;
using System;

public class DiamondManager : MonoBehaviour
{
    public static DiamondManager Instance { get; private set; }

    public int totalDiamonds;

    // رویداد برای اطلاع‌رسانی تغییر مقدار سکه
    public event Action<int> OnDiamondsUpdated;

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
        totalDiamonds = PlayerPrefs.GetInt("DiamondCount", totalDiamonds);
        OnDiamondsUpdated?.Invoke(totalDiamonds); // اطلاع‌رسانی مقدار اولیه
    }

    public void AddDiamonds(int amount) {
        if (amount > 0) {
            totalDiamonds += amount;
            SaveDiamonds();
            OnDiamondsUpdated?.Invoke(totalDiamonds); // اطلاع‌رسانی مقدار جدید
        }
    }
    public void SpendDiamonds(int amount) {
        if (amount > 0 && totalDiamonds >= amount) {
            totalDiamonds -= amount;
            SaveDiamonds();
            OnDiamondsUpdated?.Invoke(totalDiamonds);
        }
        else {
            Debug.LogWarning("Not enough diamonds or invalid amount.");
        }
    }


    public int GetDiamondCount() {
        return totalDiamonds; // بازگرداندن مقدار کنونی سکه‌ها
    }

    private void SaveDiamonds() {
        PlayerPrefs.SetInt("DiamondCount", totalDiamonds);
        PlayerPrefs.Save();
    }
}
