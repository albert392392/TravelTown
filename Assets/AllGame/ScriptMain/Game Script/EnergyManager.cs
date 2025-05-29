using UnityEngine;
using TMPro;
using Unity.Collections;
using System.Collections;
using System;

public class EnergyManager : MonoBehaviour {
    public static EnergyManager Instance { get; private set; }

    public int totalEnergys;

    public event Action<int> OnEnergysUpdated;

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
        totalEnergys = PlayerPrefs.GetInt("EnergyCount", totalEnergys);
        OnEnergysUpdated?.Invoke(totalEnergys); // اطلاع‌رسانی مقدار اولیه
    }
    public void SpendEnergy(int amount) {
        if (totalEnergys >= amount) {
            totalEnergys -= amount;
            OnEnergysUpdated?.Invoke(totalEnergys); // به‌روزرسانی UI
        }
        else {
            Debug.LogWarning("Not enough energy!");
        }
    }

    public void AddEnergy(int amount) {
        totalEnergys += amount;
        OnEnergysUpdated?.Invoke(totalEnergys); // به‌روزرسانی UI
    }
    public int GetEnergyCount() {
        return totalEnergys; // بازگرداندن مقدار کنونی سکه‌ها
    }
    public void SaveEnergys() {
        PlayerPrefs.SetInt("EnergyCount", totalEnergys);
        PlayerPrefs.Save();
    }
}