using UnityEngine;

public class OnDestroyCanvas : MonoBehaviour
{
    public static OnDestroyCanvas Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // حفظ آبجکت هنگام تغییر سین
        }
        else {
            Destroy(gameObject);
        }
    }

}
