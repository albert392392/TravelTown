using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    // مرجع به آبجکت درون بازی که باید با انیمیشن مطابقت داشته باشد
    public GameObject matchingGameObjectPrefab;

    private void Start()
    {
        if (matchingGameObjectPrefab == null)
        {
            Debug.LogWarning("Matching GameObject prefab is not assigned!");
        }
    }

    // تابع برای بررسی تطبیق آبجکت درون بازی
    public void CheckAndRemoveMatchingObject()
    {
        if (matchingGameObjectPrefab != null)
        {
            // جستجوی آبجکت مشابه در صحنه بازی
            GameObject matchingObjectInScene = FindMatchingGameObjectInScene(matchingGameObjectPrefab);

            if (matchingObjectInScene != null)
            {
                // حذف هر دو آبجکت (انیمیشن و آبجکت درون بازی)
                Destroy(matchingObjectInScene);
                Destroy(this.gameObject);

                Debug.Log("Matching objects found and removed.");
            }
            else
            {
                Debug.LogWarning("No matching object found in the scene.");
            }
        }
    }

    // متدی برای پیدا کردن آبجکت مشابه در صحنه
    private GameObject FindMatchingGameObjectInScene(GameObject prefab)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == prefab.name)
            {
                return obj; // آبجکت مشابه پیدا شد
            }
        }
        return null; // هیچ آبجکتی پیدا نشد
    }
}
