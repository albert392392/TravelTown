using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CoinParent : MonoBehaviour
{
    /*
    [System.Serializable]
    public class ChildObject
    {
        public GameObject gameObject;   // مرجع به آبجکت فرزند
        public int number;              // عدد مرتبط با آبجکت فرزند
        public Text textComponent;      // مرجع به کامپوننت Text برای نمایش عدد
        public Button actionButton;     // مرجع به دکمه‌ای که برای هر آبجکت ایجاد می‌شود
    }
   // public string ActionButtonKey = "OnActionButtonClick";
    public List<ChildObject> childObjects = new List<ChildObject>(); // لیست آبجکت‌های فرزند
    public CoinDisplay coinDisplay; // مرجع به CoinDisplay برای بررسی تعداد سکه‌ها
    private void Start()
    {
        DontDestroyOnLoad(gameObject); // این آبجکت بین صحنه‌ها نابود نمی‌شود
        
        // بازیابی وضعیت نابودی از PlayerPrefs
        for (int i = 0; i < childObjects.Count; i++)
        {
            int isDestroyed = PlayerPrefs.GetInt("ChildObjectDestroyed_" + i, 0); // پیش‌فرض 0 یعنی نابود نشده است
            if (isDestroyed == 1)
            {
                if (childObjects[i].actionButton != null)
                {
                    Destroy(childObjects[i].actionButton.gameObject);
                }

                if (childObjects[i].gameObject != null)
                {
                    Destroy(childObjects[i].gameObject);
                }
            }
            else
            {
                UpdateChildText(childObjects[i]);
                CheckAndActivateButton(childObjects[i]);
            }
        }
    }

    // به‌روزرسانی تکست هر آبجکت فرزند بر اساس عدد مرتبط
    private void UpdateChildText(ChildObject child)
    {
        if (child.textComponent != null)
        {
            child.textComponent.text = child.number.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned for the child object: " + child.gameObject.name);
        }
    }

    private void CheckAndActivateButton(ChildObject child)
    {
        if (coinDisplay != null && coinDisplay.GetCoinCount() >= child.number)
        {
            // فعال‌سازی دکمه اگر تعداد سکه کافی باشد
            if (child.actionButton != null)
            {
                child.actionButton.gameObject.SetActive(true);
                child.actionButton.onClick.AddListener(() => OnButtonClick(child));
            }
        }
    }

    // عملیات کلیک روی دکمه یا آبجکت فرزند
    private void OnButtonClick(ChildObject child)
    {
        if (coinDisplay.GetCoinCount() >= child.number)
        {
            coinDisplay.RemoveCoins(child.number);

            int index = childObjects.IndexOf(child);
            PlayerPrefs.SetInt("ChildObjectDestroyed_" + index, 1);
            PlayerPrefs.Save();

            if (child.actionButton != null)
            {
                Destroy(child.actionButton.gameObject);
            }

            if (child.gameObject != null)
            {
                Destroy(child.gameObject);
            }

            // حذف از لیست
            childObjects.Remove(child);

            SaveChildObjectsState();

            if (childObjects.Count > 0)
            {
                CheckAndActivateButton(childObjects[0]);
            }
        }
    }
    public void OnActionButtonClick()
    {
        ChildObjectController childObjectController = GetComponent<ChildObjectController>();
        var childObj = childObjectController.actionButtonKey;
        PlayerPrefs.SetInt(childObj,1);
        PlayerPrefs.Save();
    }
    public void SaveChildObjectsState()
    {
        PlayerPrefs.SetInt("MoneyCount", coinDisplay.coinCount);
        PlayerPrefs.SetInt("ChildObjectCount", childObjects.Count);

        for (int i = 0; i < childObjects.Count; i++)
        {
            PlayerPrefs.SetInt("ChildObjectNumber_" + i, childObjects[i].number);
        }

        PlayerPrefs.Save();
    }

    public void LoadChildObjectsState()
    {
        int objectCount = PlayerPrefs.GetInt("ChildObjectCount", 0);
        int savedCoinCount = PlayerPrefs.GetInt("MoneyCount", 0);

        coinDisplay.SetCoinCount(savedCoinCount);

        for (int i = 0; i < objectCount; i++)
        {
            if (i < childObjects.Count)
            {
                childObjects[i].number = PlayerPrefs.GetInt("ChildObjectNumber_" + i, childObjects[i].number);
                UpdateChildText(childObjects[i]);
            }
        }
    }

    // ذخیره وضعیت فعلی در PlayerPrefs
    private void SaveState()
    {
        PlayerPrefs.SetInt("ChildObjectsCount", childObjects.Count);
        PlayerPrefs.Save();
    }
    */
}
