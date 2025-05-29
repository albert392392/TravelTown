using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
    public float numberLevel;
    public float countLevel;

    public int numberTotal;
    public int counttotal;

    private Image LevelAmount;
    private TextMeshProUGUI TextTotal;
    public float fillAmount;
    private Color fillColor;

    private void Start() {
        GameObject LevelAmountchild = this.gameObject.transform.GetChild(0).gameObject;
        GameObject TextTotalchild = this.gameObject.transform.GetChild(2).gameObject;
        TextTotal = TextTotalchild.GetComponent<TextMeshProUGUI>();
        LevelAmount = LevelAmountchild.GetComponent<Image>();
        fillColor = LevelAmount.color;
        fillAmount = LevelAmount.fillAmount;  // گرفتن مقدار فعلی از LevelAmount.fillAmount

        LoadTextTotal();
        NextLevel();
        NextTotal();
    }

    public void CurrentLevel() {
        // منطق برای سطح فعلی
    }

    public void NextLevel() {
        CurrentLevel();
    }

    public void CurrentTotal() {
        // منطق برای تعداد فعلی
    }

    public void NextTotal() {
        // اگر fillAmount به 1 برسد، numberTotal به مقدار 1 اضافه شده و سپس fillAmount به 0 برمی‌گردد
        if (fillAmount >= 1f) {
            numberTotal += 1;
            fillAmount = 0f;
            LevelAmount.fillAmount = fillAmount;  // مقدار fillAmount را بروزرسانی می‌کنیم
        }

        // نمایش مقدار نهایی
        TextTotal.text = (numberTotal + counttotal).ToString();
        SaveTextTotal();
    }

    private void SaveTextTotal() {
        int textTotalValue = int.Parse(TextTotal.text);
        PlayerPrefs.SetInt("TotalLevel", textTotalValue);
        PlayerPrefs.Save();
    }

    private void LoadTextTotal() {
        if (PlayerPrefs.HasKey("TotalLevel")) {
            int savedValue = PlayerPrefs.GetInt("TotalLevel");
            TextTotal.text = savedValue.ToString();
        }
        else {
            TextTotal.text = "0";
        }
    }
}
