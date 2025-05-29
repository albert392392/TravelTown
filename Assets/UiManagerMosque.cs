using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerMosque : MonoBehaviour {
    public static UiManagerMosque instance { get; private set; }

    public Camera mainCamera; // Ensure this is assigned in the inspector or dynamically
    public GameObject choosePanel4;
    public ObjectMenuScrollBarChoose menuScrollBarChoose;
    public ButtonMenuScrollChoose ButtonMenuScrollChoose;
    public GameObject ScrollBarMenu;
    public SimpleScrollSnap snap;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        if (mainCamera == null) {
            mainCamera = Camera.main;
            if (mainCamera == null) {
                Debug.LogError("Main Camera not found. Assign a camera to UiManagerMosque.");
            }
        }
    }
}
