using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BubbleScript : MonoBehaviour {
    [SerializeField] private GameObject _newGameObject;  // شیء جدید برای جایگزینی
    private Vector3 positionOfthis;  // ذخیره موقعیت شیء
    private Transform parentOfthis;  // ذخیره والد شیء

    public int DiamondCount;  // تعداد الماس (یا هر چیزی که می‌خواهید در اینجا ذخیره کنید)
    private string InformationObject;  // اطلاعات شیء برای نمایش در UI

    private bool isButtonClicked = false;  // آیا دکمه کلیک شده است؟
    private bool isMoneyOnGridInstantiated = false;  // متغیر برای بررسی اینکه آیا شیء MoneyOnGrid قبلاً ساخته شده است یا نه

    private float timeSinceButtonClicked = 0f;  // زمان سپری شده از آخرین کلیک روی دکمه

    void Start() {
        // اضافه کردن لیسنر به دکمه
        if (UIManager.Instance != null && UIManager.Instance.ButtonPin != null) {
            UIManager.Instance.ButtonPin.onClick.AddListener(OnButtonClick);
            Debug.Log("Listener added to ButtonPin.");
        }
        else {
            Debug.LogError("ButtonPin is not assigned in UIManager.");
        }
    }

    // متد برای وقتی که دکمه کلیک می‌شود
    public void OnButtonClick() {
        // بررسی اگر قبلاً کلیک شده باشد
        if (isButtonClicked) {
            Debug.Log("Button already clicked. No further action.");
            return; // اگر قبلاً کلیک شده باشد، تابع را ترک می‌کنیم.
        }

        Debug.Log("OnButtonClick() called.");

        // اگر دکمه کلیک شده است، این بخش اجرا می‌شود
        isButtonClicked = true; // علامت‌گذاری که دکمه کلیک شده است.
                                // کاهش مقدار DiamondCount از UIManager
        if (DiamondManager.Instance.GetDiamondCount() >= 1) {
            DiamondManager.Instance.SpendDiamonds(DiamondCount);
            // ذخیره موقعیت و والد فعلی شیء
            positionOfthis = this.transform.position;
            parentOfthis = this.transform.parent;

            // ایجاد شیء جدید در همان موقعیت
            GameObject newObject = Instantiate(_newGameObject, positionOfthis, Quaternion.identity);

            // تنظیم parent به همان والد قبلی
            newObject.transform.SetParent(parentOfthis);

            // تنظیم موقعیت جدید شیء به همان موقعیت اصلی
            newObject.transform.localPosition = positionOfthis - parentOfthis.position;

            InformationObject = "";
            UIManager.Instance.TextPin.text = InformationObject;

            // حذف شیء اصلی بعد از 3 ثانیه
            Destroy(this.gameObject, 3f);

            // بازگشت دکمه به حالت فعال پس از 3 ثانیه
            Invoke("EnableButton", 3f);  // دکمه پس از 3 ثانیه فعال می‌شود
                                         // ریست کردن تایمر
            timeSinceButtonClicked = 0f;
            Debug.Log("DiamondCount in UIManager decreased");
        }
        else {
            Debug.LogWarning("Not enough diamonds.");
            return;
        }
    }

    private void EnableButton() {
        // فعال کردن دکمه
        UIManager.Instance.ButtonPin.enabled = true;
        Debug.Log("ButtonPin re-enabled.");
    }

    private void Update() {
        // شبیه‌سازی کلیک روی شیء (برای موبایل یا دسکتاپ)
        if (!isButtonClicked) {
            HandleObjectClick(); // اگر دکمه کلیک نشده است، وضعیت کلیک روی شیء را بررسی می‌کنیم
        }

        // اگر دکمه کلیک نشده و 3 ثانیه گذشته باشد، تابع جایگزینی اجرا می‌شود
        if (!isButtonClicked) {
            timeSinceButtonClicked += Time.deltaTime; // افزایش تایمر

            if (timeSinceButtonClicked >= 10f) { // اگر 3 ثانیه گذشت
                ReplaceObjectWithNewIfNotClicked(); // تابع جایگزینی شیء را اجرا می‌کنیم
            }
        }
    }

    // این متد برای زمان‌بندی عمل جایگزینی شیء
    private void StartTimerForReplacement() {
        if (!isMoneyOnGridInstantiated) {
            StartCoroutine(WaitAndReplaceObject());
        }
    }

    // تابعی که به صورت IEnumerator تعریف شده است
    private IEnumerator WaitAndReplaceObject() {
        yield return new WaitForSeconds(10f); // 3 ثانیه منتظر بمانید
        ReplaceObjectWithNewIfNotClicked();
    }

    private void ReplaceObjectWithNewIfNotClicked() {
        // بررسی اینکه آیا دکمه کلیک نشده است و شیء MoneyOnGrid هنوز ساخته نشده است
        if (!isButtonClicked && !isMoneyOnGridInstantiated) {
            Debug.Log("Creating MoneyOnGrid because no button click occurred.");

            // ذخیره موقعیت و والد فعلی شیء
            positionOfthis = this.transform.position;
            parentOfthis = this.transform.parent;

            // ایجاد شیء جدید از UIManager
            GameObject moneyOnGrid = Instantiate(UIManager.Instance.MoneyOnGrid, positionOfthis, Quaternion.identity);

            // تنظیم parent به همان والد قبلی
            moneyOnGrid.transform.SetParent(parentOfthis);

            // تنظیم موقعیت جدید شیء به همان موقعیت اصلی
            moneyOnGrid.transform.localPosition = positionOfthis - parentOfthis.position;

            // علامت‌گذاری که شیء MoneyOnGrid ساخته شده است
            isMoneyOnGridInstantiated = true;

            // نابود کردن شیء اصلی (شیء که اسکریپت BubbleScript روی آن است)
            Destroy(this.gameObject);
        }
        else {
            Debug.Log("MoneyOnGrid has already been created or button was clicked.");
        }
    }

    // این متد برای مدیریت کلیک روی شیء است
    private void HandleObjectClick() {
        // بررسی لمس برای موبایل یا کلیک برای دسکتاپ
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                        ShowInformation(); // نمایش اطلاعات در UI وقتی که روی شیء کلیک می‌شود
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)) // برای دسکتاپ (کلیک با ماوس)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                    ShowInformation(); // نمایش اطلاعات در UI وقتی که روی شیء کلیک می‌شود
                }
            }
        }
    }

    // تابعی برای نمایش اطلاعات روی UI
    private void ShowInformation() {
        // وقتی روی شیء کلیک می‌شود، اطلاعات مربوط به آن را نمایش می‌دهیم
        InformationObject = "Name Object: " + this.gameObject.name + " Diamond Price: " + DiamondCount;
        UIManager.Instance.TextPin.text = InformationObject;

        // فعال کردن دکمه برای انجام عملیات (مثلاً خرید یا تعامل)
        UIManager.Instance.ButtonPin.enabled = true;
    }
}
