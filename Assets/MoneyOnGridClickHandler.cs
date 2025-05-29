using TMPro;
using UnityEngine;

public class MoneyOnGridClickHandler : MonoBehaviour {
    private int moneyCount = 2;  // مقدار سکه‌ای که به Coin_count اضافه می‌شود

    void Update() {
        // بررسی لمس صفحه (برای موبایل) یا کلیک ماوس (برای دسکتاپ)
        if (Input.touchCount > 0) { // اگر لمس وجود داشته باشد
            Touch touch = Input.GetTouch(0); // دریافت اولین لمس

            if (touch.phase == TouchPhase.Began) { // وقتی لمس شروع می‌شود
                Ray ray = Camera.main.ScreenPointToRay(touch.position); // ارسال Ray از موقعیت لمس
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) { // اگر Ray با چیزی برخورد کند
                    // بررسی اینکه آیا این برخورد با شیء MoneyOnGrid بوده است
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                        // نابود کردن شیء
                        Destroy(hit.collider.gameObject);

                        // اضافه کردن مقدار moneyCount به Coin_count در UIManager
                        CoinManager.Instance.AddCoins(moneyCount);

                        // نمایش پیام در کنسول
                        Debug.Log("MoneyOnGrid destroyed! Coins added: " + moneyCount);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)) { // برای دسکتاپ (کلیک ماوس)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ارسال Ray از موقعیت کلیک
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) { // اگر Ray با چیزی برخورد کند
                // بررسی اینکه آیا این برخورد با شیء MoneyOnGrid بوده است
                if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                    // نابود کردن شیء
                    Destroy(hit.collider.gameObject);

                    CoinManager.Instance.AddCoins(moneyCount);

                    // نمایش پیام در کنسول
                    Debug.Log("MoneyOnGrid destroyed! Coins added: " + moneyCount);
                }
            }
        }
    }
}
