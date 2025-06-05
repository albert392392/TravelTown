using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    public static PlayerCharacter Instance { get; private set; }

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
    //private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    //private float smoothSpeed = 10f;
    //private float scaleFactorY;
    public float newScaleX;
    public float newScaleY;
    public float inCreaseScaler;
    public float deCreaseScaler;
    [SerializeField] private string PositionAndScaleSaveKey;

    //private bool hasScaled = false;
    public void Awake() {
        mainCamera = Camera.main;
    }
    public void Start() {
        LoadPositionAndScale();
    }
    public void Update() {
        if(mainCamera == null)
            mainCamera = Camera.main;

        HandleTouch();
        if (isDragging) {
            UpdateScaleBasedOnY();
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
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
        if(IsTouchingCharacter(wordPos)) {
            isDragging = true;
            offset = (Vector2)transform.position - wordPos;
        }
    }
    private void OnTouchMove(Vector3 position) {
        if (isDragging) {
            Vector2 wordPos = Camera.main.ScreenToWorldPoint(position);
            Vector2 targetPos = wordPos + offset;
            targetPosition = new Vector3(
                targetPos.x,
                targetPos.y,
                0f
                );
        }
    }
    private void OnTouchEnd(Touch touch) {
        isDragging = false;
        SavePositionAndScale();
    }

    private bool IsTouchingCharacter(Vector3 position) {
        Collider2D[] hitColliders = Physics2D.OverlapPointAll(position);
        foreach (var hit in hitColliders) {
            if (hit.transform == transform)
                return true;
        }
        return false;
    }
    private void UpdateScaleBasedOnY() {
        float normalizedY = Mathf.InverseLerp(minY, maxY, transform.position.y);
        float scale = Mathf.Lerp(maxScale, minScale, normalizedY);

        Vector3 targetScale = new Vector3(scale,scale, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale,targetScale ,Time.deltaTime * 10f);
    }

    public void SnapBack(Vector2 position, Vector2 scale) {
        transform.DOMove(position, 0.3f).SetEase(Ease.OutQuad);
        transform.DOScale(scale, 0.3f).SetEase(Ease.OutQuad);
    }
    public void SavePositionAndScale() {
        if (!GetComponentInChildren<Foot>().isOverThing && GetComponentInChildren<Foot>().isOnGround) {
            string data = string.Join(",",
                targetPosition.x.ToString(),
                targetPosition.y.ToString(),
                transform.localScale.x.ToString(),
                transform.localScale.y.ToString()
                );

            PlayerPrefs.SetString(PositionAndScaleSaveKey, data);
            PlayerPrefs.Save();

        }
        Debug.Log("SavePositionAndScale");
    }
    public void LoadPositionAndScale() {
        if (PlayerPrefs.HasKey(PositionAndScaleSaveKey)) {
            string[] data = PlayerPrefs.GetString(PositionAndScaleSaveKey).Split(',');
            if (data.Length == 4) {
                targetPosition.x = float.Parse(data[0]);
                targetPosition.y = float.Parse(data[1]);
                float scaleX = float.Parse(data[2]);
                float scaleY = float.Parse(data[3]);

                transform.position = new Vector2(targetPosition.x, targetPosition.y);
                transform.localScale = new Vector2(scaleX, scaleY);
            }
        }
        Debug.Log("LoadPositionAndScale");
    }
    private void OnApplicationQuit() {
        SavePositionAndScale();
    }
    private void OnApplicationPause(bool pause) {
       if(pause)SavePositionAndScale();
    }
}
