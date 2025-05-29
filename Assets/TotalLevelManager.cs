using System;
using UnityEngine;

public class TotalLevelManager : MonoBehaviour {
    public static TotalLevelManager Instance { get; private set; }

    public float filltotalLevels; // مقدار پر شدن [0, بی‌نهایت]
    public event Action<float> OnTotalLevelsUpdatedFill;

    public int totalLevelIndex; // سطح کلی
    public event Action<int> OnTotalLevelsUpdatedIndex;

    private const float FillThreshold = 100f; // مقدار حداکثر برای هر سطح (100)

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // بارگذاری مقادیر ذخیره‌شده
        filltotalLevels = PlayerPrefs.GetFloat("TotalLevelCountFill", filltotalLevels); // مقدار پیش‌فرض 0
        totalLevelIndex = PlayerPrefs.GetInt("TotalLevelCountIndex", totalLevelIndex); // مقدار پیش‌فرض 0

        // به‌روزرسانی UI در شروع بازی
        OnTotalLevelsUpdatedFill?.Invoke(filltotalLevels % FillThreshold); // فقط مقدار باقی‌مانده
        OnTotalLevelsUpdatedIndex?.Invoke(totalLevelIndex);
    }

    private void Update() {
        // بررسی عبور از مرز 100 و به‌روزرسانی لول
        if (filltotalLevels >= FillThreshold) {
            int levelsToAdd = (int)(filltotalLevels / FillThreshold); // محاسبه تعداد لول‌های جدید
            AddTotalLevelIndex(levelsToAdd); // اضافه کردن لول
            filltotalLevels %= FillThreshold; // مقدار باقی‌مانده کمتر از 100
            OnTotalLevelsUpdatedFill?.Invoke(filltotalLevels); // اطلاع‌رسانی مقدار باقی‌مانده
        }
        SaveValues(); // ذخیره مقادیر
    }

    public void AddTotalLevelsFill(float amount) {
        if (amount > 0f) {
            filltotalLevels += amount;

            OnTotalLevelsUpdatedFill?.Invoke(filltotalLevels); // اطلاع‌رسانی تغییر مقدار
            SaveValues(); // ذخیره مقادیر
        }
    }

    public void AddTotalLevelIndex(int amount) {
        if (amount > 0) {
            totalLevelIndex += amount;

            OnTotalLevelsUpdatedIndex?.Invoke(totalLevelIndex); // اطلاع‌رسانی تغییر سطح
            SaveValues(); // ذخیره مقادیر
        }
    }

    public float GetTotalLevelCountFill() {
        return filltotalLevels % FillThreshold; // بازگرداندن مقدار باقی‌مانده
    }

    public int GetTotalLevelCountIndex() {
        return totalLevelIndex;
    }

    private void SaveValues() {
        PlayerPrefs.SetFloat("TotalLevelCountFill", filltotalLevels);
        PlayerPrefs.SetInt("TotalLevelCountIndex", totalLevelIndex);
        PlayerPrefs.Save();
    }
}
