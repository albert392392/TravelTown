using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;
using System.Collections;

public class DiamondStore : MonoBehaviour
{
    public TextMeshProUGUI diamondDisplayText; // مرجع برای نمایش تعداد الماس‌ها
    public Button giftBoxButton; // مرجع به دکمه GiftBox
    private const string DiamondKey = "DiamondCount"; // کلید ذخیره الماس‌ها
    private int diamondCount;

    private void Start()
    {
        // مقدار ذخیره‌شده را دریافت کن و نمایش بده
        diamondCount = PlayerPrefs.GetInt(DiamondKey, 0);
        UpdateDiamondText(diamondCount);
        CheckGiftBoxActivation();
    }

    // به‌روزرسانی متن نمایش تعداد الماس‌ها
    private void UpdateDiamondText(int count)
    {
        if (diamondDisplayText != null)
        {
            diamondDisplayText.text = count.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned.");
        }
    }

    // فعال یا غیر فعال کردن دکمه GiftBox
    private void CheckGiftBoxActivation()
    {
        if (giftBoxButton != null)
        {
            giftBoxButton.interactable = diamondCount >= 10;
        }
    }

    // متدی برای کلیک بر روی دکمه GiftBox
    public void OnGiftBoxClicked()
    {
        if (diamondCount >= 10)
        {
            // کاهش 10 الماس
            diamondCount -= 10;
            PlayerPrefs.SetInt(DiamondKey, diamondCount);
            PlayerPrefs.Save();
            UpdateDiamondText(diamondCount);

            // افزایش 100 انرژی
            EnergyStore energyStore = FindObjectOfType<EnergyStore>();
            if (energyStore != null)
            {
                energyStore.AddEnergy(100);
            }

            // اجرای انیمیشن از اسکریپت Energy_Anim
            Energy_Anim energyAnim = FindObjectOfType<Energy_Anim>();
            if (energyAnim != null)
            {
                StartCoroutine(WaitAndFinishAnimation(energyAnim, 2f)); // صبر2 ثانیه سپس اجرای FinishAnimation
                energyAnim.PlayAnimation(); // فرض بر اینکه متدی به نام PlayAnimation دارید
            }
            // بررسی دوباره برای فعال/غیرفعال کردن دکمه
            CheckGiftBoxActivation();
        }
    }
    private IEnumerator WaitAndFinishAnimation(Energy_Anim energyAnim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        energyAnim.FinishAnimaton(); // فراخوانی متد FinishAnimation پس از 5 ثانیه
    }
}
