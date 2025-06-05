using System.Collections;
using TMPro;
using UnityEngine;

public class DiamondDisplay : MonoBehaviour {
    public static DiamondDisplay Instance { get;private set; }
    public TextMeshProUGUI diamondText;
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
        /* if (DiamondManager.Instance != null) {
             // مقدار اولیه الماس‌ها را تنظیم می‌کنیم
             UpdateDiamondText(DiamondManager.Instance.GetDiamondCount());
             // اتصال به رویداد به‌روزرسانی
             DiamondManager.Instance.OnDiamondsUpdated += UpdateDiamondText;
         }*/
        if (DiamondManager.Instance != null && DiamondManager.Instance.totalDiamonds != int.Parse(diamondText.text)) {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    private void Update() {
        // Check if the coin count has changed and start the animation if needed
        if (DiamondManager.Instance != null && DiamondManager.Instance.totalDiamonds != int.Parse(diamondText.text)) {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    public IEnumerator NumberAnimationCounter() {
        int currentCoins = int.Parse(diamondText.text);
        int targetCoins = DiamondManager.Instance.totalDiamonds;

        // Animate the coin count increment or decrement
        while (currentCoins != targetCoins) {
            if (currentCoins < targetCoins) {
                currentCoins += Mathf.CeilToInt((targetCoins - currentCoins) * 0.1f); // Increase speed by incrementing faster
                if (currentCoins > targetCoins) currentCoins = targetCoins; // Ensure it doesn't overshoot
            }
            else {
                currentCoins -= Mathf.CeilToInt((currentCoins - targetCoins) * 0.1f); // Decrease speed by decrementing faster
                if (currentCoins < targetCoins) currentCoins = targetCoins; // Ensure it doesn't undershoot
            }

            diamondText.text = currentCoins.ToString();
            yield return new WaitForSeconds(0.02f); // Adjusted for faster animation
        }
    }
    /*
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
    }*/
}
