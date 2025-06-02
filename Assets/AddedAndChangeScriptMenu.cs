using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddedAndChangeScriptMenu : MonoBehaviour {
    public HandleObjectClickScript handleObjectClick = null;
    public GameObject choosePanel4 = null;
    public Button clickedButton;
    private bool previousObjectActiveState = true;
    [SerializeField] private bool isIncreasing = true;
    public bool playfunction;
    private bool playfunctionTransparenty;
    private bool isTouched;
    public bool isObjectActive;
    private float touchStartTime;
    private MoveWhenPanelStartedManager moveWhenPanelStartedManager;
    private DraggableObjectCustomizer draggableObjectCustomizer = null;
    [SerializeField] private List<ObjectMenuItem> objectMenuItem;

    public bool isDraggingFromCustomizer = false;
    [Header("Object References")]
    public GameObject[] addedObjs;
    public GameObject[] unableObjs;
    public GameObject objectMenu;
    public GameObject scrollBarMenu;

    [Header("Buttons")]
    public Button btnCorrect;
    public Button btnNotCorrect;
    public Button btnItems;

    [Header("UI & Managers")]
    public ButtonMenuScrollChoose buttonMenuScrollChoose;
    public ObjectMenuScrollBarChoose objectMenuScrollBarChoose;

    [Header("Visual State")]
    public Color currentColor;
    public Color previousColor;
    public Sprite currentSprite;
    public Sprite previousSprite;

    [Header("Keys for Persistence")]
    public string addedChangeKey;
    public string currentSpriteKey;
    public string currentColorKey;
    public string previousSpriteKey;
    public string previousColorKey;

    [Header("Fade Settings")]
    public float fadeSpeed = 0.5f;

    [SerializeField] private List<ObjectMenuItem> objectMenuItems;
    private MoveWhenPanelStartedManager moveManager;

    private bool isToggled;
    private bool playFadeInOut;
    private bool playFullOpaque;

    private bool wasPreviouslyActive = true;

    private void Start() {
        draggableObjectCustomizer = GetComponent<DraggableObjectCustomizer>();
        InitializeManagers();
        LoadSavedState();
        ApplyInitialVisuals();
    }

    private void Update() {
        if (isObjectActive != previousObjectActiveState) {
            SetActiveState(isObjectActive);
            previousObjectActiveState = isObjectActive;
        }

        HandleTouchDrag();
        if (isObjectActive != wasPreviouslyActive) {
            SetActiveState(isObjectActive);
            wasPreviouslyActive = isObjectActive;
        }
        if (playFadeInOut) FadeInOutLoop();
        if (playFullOpaque) ApplyFullOpacity();
    }
    private void InitializeManagers() {
        moveManager = FindAnyObjectByType<MoveWhenPanelStartedManager>();
        buttonMenuScrollChoose = UiManagerMosque.instance.ButtonMenuScrollChoose;
        objectMenuScrollBarChoose = UiManagerMosque.instance.menuScrollBarChoose;
        btnCorrect = buttonMenuScrollChoose.correctButton;
        btnNotCorrect = buttonMenuScrollChoose.notCorrectButton;
    }

    private void LoadSavedState() {
        if (PlayerPrefs.HasKey(addedChangeKey)) {
            isObjectActive = PlayerPrefs.GetInt(addedChangeKey) == 1;
        }
        currentColor = LoadColor(currentColorKey, currentColor);
        previousColor = LoadColor(previousColorKey, previousColor);
        currentSprite = LoadSprite(currentSpriteKey, currentSprite);
        previousSprite = LoadSprite(previousSpriteKey, previousSprite);
    }

    private void ApplyInitialVisuals() {
        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                sr.color = currentColor;
                sr.sprite = currentSprite;
            }
        }
    }

    private void SetActiveState(bool isActive) {
        foreach (var obj in addedObjs) ToggleComponentsRecursively(obj, isActive);
    }

    private void ToggleComponentsRecursively(GameObject obj, bool isActive) {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = isActive;

        if (obj.TryGetComponent(out Collider col)) col.enabled = isActive;
        if (obj.TryGetComponent(out Collider2D col2D)) col2D.enabled = isActive;
        if (obj.TryGetComponent(out Rigidbody2D rb2D)) rb2D.simulated = isActive;
        if (obj.TryGetComponent(out Rigidbody rb)) rb.isKinematic = !isActive;

        foreach (Transform child in obj.transform) ToggleComponentsRecursively(child.gameObject, isActive);
    }

    private void HandleTouchDrag() {

        if (isDraggingFromCustomizer) return;

        if (Input.touchCount == 0)
            return;
        Touch touch = Input.GetTouch(0);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
        worldPosition.z = 0f;

        switch (touch.phase) {
            case TouchPhase.Began:
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                if (hit.collider != null) {
                    foreach (GameObject obj in addedObjs) {
                        if (hit.collider.gameObject == obj) {
                            ChoosePanel4Creator(obj.transform);
                            break;
                        }
                        else if(hit.collider.gameObject != obj && isTouched){
                            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
                            playFadeInOut = false;
                            playFullOpaque = true;
                            ResetUnselected();
                            SaveColor(currentColorKey, currentColor);
                            SaveSprite(currentSpriteKey, currentSprite);
                            moveManager.isStartMove = false;
                            moveManager.isMoveToFirst = true;

                            isToggled = false;
                        }
                    }
                }
                break;
        }
    }
    public void ChoosePanel4Creator(Transform obj) {
        if (!isToggled) {
            //Vector2 scale = new Vector2(draggableObjectCustomizer.newScaleX , draggableObjectCustomizer.newScaleY);
            Transform footThis = obj.GetComponentInChildren<FootDownDraggable>().transform;
            Vector2 footPosition = footThis.position;
            Vector2 newpositionInstantate = new Vector2(footPosition.x , footPosition.y * 1.3f);
            Vector2 positionInstantate = new Vector2(newpositionInstantate.x, newpositionInstantate.y);

            GameObject newObj = Instantiate(UiManagerMosque.instance.choosePanel4, positionInstantate, Quaternion.identity);
            Canvas canvas = newObj.GetComponent<Canvas>();
            canvas.sortingOrder = 10;
            newObj.transform.SetParent(obj ,true);
            handleObjectClick = FindFirstObjectByType<HandleObjectClickScript>();
            handleObjectClick.addedAndChangeScriptMenuObj = GetComponent<AddedAndChangeScriptMenu>();
            ChoosePanelClickerRotation choosePanelClickerRotation = GetComponentInChildren<ChoosePanelClickerRotation>();
            choosePanelClickerRotation.parentMainGameobj = gameObject.transform;
            ChoosePanel4Scaler choosePanel4Scaler = GetComponentInChildren<ChoosePanel4Scaler>();
            choosePanel4Scaler.draggableObjectCustomizer = draggableObjectCustomizer;
        }
        else {
            choosePanel4 = null;
            foreach (var choosePanel in GameObject.FindGameObjectsWithTag("choosePanel4")) {
                Destroy(choosePanel.gameObject);
            }
            playFadeInOut = false;
            playFullOpaque = true;
            ResetUnselected();
            SaveColor(currentColorKey, currentColor);
            SaveSprite(currentSpriteKey, currentSprite);
            moveManager.isStartMove = false;
            moveManager.isMoveToFirst = true;
        }
        isToggled = !isToggled;
    }
    public void HandleObjectClick() {
        if (!isToggled) {
            AssignToMenuItems();
            playFadeInOut = true;
            playFullOpaque = false;
            FadeOutUnselected();
            moveManager.isStartMove = true;
            moveManager.isMoveToFirst = false;
            choosePanel4 = GameObject.FindGameObjectWithTag("choosePanel4");
            Destroy(choosePanel4.gameObject);

            foreach (ObjectMenuAdderClick objMenuAdder in FindObjectsOfType<ObjectMenuAdderClick>()) {
                objMenuAdder.addedAndChangeScriptMenu = GetComponent<AddedAndChangeScriptMenu>();
            }

            btnCorrect.GetComponent<ObjectMenuClickButton>().addedAndChangeScriptMenu = this;
            btnNotCorrect.GetComponent<ObjectMenuClickButton>().addedAndChangeScriptMenu = this;
        }
        else {
            playFadeInOut = false;
            playFullOpaque = true;
            ResetUnselected();
            SaveColor(currentColorKey, currentColor);
            SaveSprite(currentSpriteKey, currentSprite);
            moveManager.isStartMove = false;
            moveManager.isMoveToFirst = true;
        }
        isToggled = !isToggled;
    }

    private void FadeInOutLoop() {
        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                var color = sr.color;
                color.a += (isIncreasing ? 1 : -1) * fadeSpeed * Time.deltaTime;
                if (color.a >= 1f) { color.a = 1f; isIncreasing = false; }
                else if (color.a <= 0.47f) { color.a = 0.47f; isIncreasing = true; }
                sr.color = color;
            }
        }
    }

    private void ApplyFullOpacity() {
        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                var color = sr.color;
                color.a = Mathf.Min(color.a + fadeSpeed * Time.deltaTime, 1f);
                sr.color = color;
            }
        }
    }

    private void FadeOutUnselected() {
        foreach (var obj in unableObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                var color = sr.color;
                color.a = 0.6f;
                sr.color = color;
            }
        }
    }

    private void ResetUnselected() {
        foreach (var obj in unableObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                var color = sr.color;
                color.a = 1f;
                sr.color = color;
            }
        }
    }
    private void AssignToMenuItems() {
        foreach (var item in objectMenuItems) {
            var newObj = Instantiate(objectMenu, objectMenuScrollBarChoose.transform);
            if (newObj.TryGetComponent<Image>(out var img)) {
                item.color.a = 1f;
                img.sprite = item.sprite;
                img.color = item.color;
            }
            if (newObj.TryGetComponent<ObjectMenuAdderClick>(out var adder)) {
                adder.addedAndChangeScriptMenu = this;
            }
        }
    }

    public void OnCorrectClickBtn() {
        previousColor = currentColor;
        previousSprite = currentSprite;
        SaveColor(previousColorKey, previousColor);
        SaveSprite(previousSpriteKey, previousSprite);
        GameObject go = btnItems.gameObject;
        if (go.TryGetComponent<Image>(out var image)) {
            currentColor = image.color;
            currentSprite = image.sprite;
            SaveColor(currentColorKey, currentColor);
            SaveSprite(currentSpriteKey, currentSprite);
        }

        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                sr.color = image.color;
                sr.sprite = image.sprite;
            }
        }

        playFadeInOut = false;
        playFullOpaque = true;
        ResetUnselected();
        moveManager.isStartMove = false;
        moveManager.isMoveToFirst = true;
    }

    public void OnNotCorrectClickBtn() {
        playFadeInOut = false;
        playFullOpaque = true;
        ResetUnselected();

        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                sr.color = previousColor;
                sr.sprite = previousSprite;
            }
        }

        moveManager.isStartMove = false;
        moveManager.isMoveToFirst = true;
    }

    public void OnclickObjectsMenu(Button sender) {
        clickedButton = sender;
        btnItems = sender;

        GameObject go = btnItems.gameObject;
        if (go.TryGetComponent<Image>(out var image)) {

        }

        foreach (var obj in addedObjs) {
            if (obj.TryGetComponent<SpriteRenderer>(out var sr)) {
                sr.color = image.color;
                sr.sprite = image.sprite;
            }
        }
    }

    private void SaveColor(string key, Color color) => PlayerPrefs.SetString(key, $"{color.r},{color.g},{color.b},{color.a}");
    private Color LoadColor(string key, Color fallback) {
        if (!PlayerPrefs.HasKey(key)) return fallback;
        var parts = PlayerPrefs.GetString(key).Split(',');
        return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
    }

    private void SaveSprite(string key, Sprite sprite) {
        if (sprite != null) PlayerPrefs.SetString(key, sprite.name);
    }
    private Sprite LoadSprite(string key, Sprite fallback) {
        if (!PlayerPrefs.HasKey(key)) return fallback;
        var name = PlayerPrefs.GetString(key);
        return Resources.Load<Sprite>(name) ?? fallback;
    }

    [System.Serializable]
    public class ObjectMenuItem {
        public Sprite sprite;
        public Color color;
    }
}
