using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class ScatterObjectsWithInterval : MergeableBase {
    public List<GameObject> objectsPrefabs;
    public List<Transform> targetPositions;
    private GridManager gridManager1;
    private UIManager uiManager;
    public bool isOutOfBounds = false;
    [SerializeField] private float scatterSpeed;
    [SerializeField] private float spawnInterval;
    private Vector2 touchStartPosition;
    private float dragThreshold = 20f;

    private float updateInterval = 1f;
    private float nextUpdateTime = 0f;

    public int DiamondNeedForClock;
    public float TimerStopClock;

    [SerializeField] private TextMeshProUGUI _timerText;
    private float timeRemaining = 120f;

    public GameObject scatterObjectTimerClock;

    public int spawnedObjectCount = 0;
    public float timerRemaining;

    private Vector3 originalScale;
    private float touchStartTime;
    private HelpMergeManager helpMergeManager;
    private bool hasTouchedObject = false;

    private void Start() {
        helpMergeManager = FindAnyObjectByType<HelpMergeManager>();
        uiManager = UIManager.Instance;
        if (uiManager == null) {
            Debug.LogError("UIManager.Instance is null!");
            return;
        }
        originalPosition = transform.position;
        originalScale = transform.localScale;

        _timerText = uiManager.TimerText;

        gridManager1 = FindObjectOfType<GridManager>();
        if (gridManager1 == null) {
            Debug.LogError("GridManager not found in scene!");
            return;
        }

        gridManager1.OnEmptyTargetPotationsChanged += SetTargetPositions;

        scatterObjectTimerClock = Instantiate(uiManager.ScatterObjectTimerClock);
        scatterObjectTimerClock.transform.SetParent(this.transform, false);
        scatterObjectTimerClock.SetActive(false);
    }

    private void Update() {
        HandleTouch();
        UpdateTimer();

        if (Time.time >= nextUpdateTime) {
            if (gridManager1 != null && gridManager1.emptytargetPotations.Count > 0) {
                SetTargetPositions(gridManager1.emptytargetPotations);
            }
            nextUpdateTime = Time.time + updateInterval;
        }
        if (mergedObject != null) {
            CheckForTargetsAndDestroy(mergedObject.transform);
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
                OnTouchEnd(touch);
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

            ObjectDragManager.Instance.SetDragging(gameObject);

            if (touchedObject != null) {
                UIManager.Instance.chooseOver.SetActive(false);
                UIManager.Instance.chooseOver.transform.position = touchedObject.transform.parent.position;
                UIManager.Instance.chooseOver.SetActive(true);
            }
            SetState(MergeableState.Spawning);


            SetState(MergeableState.Dragging);


            SetState(MergeableState.Spawning);

            hasTouchedObject = true;
            originalPosition = transform.parent.position;
            ButtonDeleteObject.Instance.objectDelete = null;
            ButtonDeleteObject.Instance.objectDelete = GetComponent<MergeableBase>();
            touchStartTime = Time.time; // ذخیره زمان شروع لمس
            originalScale = transform.localScale;
            //lastTargetPosition = transform.parent;
            float size = originalScale.magnitude * 1.1f;
            float mainSize = originalScale.magnitude;
            isTouchUp = false;
            isTouchDown = true;
            if (mainSize > size) {
                transform.DOScale(originalScale * 1.1f, 0.2f);
            }
        }
    }

    private void OnTouchMove(Vector3 position) {
        if (SpiderWeb != null || box != null || !hasTouchedObject) return;

        float distance = Vector3.Distance(position, touchStartPosition);
        if (distance > dragThreshold) {
            if (UIManager.Instance.chooseOver != null) {
                UIManager.Instance.chooseOver.SetActive(false);
            }
            SetState(MergeableState.Dragging);

            float zDist = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, zDist));
            touchWorldPos.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, touchWorldPos, Time.deltaTime * 15f);
        }

    }

    private void OnTouchEnd(Touch touch) {
        if (!hasTouchedObject) return;
        ObjectDragManager.Instance.ClearDragging();
        //SetState(MergeableState.Idle);
        if (CurrentState != MergeableState.Dragging && CurrentState != MergeableState.Swap && CurrentState != MergeableState.MovingToTarget && CurrentState != MergeableState.ReturnBackPos) 
        {
            SetState(MergeableState.Spawning);
            CheckClickOrTouch();
            isTouchUp = true;
            hasTouchedObject = false;
            isTouchDown = false;
            return;
        }
        float distance = Vector3.Distance(touch.position, touchStartPosition);
        ObjectDragManager.Instance.ClearDragging();
        transform.DOScale(originalScale, 0.25f).SetEase(Ease.InBack);
        hasTouchedObject = false;
        isTouchDown = false;
        isTouchUp = true;
        touchedObject = null;
        SetState(MergeableState.Idle);
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
    private void CheckClickOrTouch() {
        if (box != null && SpiderWeb != null) return;
        if (EnergyManager.Instance != null && EnergyManager.Instance.GetEnergyCount() > 0) {
            StartCoroutine(ScatterObjects());
        }
    }
    IEnumerator ScatterObjects() {
        if (EnergyManager.Instance == null ||
            EnergyManager.Instance.GetEnergyCount() <= 0 ||
            gridManager1 == null ||
            gridManager1.emptytargetPotations.Count == 0) {
            Debug.LogWarning("Cannot spawn objects: Check energy and available positions.");
            yield break;
        }

        ObjectDragManager.Instance.ClearDragging();

        SetState(MergeableState.Spawning);
        HelpMergeManager helpMergeManager = FindObjectOfType<HelpMergeManager>();
        helpMergeManager.idleTimer = 0f;
        spawnedObjectCount++;
        ObjectRotationHint objectRotationHint = FindObjectOfType<ObjectRotationHint>();
        if (objectRotationHint != null) {
            if (objectRotationHint._MergeSpawnCount == 3) {
                objectRotationHint._MergeSpawnCount++;
            }
        }
        Transform targetParent = GetAnotherFreeTarget();
        if (targetParent == null) {
            Debug.LogWarning("No free target found!");
            SetState(MergeableState.Idle);
            yield break;
        }
      
        StartCoroutine(ParticleSys(targetParent));

        // رزرو فوری (کلیدی)
        GridManager.Instance.ReservePosition(targetParent.position);
        GameObject selectedPrefab = objectsPrefabs[Random.Range(0, objectsPrefabs.Count)];
        GameObject spawnedObject = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        // اسپاون و انتقال
        spawnedObject.transform.DOJump(targetParent.position, 1.5f, 1, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnStart(() => {
                var col = spawnedObject.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            })
            .OnComplete(() => {
                if (targetParent.childCount == 0) {
                    spawnedObject.transform.SetParent(targetParent, false);
                    spawnedObject.transform.position = targetParent.position;
                    GridManager.Instance.MarkPositionAsOccupied(targetParent.position);
                }

                var col = spawnedObject.GetComponent<Collider>();
                if (col != null) {
                    col.enabled = true;
                }
                // پاک‌سازی رزرو (اختیاری)
                GridManager.Instance.UnreservePosition(targetParent.position);
            });
        EnergyManager.Instance.SpendEnergy(1);

        SetState(MergeableState.Idle);
    }
    IEnumerator ParticleSys(Transform targetParent) {

        GameObject particleSystem = Instantiate(UIManager.Instance.merge_particleSystem.gameObject, targetParent.position, Quaternion.identity);
        ParticleSystem ps = particleSystem.GetComponent<ParticleSystem>();
        ps.Play();

        yield return new WaitForSeconds(1f);


        ps.Stop();
        Destroy(particleSystem);
    }
    private Transform GetAnotherFreeTarget() {
        foreach (Transform t in GridManager.Instance.emptytargetPotations) {
            if (t.childCount == 0 &&
                !GridManager.Instance.GetEmptyTargetPosition(t.position) &&
                !GridManager.Instance.IsReserved(t.position)) {
                return t;
            }
        }
        return null;
    }

    private void SetTargetPositions(List<Transform> targets) {
        targetPositions.Clear();
        targetPositions.AddRange(targets);
    }
    void StartCooldownTimer() {
        timerRemaining = TimerStopClock;
        scatterObjectTimerClock.SetActive(true);
        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer() {
        while (timerRemaining > 0) {
            timerRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timerRemaining / 60);
            int seconds = Mathf.FloorToInt(timerRemaining % 60);
            uiManager.TextPin.text = $"Spend {DiamondNeedForClock} diamonds to proceed. {minutes:00}:{seconds:00}";
            yield return null;
        }

        scatterObjectTimerClock.SetActive(false);
        spawnedObjectCount = 0;
        StartCoroutine(ScatterObjects());
    }

    void UpdateTimer() {
        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            _timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else {
            timeRemaining = 120f;
            EnergyManager.Instance.AddEnergy(1);
        }
    }
}