using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddedAndChangeScriptMenu : MonoBehaviour {
    public GameObject[] addedObjs; // لیست آبجکت‌ها
    public GameObject[] unableObjs;
    public float fadeSpeed = 0.5f; // سرعت تغییر شفافیت
    public Button btnCorrect;
    public Button btnNotCorrect;
    public Button btnItems;
    public GameObject scrollBarMenu;
    public ButtonMenuScrollChoose buttonMenuScrollChoose;
    public ObjectMenuScrollBarChoose objectMenuScrollBarChoose;
    public HandleObjectClickScript handleObjectClick = null;
    public GameObject choosePanel4 = null;
    public GameObject objectMenu;
    // private bool canvasEnable;
    public Button clickedButton; // دکمه‌ای که کلیک شده
    private bool previousObjectActiveState = true; // ذخیره وضعیت قبلی isObjectActive
    public Color previousColor; // ذخیره رنگ قبلی برای بازگردانی
    public Color currentColor;

    public Sprite previousSprite;
    public Sprite currentSprite;

    [SerializeField] private bool isIncreasing = true; // حالت تغییر مقدار ترنسپرنتی (افزایش یا کاهش)
    public bool playfunction;
    private bool playfunctionTransparenty;
    public bool isToggled; // متغیر برای مدیریت حالت کلیک‌ها
    private bool isTouched;
    public bool isObjectActive;

    public string addedChangeKey;
    public string currentSpriteKey;
    public string currentColorKey;
    public string previousSpriteKey;
    public string previousColorKey;
    private float touchStartTime;
    private MoveWhenPanelStartedManager moveWhenPanelStartedManager;
    private DraggableObjectCustomizer draggableObjectCustomizer = null;
    public List<ObjectMenuAdderClick> objectMenuAdderClick;
    [SerializeField] private List<ObjectMenuItem> objectMenuItem;

    public bool isDraggingFromCustomizer = false;
    /*
    private void Awake() {
        objectMenuScrollBarChoose = FindAnyObjectByType<ObjectMenuScrollBarChoose>();
        objectMenu = objectMenuScrollBarChoose.objectMenu;
    }*/

    private void Start() {
        draggableObjectCustomizer = GetComponent<DraggableObjectCustomizer>();

        moveWhenPanelStartedManager = FindAnyObjectByType<MoveWhenPanelStartedManager>();
        buttonMenuScrollChoose = UiManagerMosque.instance.ButtonMenuScrollChoose;
        objectMenuScrollBarChoose = UiManagerMosque.instance.menuScrollBarChoose;
        btnCorrect = UiManagerMosque.instance.ButtonMenuScrollChoose.correctButton;
        btnNotCorrect = UiManagerMosque.instance.ButtonMenuScrollChoose.notCorrectButton;

        // scrollBarMenu.gameObject.SetActive(false);
        playfunction = false;
        if (PlayerPrefs.HasKey(addedChangeKey)) { // بررسی وجود مقدار
            isObjectActive = PlayerPrefs.GetInt(addedChangeKey) == 1; // مقداردهی مجدد
        }

        currentColor = LoadColor(currentColorKey, currentColor);
        previousColor = LoadColor(previousColorKey, previousColor);

        currentSprite = LoadSprite(currentSpriteKey, currentSprite);
        previousSprite = LoadSprite(previousSpriteKey, previousSprite);

        // 🔹 اعمال رنگ و اسپرایت ذخیره‌شده روی تمام `addedObjs`
        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    spriteRenderer.color = currentColor;
                    if (currentSprite != null) {
                        spriteRenderer.sprite = currentSprite;
                    }
                }
            }
        }

        /*
        foreach (GameObject obj in addedObjs) {
            if (obj.GetComponent<DraggableObjectCustomizer>() != null && obj.GetComponent<DraggableObjectCustomizer>().) {
                var draggbleObj = obj.gameObject.GetComponent<AddedAndChangeScriptMenu>();
                if (draggbleObj != null) {
                    draggbleObj.enabled = false;
                }
            }
        }*/
        /*
        if (PlayerPrefs.HasKey("isObjectActiveAddedAndChangeScriptMenu_CanvasEnable")) { // بررسی وجود مقدار
            canvasEnable = PlayerPrefs.GetInt("isObjectActiveAddedAndChangeScriptMenu_CanvasEnable") == 1; // مقداردهی مجدد
        }*/
    }

    private void Update() {
        // فقط زمانی که مقدار isObjectActive تغییر کند، ToggleComponents فراخوانی شود  
        if (isObjectActive != previousObjectActiveState) {
            SetActiveState(isObjectActive);
            previousObjectActiveState = isObjectActive;
        }

        HandleTouchDrag();

        if (playfunction) {
            ChangeTransParentyInLoop();
        }

        if (playfunctionTransparenty) {
            FullTransparenty();
        }
    }
    public void SetActiveState(bool isActive) {
        ToggleComponents(this.gameObject, isActive);
    }
    private void ToggleComponents(GameObject obj, bool isActive) {
        // فعال یا غیرفعال کردن SpriteRenderer
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.enabled = isActive;
        }
        // فعال یا غیرفعال کردن Collider
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) {
            collider.enabled = isActive;
        }
        Collider2D collider2D = obj.GetComponent<Collider2D>();
        if (collider2D != null) {
            collider2D.enabled = isActive;
        }
        // فعال یا غیرفعال کردن Rigidbody
        Rigidbody2D rb2D = obj.GetComponent<Rigidbody2D>();
        if (rb2D != null) {
            rb2D.simulated = isActive;
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = !isActive;
        }

        foreach (Transform child in obj.transform) {
            ToggleComponents(child.gameObject, isActive);
        }
    }
    private void ChangeTransParentyInLoop() {
        // حلقه‌ای برای عبور از تمام آبجکت‌ها در لیست
        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    Color color = sprite.color;

                    // افزایش یا کاهش مقدار شفافیت بر اساس حالت
                    if (isIncreasing) {
                        color.a += fadeSpeed * Time.deltaTime;
                        if (color.a >= 1f) { // مقدار 1f معادل شفافیت 255 است
                            color.a = 1f;
                            isIncreasing = false; // تغییر حالت به کاهش
                        }
                    }
                    else {
                        color.a -= fadeSpeed * Time.deltaTime;
                        if (color.a <= 0.47f) { // مقدار 0.47f معادل شفافیت 120 است
                            color.a = 0.47f;
                            isIncreasing = true; // تغییر حالت به افزایش
                        }
                    }

                    // اعمال مقدار جدید به رنگ
                    sprite.color = color;
                }
            }
        }
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

                            playfunctionTransparenty = true;
                            playfunction = false;
                            OnableObjsTransparenty();
                            FullTransparenty();
                            moveWhenPanelStartedManager.isStartMove = false;
                            moveWhenPanelStartedManager.isMoveToFirst = true;
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
            isToggled = true;
        }
        else {
            choosePanel4 = null;
            foreach (var choosePanel in GameObject.FindGameObjectsWithTag("choosePanel4")) {
                Destroy(choosePanel.gameObject);
            }
            playfunctionTransparenty = true;
            playfunction = false;
            OnableObjsTransparenty();
            FullTransparenty();
            moveWhenPanelStartedManager.isStartMove = false;
            moveWhenPanelStartedManager.isMoveToFirst = true;
            isToggled = false;
        }
    }
    public void HandleObjectClick() {
        isTouched = true;
        if (!isToggled) {
            // objectMenuAdderClick.addedAndChangeScriptMenu = GetComponent<AddedAndChangeScriptMenu>();
            RequestTothePanelChooseObjectAdd();
            playfunctionTransparenty = false;
            playfunction = true;
            ChangeTransParentyInLoop();
            UnableObjsTransparenty();
            //scrollBarMenu = FindAnyObjectByType<scroll_Into>().gameObject;
            //moveWhenPanelStartedManager.ScrollBar = scrollBarMenu.gameObject;
            moveWhenPanelStartedManager.isStartMove = true;
            moveWhenPanelStartedManager.isMoveToFirst = false;
            isToggled = true;
            choosePanel4 = GameObject.FindGameObjectWithTag("choosePanel4");
            Destroy(choosePanel4.gameObject);
        }
        else {
            playfunctionTransparenty = true;
            playfunction = false;
            OnableObjsTransparenty();
            FullTransparenty();
            moveWhenPanelStartedManager.isStartMove = false;
            moveWhenPanelStartedManager.isMoveToFirst = true;
            isToggled = false;
        }
    }
    private void RequestTothePanelChooseObjectAdd() {
        foreach (ObjectMenuItem objMenu in objectMenuItem) {
            // ساخت آبجکت جدید
            GameObject newMenuItem = Instantiate(objectMenu, objectMenuScrollBarChoose.transform);

            // تغییرات روی تصویر دکمه جدید
            Image imageComponent = newMenuItem.GetComponent<Image>();
            if (imageComponent != null) {
                objMenu.color.a = 255f; // تنظیم شفافیت رنگ
                imageComponent.sprite = objMenu.sprite;
                imageComponent.color = objMenu.color;
            }
            
            ObjectMenuAdderClick allMenuAdderClick = newMenuItem.GetComponent<ObjectMenuAdderClick>();
            allMenuAdderClick.addedAndChangeScriptMenu = GetComponent<AddedAndChangeScriptMenu>();
            /*
            Button newButton = newMenuItem.GetComponent<Button>();
            if (newButton != null) {
                newButton.onClick.RemoveAllListeners(); // حذف لیسنرهای قبلی
                newButton.onClick.AddListener(() => OnclickObjectsMenu(newButton));
            }*/
        }
    }

    /*
    private void ClearRequestPanelObjs() {
        for (int i = 0; i < objectMenuScrollBarChoose.transform.childCount; i++) {
             Destroy(objectMenuScrollBarChoose.transform.GetChild(i).gameObject);
        }
    }*/
    private void UnableObjsTransparenty() {
        foreach (GameObject obj in unableObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    Color color = sprite.color;
                    color.a = 0.6f; // مقدار شفافیت
                    sprite.color = color;
                }
            }
        }
    }

    private void OnableObjsTransparenty() {
        foreach (GameObject obj in unableObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    Color color = sprite.color;
                    color.a = 1f; // مقدار شفافیت نهایی
                    sprite.color = color;
                }
            }
        }
    }
    private void RefreshButtonListeners() {
        // ابتدا لیسنرهای قدیمی را حذف کنید
        btnCorrect.onClick.RemoveAllListeners();
        btnNotCorrect.onClick.RemoveAllListeners();
        btnItems.onClick.RemoveAllListeners();

        // سپس لیسنرهای جدید را اضافه کنید
        btnCorrect.onClick.AddListener(() => OnCorrectClickBtn(btnCorrect));
        btnNotCorrect.onClick.AddListener(() => OnNotCorrectClickBtn(btnNotCorrect));
        btnItems.onClick.AddListener(() => OnclickObjectsMenu(btnItems));
    }

    public void OnCorrectClickBtn(Button sender) {
        clickedButton = sender;
        btnCorrect = sender;
        Debug.Log($"Correct button clicked: {btnCorrect.name}");
        // مقداردهی مجدد دکمه‌ها
        RefreshButtonListeners();

        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    SaveColorUnableObjects(obj, sprite.sprite, sprite.color);
                    SaveCurrentState();
                    previousSprite = currentSprite;
                    previousColor = currentColor;
                    playfunctionTransparenty = true;
                    moveWhenPanelStartedManager.isStartMove = false;
                    moveWhenPanelStartedManager.isMoveToFirst = true;
                    FullTransparenty();
                   /* var draggbleObj = obj.gameObject.GetComponent<AddedAndChangeScriptMenu>();
                    if (draggbleObj != null) {
                        draggbleObj.enabled = false;
                    }*/
                }
            }
        }

        playfunction = false;
        OnableObjsTransparenty();
        scrollBarMenu.SetActive(false);
        isToggled = false;
    }

    public void OnNotCorrectClickBtn(Button sender) {
        clickedButton = sender;
        btnNotCorrect = sender;
        Debug.Log($"Not Correct button clicked: {btnNotCorrect.name}");
        // مقداردهی مجدد دکمه‌ها
        RefreshButtonListeners();

        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    LastColorUnableObjects(obj, sprite.sprite, previousColor);
                    moveWhenPanelStartedManager.isStartMove = false;
                    moveWhenPanelStartedManager.isMoveToFirst = true;
                    playfunctionTransparenty = true;
                    FullTransparenty();
                }
            }
        }

        playfunction = false;
        OnableObjsTransparenty();
        scrollBarMenu.SetActive(false);
        isToggled = false;
    }
    public void OnclickObjectsMenu(Button sender) {
        clickedButton = sender;
        btnItems = sender;

        GameObject gameObject = btnItems.gameObject;
        Image imageGameObj = gameObject.GetComponent<Image>();
        currentColor = imageGameObj.color;
        currentSprite = imageGameObj.sprite;

        SaveCurrentState();

        // مقداردهی مجدد دکمه‌ها

        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    sprite.color = currentColor;
                    sprite.sprite = currentSprite;
                }
            }
        }
        RefreshButtonListeners();
    }


    private void FullTransparenty() {
        // حلقه‌ای برای عبور از تمام آبجکت‌ها در لیست
        foreach (GameObject obj in addedObjs) {
            if (obj != null) {
                SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
                if (sprite != null) {
                    Color color = sprite.color;
                    color.a += fadeSpeed * Time.deltaTime;
                    if (color.a >= 1f) { // مقدار 1f معادل شفافیت 255 است
                        color.a = 1f;
                    }
                    // اعمال مقدار جدید به رنگ
                    sprite.color = color;
                }
            }
        }
    }
    private void LastColorUnableObjects(GameObject objs, Sprite spriteMain, Color color) {
        SpriteRenderer sprite = objs.GetComponent<SpriteRenderer>();
        if (sprite != null) {
            sprite.color = color;
        }
        if (sprite.sprite != null) {
            sprite.sprite = spriteMain;
        }
    }

    private void SaveColorUnableObjects(GameObject objs,Sprite spriteMain,Color color) {
        SpriteRenderer sprite = objs.GetComponent<SpriteRenderer>();
        if (sprite != null) {
            sprite.color = color;
        }
        if(sprite.sprite != null) {
            sprite.sprite = spriteMain;
        }
    }
    private void SaveSprite(string key, Sprite sprite) {
        if (sprite != null) {
            PlayerPrefs.SetString(key, sprite.name);
            PlayerPrefs.Save();
        }
    }

    private Sprite LoadSprite(string key, Sprite defaultSprite) {
        if (PlayerPrefs.HasKey(key)) {
            string spriteName = PlayerPrefs.GetString(key);
            Sprite loadedSprite = Resources.Load<Sprite>(spriteName);

            if (loadedSprite != null) {
                return loadedSprite; // مقدار صحیح لود شد
            }
        }
        return defaultSprite; // مقدار قبلی حفظ شود
    }

    private void SaveColor(string key, Color color) {
        string colorString = $"{color.r},{color.g},{color.b},{color.a}"; // تبدیل به رشته
        PlayerPrefs.SetString(key, colorString);
        PlayerPrefs.Save();
    }

    private Color LoadColor(string key, Color defaultColor) {
        if (PlayerPrefs.HasKey(key)) {
            string[] rgba = PlayerPrefs.GetString(key).Split(',');
            return new Color(
                float.Parse(rgba[0]),
                float.Parse(rgba[1]),
                float.Parse(rgba[2]),
                float.Parse(rgba[3])
            );
        }
        return defaultColor;
    }

    private void SaveCurrentState() {
        SaveColor(currentColorKey, currentColor);
        SaveColor(previousColorKey, previousColor);

        SaveSprite(currentSpriteKey, currentSprite);
        SaveSprite(previousSpriteKey, previousSprite);
    }

    [System.Serializable]
    public class ObjectMenuItem {
        public Sprite sprite;
        public Color color;
    }
}
