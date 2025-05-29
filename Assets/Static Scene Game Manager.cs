using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StaticSceneGameManager : MonoBehaviour {

    /*
    public static StaticSceneGameManager Instance { get; set; }

    // DG Move
    [SerializeField] private float moveDurationFinger = 1f;
    [SerializeField] private float returnDurationFinger = 1f;

    private bool isAnimating = false;  // فیلد برای کنترل وضعیت انیمیشن
    private Dictionary<GameObject, Vector3> previousPositions = new Dictionary<GameObject, Vector3>();

    private void Awake() {
        // Ensure only one instance of GameManager exists
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        // بررسی موقعیت آبجکت‌ها و اجرای انیمیشن در صورت تغییر
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>()) {
            if (obj.name.Contains("ObjectName")) // فیلتر آبجکت‌ها بر اساس نام دلخواه
            {
                if (previousPositions.ContainsKey(obj)) {
                    if (previousPositions[obj] != obj.transform.position) // اگر موقعیت تغییر کرده
                    {
                        GameObject nextObj = FindNextObjectWithName(obj.name, obj, null);
                        if (nextObj != null) {
                            MoveFinger(obj, nextObj);
                        }
                        previousPositions[obj] = obj.transform.position; // به‌روز‌رسانی موقعیت
                    }
                }
                else {
                    previousPositions[obj] = obj.transform.position; // ذخیره موقعیت اولیه
                }
            }
        }
    }

    public void MoveFinger(GameObject obj1, GameObject obj2) {
        if (isAnimating) return; // اگر انیمیشن در حال اجراست، تابع را ترک کن

        if (obj1.name == obj2.name) {
            GameObject CreateFinger = Instantiate(UIManager.Instance.HelpFinger, obj1.transform.position, Quaternion.identity);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(CreateFinger.transform.DOMove(obj2.transform.position, moveDurationFinger))
                    .Append(CreateFinger.transform.DOMove(obj1.transform.position, returnDurationFinger))
                    .OnStart(() => isAnimating = true)  // شروع انیمیشن
                    .OnComplete(() => {
                        isAnimating = false;  // پایان انیمیشن
                        Destroy(CreateFinger, 0f); // حذف انگشت پس از انیمیشن

                        // جستجوی آبجکت بعدی با همان نام
                        GameObject nextObj = FindNextObjectWithName(obj1.name, obj1, obj2);
                        if (nextObj != null) {
                            MoveFinger(obj1, nextObj); // اجرای انیمیشن روی آبجکت بعدی
                        }
                    });
        }
        else {
            Debug.Log("obj1 contains SpiderWeb or box, or names do not match. Finger animation will not play.");
        }
    }

    private GameObject FindNextObjectWithName(string name, GameObject currentObj1, GameObject currentObj2) {
        // جستجوی آبجکت بعدی با نام مشابه در صحنه
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>()) {
            if (obj.name == name && obj != currentObj1 && obj != currentObj2) {
                return obj;
            }
        }
        return null; // اگر آبجکت مشابهی پیدا نشد
    }*/
}
