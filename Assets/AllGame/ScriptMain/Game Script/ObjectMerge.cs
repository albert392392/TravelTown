using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.Collections.LowLevel.Unsafe;

public class ObjectMerge : MergeableBase {

    public GameObject LastObject;
    private Vector3 originalScale;
    private Vector2 touchStartPosition;
    private float dragThreshold = 13f;
    public bool isOutOfBounds = false;
   
    public int AddCountCoinPanel;
    public string AddTextLevel;
    public Sprite PlaceSpritePanel;
    public string AddPanelName;
    public int DiamondCountSpider = 14;
    public float distanceToMerge = 2f;
    private Collider collider;
    private void Start() {
        collider = GetComponent<Collider>();
        CustomerScript = UIManager.Instance.GetCustomer(customerScript);
        originalPosition = transform.position;
        originalScale = transform.localScale;

        if (!objectsList.Contains(gameObject)) {
            objectsList.Add(gameObject);
        }
    }

    private void Update() {

        if (Input.touchCount > 0) HandleTouch();

        // UIManager.Instance.chooseOver.gameObject.SetActive(!isDragging);

        if (box != null) collider.enabled = false;
        else if (SpiderWeb != null && box == null) collider.enabled = true;

        if (Merged) {
            CheckForTargetsAndDestroy(transform);
        }
    }

    private void HandleTouch() {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        switch (touch.phase) {
            case TouchPhase.Began:
                if (CurrentState != MergeableState.Dragging && CanInteract()) OnTouchBegin(touch.position);
                break;
            case TouchPhase.Moved:
                if (CurrentState == MergeableState.Dragging && CanInteract()) OnTouchMove(touch.position);
                break;
            case TouchPhase.Ended:
                if (CurrentState == MergeableState.Dragging) OnTouchEnd(touch);
                break;
        }
    }

    private void OnTouchBegin(Vector3 position) {
        if (ObjectDragManager.Instance.IsDraggingSomething())
            return;

        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject) {
            touchedObject = hit.collider.gameObject;
            touchStartPosition = position;
            /* ObjectDragManager.Instance.SetDragging(gameObject);
             ObjectManager.Instance.isDragging = true;*/
            ObjectDragManager.Instance.SetDragging(gameObject);
            if (touchedObject != null) {
                if (UIManager.Instance.chooseOver != null) {
                    UIManager.Instance.chooseOver.SetActive(false);
                    UIManager.Instance.chooseOver.transform.position = touchedObject.transform.parent.position;
                    UIManager.Instance.chooseOver.SetActive(true);
                }
            }
            ButtonDeleteObject.Instance.objectDelete = null;
            ButtonDeleteObject.Instance.objectDelete = GetComponent<MergeableBase>();

            originalPosition = transform.parent.position;
            //lastTargetPosition = transform.parent;
            DOTween.Kill(transform);
            transform.DOScale(originalScale * 1.1f, 0.2f);
            SetState(MergeableState.Dragging);
            isTouchUp = false;
            isTouchDown = true;
        }
    }

    private void OnTouchMove(Vector3 position) {
        if (CurrentState != MergeableState.Dragging || SpiderWeb != null || box != null) return;

        float distance = Vector3.Distance(position, touchStartPosition);
        if (distance > dragThreshold) {
            if (UIManager.Instance.chooseOver != null) {
                UIManager.Instance.chooseOver.SetActive(false);
            }


            float zDist = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, zDist));
            touchWorldPos.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, touchWorldPos, Time.deltaTime * 20f);
        }
    }

    private void OnTouchEnd(Touch touch) {
        if (CurrentState != MergeableState.Dragging) return;

        SetState(MergeableState.Idle);
        isTouchUp = true;
        isTouchDown = false;
        ObjectDragManager.Instance.ClearDragging();
        transform.DOScale(originalScale, 0.2f);

        float distance = Vector3.Distance(touch.position, touchStartPosition);
   
        // لمس بدون کشیدن
        if (distance < dragThreshold) {
            ReturnToOriginalPosition();
            return;
        }

        if (TryMergeOrSwap())
            return;

        // برگشت به جای خالی قبلی اگر هنوز در Grid هست
        if (lastTargetPosition != null &&
            lastTargetPosition.childCount == 0 &&
            !isNearBox &&
            GridManager.Instance.IsPositionInGrid(lastTargetPosition.position)) {
            MoveToTarget(lastTargetPosition);
            return;
        }

        // اگر نزدیک باکس بود ولی جای مشخصی نداشت
        if (isNearBox && CurrentState != MergeableState.MovingToTarget) {
            ReturnToOriginalPosition();
            return;
        }

        // اگر موقعیت داخل گرید هست، به نزدیک‌ترین خانه خالی برو
        if (GridManager.Instance.IsPositionInGrid(transform.position)) {
            Transform nearest = GridManager.Instance.FindBestEmptyTarget(transform.position);
            if (nearest != null) {
                MoveToTarget(nearest);
                UpdateChooseOver(transform.parent);
            }
            else {
                ReturnToOriginalPosition();
            }
            return;
        }

        if (inventoryItemDragHandler != null) {
            In_inventory();
        }
        // در نهایت بازگشت
        ReturnToOriginalPosition();
    }
    private bool TryMergeOrSwap() {
        Collider[] nearby = Physics.OverlapSphere(transform.position, 1.2f);

        foreach (var col in nearby) {
            if (col == GetComponent<Collider>()) continue;

            GameObject other = col.gameObject;
            if (!other) continue;

            var otherMerge = other.GetComponent<MergeableBase>();
            if (otherMerge == null || otherMerge == this) continue;

            if (otherMerge.box != null) continue;

            // 1️⃣ ادغام اگر مشابه بودن  
            if (IsSimilarTo(other)) {
                MergeWith(other);
                UpdateChooseOver(other.transform.parent);
                return true;
            }


            // ✅ رد کردن آبجکت‌هایی که box یا SpiderWeb دارند
            if (otherMerge.box != null || otherMerge.SpiderWeb != null) continue;

            Transform otherParent = other.transform.parent;
            Transform myParent = transform.parent;


            if (GridManager.Instance.emptytargetPotations.Count == 0) {
                otherGameObjectToswap = null;
            }
            if (otherGameObjectToswap == null) {
                // 3️⃣ Swap اگر غیرمشابه بودن و هر دو موقعیت پر بودن
                if (!IsSimilarTo(other) &&
                    myParent != null && otherParent != null &&
                    myParent.childCount > 0 && otherParent.childCount > 0) {
                    SwapWith(other);
                    UpdateChooseOver(other.transform.parent);
                    return true;
                }
            }
            if (otherGameObjectToswap != null) {
                if (otherParent != null) {
                    Transform pushTarget = GridManager.Instance.FindBestEmptyTarget(otherParent.position);
                    if (pushTarget != null && pushTarget.childCount == 0) {
                        Vector3 originalOtherPos = other.transform.position;

                        other.transform.SetParent(pushTarget);
                        other.transform.DOMove(pushTarget.position, 0.25f).SetEase(Ease.OutQuad);

                        transform.SetParent(otherParent);
                        transform.DOMove(originalOtherPos, 0.25f).SetEase(Ease.OutQuad).OnComplete(() => {
                            UpdateChooseOver(transform.parent);
                        });

                        return true;
                    }
                }
            }
        }

        return false;
    }
    private void MainDeletePanelSell() {
        UIManager.Instance.PlaceSpritePanel.sprite = PlaceSpritePanel;
        UIManager.Instance.placeTextLevel.text = AddTextLevel;
        UIManager.Instance.placePanelText.text = AddPanelName;
        UIManager.Instance.placeCountCoin.text = AddCountCoinPanel.ToString();
        UIManager.Instance.placeButtonPanel.enabled = true;
        UIManager.Instance.LastMargeObject = mergedPrefab;
        UIManager.Instance.AddCountCoinPanel = mergedPrefab.GetComponent<ObjectMerge>().AddCountCoinPanel;
    }
}
