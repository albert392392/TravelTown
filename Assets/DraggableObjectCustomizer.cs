using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DraggableObjectCustomizer : MonoBehaviour {
    public static DraggableObjectCustomizer instance { get; private set; }

    [Header("Clamp Settings")]
    [SerializeField] private float minY = -65f;
    [SerializeField] private float maxY = 16f;
    [SerializeField] private float minX = -380f;
    [SerializeField] private float maxX = 360f;

    [Header("Scale Control")]
    [SerializeField] private float minScale = 9f;
    [SerializeField] private float maxScale = 21f;

    private Vector2 offset;
    public bool isDragging = false;
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    

    private GameObject choosePanel4;
    private float scaleFactorY;
    public float newScaleX;
    public float newScaleY;
    public float inCreaseScaler;
    public float deCreaseScaler;
    public bool isObjectActive;
    public string activeObjectKey;
    [SerializeField] private string PositionAndScaleSaveKey;
    private Vector3 targetPosition;
    private bool hasScaled = false;


    public void Awake() {
        mainCamera = Camera.main;
    }

    public void Start() {
        if (PlayerPrefs.HasKey(activeObjectKey)) { // بررسی وجود مقدار
            isObjectActive = PlayerPrefs.GetInt(activeObjectKey) == 1;
        }
        LoadPositionAndScale();
    }
    public void Update() {
        /*
        if (isObjectActive && !hasScaled) {
            transform.localScale = new Vector2(15, 15);
            hasScaled = true;
        }*/
        HandleTouch();
        if (isDragging) {
            UpdateScaleBasedOnY();
            transform.position = Vector3.Lerp(transform.position,targetPosition, Time.deltaTime * 10f);
        }
    }
    private void HandleTouch() {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        switch (touch.phase) {
            case TouchPhase.Began:
                OnTouchBegin(touch.position);
                break;
            case TouchPhase.Moved:
                OnTouchMove(touch.position);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                OnTouchEnd(touch);
                break;
        }
    }
    private void OnTouchBegin(Vector2 position) {
        Vector2 wordPos = Camera.main.ScreenToWorldPoint(position);
        if (IsTouchingCharacter(wordPos)) {
            isDragging = true;
            offset = (Vector2)transform.position - wordPos;

            choosePanel4 = GameObject.FindGameObjectWithTag("choosePanel4");
            if (choosePanel4 != null) {
                choosePanel4.SetActive(true);
            }
            GetComponent<AddedAndChangeScriptMenu>().isDraggingFromCustomizer = false;
        }
    }
    private void OnTouchMove(Vector3 position) {
        choosePanel4 = GameObject.FindGameObjectWithTag("choosePanel4");
        if (isDragging) {
            GetComponent<AddedAndChangeScriptMenu>().isDraggingFromCustomizer = true;
            Vector2 wordPos = Camera.main.ScreenToWorldPoint(position);
            Vector2 targetPos = wordPos + offset;
            targetPosition = new Vector3(
                targetPos.x,
                targetPos.y,
                0f
                );
            if (choosePanel4 != null) {
                Destroy(choosePanel4);
            }
        }
    }
    private void OnTouchEnd(Touch touch) {
        isDragging = false;
        GetComponent<AddedAndChangeScriptMenu>().isDraggingFromCustomizer = false;
        SavePositionAndScale();
    }
    private bool IsTouchingCharacter(Vector3 position) {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(position);
        foreach (var hit in hitColliders) {
            if (hit.transform == transform) {
                GetComponent<AddedAndChangeScriptMenu>().isDraggingFromCustomizer = true;
                return true;
            }
        }
        return false;
    }
    private void UpdateScaleBasedOnY() {
        float normalizedY = Mathf.InverseLerp(minY, maxY, transform.position.y);
        float scale = Mathf.Lerp(maxScale, minScale, normalizedY);

        Vector3 targetScale = new Vector3(scale, scale, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
    }

    public void SnapBack(Vector2 position, Vector2 scale) {
        transform.DOMove(position, 0.3f).SetEase(Ease.OutQuad);
        transform.DOScale(scale, 0.3f).SetEase(Ease.OutQuad);
    }
    public void SavePositionAndScale() {
        if (!GetComponentInChildren<FootDownDraggable>().isOverThing && GetComponentInChildren<FootDownDraggable>().isOnGround) {
            string data = string.Join(",",
                transform.position.x.ToString(),
                transform.position.y.ToString(),
                transform.localScale.x.ToString(),
                transform.localScale.y.ToString()
                );

            PlayerPrefs.SetString(PositionAndScaleSaveKey, data);
            PlayerPrefs.Save();
        }
    }
    public void LoadPositionAndScale() {
        if (PlayerPrefs.HasKey(PositionAndScaleSaveKey)) {
            string[] data = PlayerPrefs.GetString(PositionAndScaleSaveKey).Split(',');
            if (data.Length == 4) {
                targetPosition.x = float.Parse(data[0]);
                targetPosition.y = float.Parse(data[1]);
                newScaleX = float.Parse(data[2]);
                newScaleY = float.Parse(data[3]);

                transform.position = new Vector2(targetPosition.x, targetPosition.y);
                transform.localScale = new Vector2(newScaleX, newScaleY);
            }
        }
    }
    public void IncreaseScale(float increment)
    {
        if (transform.localScale.x + increment <= maxScale && transform.localScale.y + increment <= maxScale)
        {
            newScaleX = transform.localScale.x;
            newScaleY = transform.localScale.y;
            newScaleX += increment;
            newScaleY += increment;
            transform.DOScale(new Vector2(newScaleX, newScaleY), 0.3f).SetEase(Ease.OutQuad);
        }
    }

    public void DecreaseScale(float decrement)
    {
        if (newScaleX - decrement >= minScale && newScaleY - decrement >= minScale)
        {
            newScaleX -= decrement;
            newScaleY -= decrement;
            transform.DOScale(new Vector2(newScaleX, newScaleY), 0.3f).SetEase(Ease.OutQuad);
        }
    }
    private void OnApplicationQuit() {
        SavePositionAndScale();
    }
    private void OnApplicationPause(bool pause) {
        SavePositionAndScale();
    }
}
