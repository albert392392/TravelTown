using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class ObjectDataFingerHelp {
    public string objectName;
    public Vector3 position;
}
[System.Serializable]
public class ObjectDataListFingerHelp {
    public List<ObjectDataFingerHelp> objects;    
}
public class ObjectRotationHint : MonoBehaviour {
    public float moveDistance = 1f;
    public float offsetX = 0.5f;
    public float offsetY = 0.5f;
    public float offsetZ = 0.5f;
    public float duration = 2f;
    public float waitTime = 3f;

    public GameObject HelpFinger;
    public SpriteMask spriteMask1;
    public SpriteMask spriteMask2;
    public GameObject squareSprite;
    public InventoryItemDragHandler inventoryItem;
    [SerializeField] private List<ObjectInformations> objectInformations;
    [SerializeField] private List<GameObject> allObjectMerge = new List<GameObject>();
    private GameObject startObj;
    private GameObject endObj;
    private GameObject singleObj;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private List<Transform> _targetPositions = new List<Transform>();
    [SerializeField] private List<GameObject> _spriders_targetPositions = new List<GameObject>();
    [SerializeField] private List<GameObject> _gameObjects_targetPositions = new List<GameObject>();
    public int _MergeSpawnCount;
    public string save_MergeSpawnCount = "save_MergeSpawnCount";
    public bool startTargetPosition;
    const string endTutorial = "endTutorailfinger";
    // Dictionary to track animating objects
    private Dictionary<GameObject, bool> animatingObjects = new Dictionary<GameObject, bool>();

    [SerializeField] private HelpMergeManager helpMergeManager;

    private void Start() {
        helpMergeManager.enabled = false;

        if (PlayerPrefs.GetInt(endTutorial, 0) == 0) {
            if (inventoryItem != null) {
                inventoryItem.GetComponent<Button>().enabled = false;
                inventoryItem.GetComponent<Collider>().enabled = false;
            }
            else {
                Debug.LogError("InventoryItemDragHandler is empty ! please add to ");
            }
            StartCoroutine(HelpFingerMovement());
            helpMergeManager.enabled = false;
        }
        else if(PlayerPrefs.GetInt(endTutorial , 1) == 1) {
            DestroyWhenEnd();
        }
        _MergeSpawnCount = PlayerPrefs.GetInt(save_MergeSpawnCount,0);
        print(_MergeSpawnCount);
      
    }
    private void updateTargetPositions() {
        if (startTargetPosition) {
            // پاک کردن مقادیر قبلی برای جلوگیری از داده‌های تکراری
            _targetPositions.Clear();
            _spriders_targetPositions.Clear();
            _gameObjects_targetPositions.Clear();

            foreach (var target in gridManager.targetPositions) {
                if (!_targetPositions.Contains(target)) {
                    _targetPositions.Add(target);
                }

                var spider = target.GetComponentInChildren<SpiderScript>();
                if (spider != null && !_spriders_targetPositions.Contains(spider.gameObject)) {
                    _spriders_targetPositions.Add(spider.gameObject);
                }

                var scatter = target.GetComponentInChildren<ScatterObjectsWithInterval>();
                var merge = target.GetComponentInChildren<ObjectMerge>();

                if ((scatter != null) && !_gameObjects_targetPositions.Contains(scatter.gameObject)) {
                    _gameObjects_targetPositions.Add(scatter.gameObject);
                }
                else if ((merge != null) && !_gameObjects_targetPositions.Contains(merge.gameObject)) {
                    _gameObjects_targetPositions.Add(merge.gameObject);
                }
            }
        }

        startTargetPosition = false; // یک‌بار مقداردهی انجام شود
    }
    private void Update() {
        CleanupAnimatedObjects(); // Remove references to destroyed objects
        UpdateObjectList();

        updateTargetPositions();
        // Process ObjectInformations without removing at the start

        for (int i = 0; i < objectInformations.Count; i++) {
            ProcessObjectInformation(objectInformations[i]);
        }

        for (int i = objectInformations.Count - 1; i >= 0; i--) {
            if (objectInformations[i].isFinish == true) {
                objectInformations.RemoveAt(i);
            }
        }

        CheckCurrentTaskCompletion();
    }
    private bool IsTaskComplete(ObjectInformations info) {
        // Check if the current ObjectInformations entry is completed
        return info.foundObjects.Count == 0;
    }

    private void CleanupAnimatedObjects() {
        var keysToRemove = animatingObjects.Keys.Where(obj => obj == null).ToList();
        foreach (var key in keysToRemove) {
            animatingObjects.Remove(key);
        }
    }

    private void UpdateObjectList() {
        var objectsWithMerge = FindObjectsOfType<ObjectMerge>();
        var objectsWithScatter = FindObjectsOfType<ScatterObjectsWithInterval>();
        allObjectMerge.Clear();

        allObjectMerge.AddRange(objectsWithMerge.Select(o => o.gameObject));
        foreach (var scatterObj in objectsWithScatter) {
            if (!allObjectMerge.Contains(scatterObj.gameObject)) {
                allObjectMerge.Add(scatterObj.gameObject);
            }
        }

        var objectsWithNoSpiderWeb = allObjectMerge.Where(obj => {
            var objectMerge = obj.GetComponent<ObjectMerge>();
            return objectMerge != null && objectMerge.SpiderWeb == null;
        }).ToList();

        objectsWithNoSpiderWeb = objectsWithNoSpiderWeb.OrderBy(obj => allObjectMerge.IndexOf(obj)).ToList();


        foreach (var obj in objectsWithNoSpiderWeb) {
            var textMeshPro = obj.transform.GetChild(0).GetComponent<TextMeshPro>();
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null) {
                int index = objectInformations.FindIndex(info =>
                    info.color == spriteRenderer.color &&
                    info.number == textMeshPro.text &&
                    info.sprite == spriteRenderer.sprite);

                if (index > -1) {
                    if (!objectInformations[index].foundObjects.Contains(obj)) {
                        objectInformations[index].foundObjects.Add(obj);
                       // Debug.Log($"GameObject: {obj.name} matches criteria.");
                    }
                }
            }
        }
        foreach (var obj in allObjectMerge) {
            var textMeshPro = obj.transform.GetChild(0).GetComponent<TextMeshPro>();
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null) {
                int index = objectInformations.FindIndex(info =>
                    info.color == spriteRenderer.color &&
                    info.number == textMeshPro.text &&
                    info.sprite == spriteRenderer.sprite);

                if (index > -1) {
                    if (!objectInformations[index].foundObjects.Contains(obj)) {
                        objectInformations[index].foundObjects.Add(obj);
                        // Debug.Log($"GameObject: {obj.name} matches criteria.");
                    }
                }
            }
        }
        foreach (var info in objectInformations) {
            info.foundObjects.RemoveAll(obj =>
                obj == null ||
                (obj.GetComponent<ObjectMerge>() == null && obj.GetComponent<ScatterObjectsWithInterval>() == null));
        }
    }

    private void ProcessObjectInformation(ObjectInformations info) {

        if (info == null || info.foundObjects == null || info.foundObjects.Count == 0)
            return;

        if (info.foundObjects.Count >= 2) {

            var objectsWithNoSpiderWeb = allObjectMerge.Where(obj => {
                var objectMerge = obj.GetComponent<ObjectMerge>();
                return objectMerge != null && objectMerge.SpiderWeb == null;
            }).ToList();

            objectsWithNoSpiderWeb = objectsWithNoSpiderWeb.OrderBy(obj => allObjectMerge.IndexOf(obj)).ToList();
           
            if (objectsWithNoSpiderWeb != null) {
                startObj = info.foundObjects
                    .OrderBy(obj => Vector3.Distance(objectsWithNoSpiderWeb[0].transform.position, obj.transform.position))
                    .FirstOrDefault();
                var col1 = startObj.GetComponent<Collider>();
                if (col1 != null) col1.enabled = true;
            }

            // دورترین آبجکت از startObj به عنوان endObj
            if (startObj != null) {
                endObj = info.foundObjects
                    .Where(obj => obj != startObj) // حذف startObj
                    .OrderByDescending(obj => Vector3.Distance(startObj.transform.position, obj.transform.position))
                    .FirstOrDefault();
                var col2 = endObj.GetComponent<Collider>();
                if (col2 != null) col2.enabled = true;
            }

            if (startObj == null || endObj == null) {
                info.foundObjects.Remove(startObj);
                info.foundObjects.Remove(endObj);
                return;
            }
            if (startObj != null && startObj.transform.parent != null && startObj.transform.parent.parent != null) {
                SetSpriteMaskPosition(startObj.transform.parent.parent.gameObject, spriteMask1);
            }
            if (endObj != null && endObj.transform.parent != null && endObj.transform.parent.parent != null) {
                SetSpriteMaskPosition(endObj.transform.parent.parent.gameObject, spriteMask2);
            }
            spriteMask1.enabled = true;
            spriteMask2.enabled = true;

            CheckMerge(info, startObj, endObj);
        }
        else if (info.foundObjects.Count == 1) {
            singleObj = info.foundObjects[0];
            var col = singleObj?.GetComponent<Collider>();
            if (col != null) col.enabled = true;
            var scatterScript = singleObj?.GetComponent<ScatterObjectsWithInterval>();
            if (scatterScript != null) {
                CheckClick(info, singleObj);
                AnimateSingleObject(singleObj);
            }
        }
    }
    private IEnumerator HelpFingerMovement() {
        while (true) {
            if (startObj != null && endObj != null) {
                MoveHelpFinger(startObj, endObj);
                yield return new WaitForSeconds(duration);
            }
            yield return null;
        }
    }

    public void CheckMerge(ObjectInformations info, GameObject startObj, GameObject endObj) {
        startTargetPosition = true;
        if (startObj == null || endObj == null) {
            info.foundObjects.Remove(startObj);
            info.foundObjects.Remove(endObj);
            return;
        }
        var startMergeScript = startObj.GetComponent<MergeableBase>();
        var endMergeScript = endObj.GetComponent<MergeableBase>();

        if (endMergeScript.CurrentState == MergeableState.Merge || startMergeScript.CurrentState == MergeableState.Merge) {
            print("MERRRRRRGGGG");
            Debug.Log($"Merging {startObj.name} and {endObj.name}");
            print(info.isFinish);
            info.isFinish = true;
            print(info.isFinish);
            info.foundObjects.Remove(startObj);
            info.foundObjects.Remove(endObj);
            Destroy(startObj);
            Destroy(endObj);
        }
    }
    public void CheckClick(ObjectInformations info, GameObject singleObj) {
        var scatterScript = singleObj.GetComponent<ScatterObjectsWithInterval>();
        startTargetPosition = true;
        //Debug.Log($"Scatter Spawn: {scatterScript.Spawn}");

        if (scatterScript.CurrentState == MergeableState.Spawning) {
            print("CLICKKKKKKKK");
            PlayerPrefs.SetInt(save_MergeSpawnCount, _MergeSpawnCount);
            PlayerPrefs.Save();
            info.isFinish = true;
            // Destroy(singleObj);
            info.foundObjects.Remove(singleObj);
        }
    }
    private void MoveHelpFinger(GameObject startObj, GameObject endObj) {
        if (startObj == null || endObj == null) return;
        HelpFinger.transform.DOKill();
        if (startObj.transform.parent != null) {
            HelpFinger.transform.position = startObj.transform.parent.position;
        }
        HelpFinger.transform.DOMove(endObj.transform.parent.position, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    private void SetSpriteMaskPosition(GameObject obj, SpriteMask spriteMask) {
        if (spriteMask != null && obj != null) {
            spriteMask.transform.position = obj.transform.position;
        }
    }
    private void CheckCurrentTaskCompletion() {
        if (_MergeSpawnCount == 5) {
            EnergyManager.Instance.totalEnergys += 100;
            inventoryItem.GetComponent<Button>().enabled = true;
            inventoryItem.GetComponent<Collider>().enabled = true;
            Destroy(HelpFinger);
            Destroy(squareSprite, 0.2f);
            Destroy(gameObject);
            DestroyWhenEnd();
        }
    }
    private void DestroyWhenEnd() {
        helpMergeManager.enabled = true;
        HelpFinger.SetActive(false);
        squareSprite.SetActive(false);
        this.gameObject.SetActive(false);
        startTargetPosition = true;
        PlayerPrefs.SetInt(endTutorial, 1);
        PlayerPrefs.Save();
    }
    private void OnApplicationQuit() {
        SaveMergeSpawnCount();
    }
    private void SaveMergeSpawnCount() {
        PlayerPrefs.SetInt(save_MergeSpawnCount, _MergeSpawnCount);
        PlayerPrefs.Save();
        print(_MergeSpawnCount);
    }
    private void AnimateSingleObject(GameObject obj) {
        if (obj == null) return;

        if (animatingObjects.ContainsKey(obj) && animatingObjects[obj]) return;

        animatingObjects[obj] = true;

        HelpFinger.transform.DOKill();
        HelpFinger.transform.position = obj.transform.parent.position;

        HelpFinger.transform.DOPunchScale(Vector3.one * 0.2f, duration, 1, 1)
            .OnComplete(() => {
                HelpFinger.transform.position = obj.transform.position;
                animatingObjects[obj] = false;
            });

        spriteMask1.enabled = true;
        spriteMask1.transform.position = obj.transform.parent.parent.position;
        spriteMask2.enabled = false;
    }

    [System.Serializable]
    public class ObjectInformations {
        public List<GameObject> foundObjects = new List<GameObject>();
        public Sprite sprite;
        public Color color;
        public string number;
        public bool isFinish;
    }
}
