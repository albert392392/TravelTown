using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button homeButton; // دکمه خانه
    public GameObject prefabObj; // پرفب مورد نظر
    public CoinDisplay coinDisplay; // مرجع به CoinDisplay برای مدیریت سکه‌ها
    public ChildObjectController childObjectController; // مرجع به ChildObjectController

    private void Start()
    {
        // غیرفعال‌سازی prefabObj در ابتدای بازی
        if (prefabObj != null)
        {
            prefabObj.SetActive(false);
        }

        // غیرفعال‌سازی دکمه خانه تا زمانی که شرایط لازم برآورده نشود
        homeButton.interactable = false;

        // افزودن لیسنر برای دکمه خانه
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    public void OnActionButtonClick()
    {
        // فعال‌سازی prefabObj هنگام کلیک بر روی دکمه actionButton
        if (prefabObj != null)
        {
            prefabObj.SetActive(true);
        }

        // ذخیره اطلاعات مربوط به وضعیت Scene
        SaveSceneData();

        // حذف دکمه actionButton
        // فرض بر این است که دکمه actionButton به عنوان یک GameObject به دسترس است
        // Destroy(actionButton.gameObject); // اگر دکمه actionButton در اینجا تعریف شده باشد
    }

    public void OnHomeButtonClick()
    {
        // ذخیره تمامی اطلاعات صحنه در PlayerPrefs
        SaveSceneData();

        // غیرفعال‌سازی دکمه خانه بعد از ذخیره‌سازی
        homeButton.interactable = false; // غیرفعال‌سازی دوباره دکمه خانه
    }

    private void SaveSceneData()
    {
        // ذخیره تعداد سکه‌ها
        //PlayerPrefs.SetInt("MoneyCount", coinDisplay.GetCoinCount());

        // ذخیره اطلاعات مربوط به ChildObjects
        childObjectController.SaveChildObjectsState();

        // سایر اطلاعات مربوط به Scene را نیز ذخیره کنید...

        PlayerPrefs.Save(); // ذخیره‌سازی نهایی
    }

    // فرض بر این است که دکمه actionButton به GameManager اضافه شده و این متد به آن متصل است
    public void SetActionButtonListener(Button actionButton)
    {
        actionButton.onClick.AddListener(OnActionButtonClick);
    }

    private void OnApplicationQuit()
    {
        // ذخیره‌سازی اطلاعات در هنگام خروج از بازی
        SaveSceneData();
    }
}
