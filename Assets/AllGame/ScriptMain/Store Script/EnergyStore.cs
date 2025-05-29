using UnityEngine;
using TMPro;

public class EnergyStore : MonoBehaviour
{
    public TextMeshProUGUI energyDisplayText; // مرجع برای نمایش انرژی
    private const string EnergyKey = "EnergyCount"; // کلید ذخیره انرژی
    private int energyCount;

    private void Start()
    {
        // مقدار ذخیره‌شده را دریافت کن و نمایش بده
       energyCount = PlayerPrefs.GetInt(EnergyKey, 0);
       UpdateEnergyText(energyCount);
    }

    // متد برای افزایش انرژی
    public void AddEnergy(int amount)
    {
        energyCount += amount;
        PlayerPrefs.SetInt(EnergyKey, energyCount); // ذخیره در PlayerPrefs
        PlayerPrefs.Save(); // ذخیره مطمئن
        UpdateEnergyText(energyCount);
    }

    // به‌روزرسانی متن نمایش انرژی
    private void UpdateEnergyText(int count)
    {
        if (energyDisplayText != null)
        {
            energyDisplayText.text = count.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned.");
        }
    }
}
