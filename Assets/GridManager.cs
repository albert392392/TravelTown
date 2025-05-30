using System.Collections.Generic;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System;
using System.IO;
using UnityEngine.SceneManagement;
[System.Serializable]
public class TargetPositionData {
    public Vector3 position;
    public List<ChildData> children;

    public TargetPositionData(Vector3 position) {
        this.position = position;
        this.children = new List<ChildData>();
    }
}
[System.Serializable]
public class ChildData {
    public Vector2 position;
    public string objName;
    public bool isActive;

    public List<ChildData> children = new List<ChildData>();
}
[System.Serializable]
public class SavedDataWrapper {
    public List<TargetPositionData> allTargets = new List<TargetPositionData>();
}
public class GridManager : MonoBehaviour {
    public static GridManager Instance { get; private set; }
    private Dictionary<GameObject, GameObject> objectTileMap = new Dictionary<GameObject, GameObject>();
    [SerializeField] private List<TargetPositionData> saveData = new List<TargetPositionData>();
    private List<Vector3> occupiedPositions = new List<Vector3>();
    public float GridCellSize => tileSize;

    private HashSet<Vector3> reservedPositions = new HashSet<Vector3>();

    public bool IsReserved(Vector3 pos) => reservedPositions.Contains(pos);

    public void ReservePosition(Vector3 pos) => reservedPositions.Add(pos);

    public void UnreservePosition(Vector3 pos) => reservedPositions.Remove(pos);

    public GameObject tilePrefab;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    public int gridWidth = 5;
    public int gridHeight = 5;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private GameObject[,] tiles;
    public List<Transform> targetPositions;
    public List<Transform> emptytargetPotations;
    //  [SerializeField] private GameObject opaqueObject;
    public List<GameObject> box;
    [SerializeField] private int countBox;
    [SerializeField] private float ObjectZValue;
    [SerializeField] private float BoxZValue = -1.7f;
    public GameObject SpiderWeb;
    private bool areAllTargetPositionsSet = false;
    [SerializeField] private GameObject _opaqueObjects;
    public List<GameObject> _opaqueList;
    public event Action<List<Transform>> OnEmptyTargetPotationsChanged;
    private int lastEmptyTargetCount = -1; // Track the last known count of empty targets to avoid redundant logs
    public List<GameObject> CustomerObjectPrefabs; // List of customer prefabs for name matching
    // Dictionary For Save First Setting Color
    private Dictionary<GameObject, Color> originalColors;
    const string BoxBuilderExecutedKey = "BoxBuilderExecuted";
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {

        originalColors = new Dictionary<GameObject, Color>();

        CreateGrid(); // ابتدا گرید باید ساخته بشه تا targetPositions آماده باشه

        for (int i = 0; i < _opaqueObjects.transform.childCount; i++) {
            _opaqueList.Add(_opaqueObjects.transform.GetChild(i).gameObject);
        }

        string savePath = Application.persistentDataPath + "/savefile.json";

        if (PlayerPrefs.GetInt(BoxBuilderExecutedKey, 0) == 0) {
            Debug.Log("First time setup: building initial grid");
            BoxBuilder();
            SaveTargetPositions();
            PlayerPrefs.SetInt(BoxBuilderExecutedKey, 1);
            PlayerPrefs.Save();
        }
        else if (File.Exists(savePath)) {
            Debug.Log("Loading existing game data...");
            LoadTargetPositions();
        }
        else {
            Debug.LogWarning("No save file found, even though BoxBuilder has run.");
        }

        BoxAndSpiderUpdaterObjs();
    }


    void Update() {
        //MagnetizeObjectsToTarget();  // فراخوانی تابع جذب به مرکز
        UpdateSpiderWebCollider();
        EmptyTargetPositions();
        CheckAndResetTileColors();
        UpdateCustomerMatchTileColors();
    }

    private ChildData SaveChild(Transform child, int depth = 0, int maxDepth = 5) {
        if (depth > maxDepth) return null;

        var id = child.GetComponent<PrefabIdentifier>();

        ChildData data = new ChildData() {
            position = child.position,
            isActive = child.gameObject.activeSelf,
            objName = id != null ? id.prefabKey : child.gameObject.name.Replace(" (Clone)", "").Trim(),
            children = new List<ChildData>()
        };

        foreach (Transform subChild in child) {
            var subData = SaveChild(subChild, depth + 1, maxDepth);
            if (subData != null)
                data.children.Add(subData);
        }

        return data;
    }
    public bool IsPositionEmpty(Vector3 position) {
        foreach (Transform target in targetPositions) {
            float dist = Vector3.Distance(target.position, position);

            // اگر فاصله خیلی کم باشه، یعنی تو همون نقطه‌ایم
            if (dist < tileSize * 0.4f) {
                return target.childCount == 0; // یعنی خالیه
            }
        }
        return false; // اصلاً داخل targetPositions نبود یا نزدیک نبود
    }

    public void MarkPositionAsOccupied(Vector3 position) {
        if (!occupiedPositions.Contains(position)) {
            occupiedPositions.Add(position);
        }
    }
    public void MarkPositionAsFree(Vector3 position) {
        if (occupiedPositions.Contains(position)) {
            occupiedPositions.Remove(position);
        }
    }
    public Transform GetEmptyTargetPosition(Vector3 position) {
        foreach (var target in targetPositions) {
            if (Vector3.Distance(target.position, position) < 0.5f && target.childCount > 0) {
                return target;
            }
        }
        return null;
    }


    public void MergeObjects(MergeableBase a, MergeableBase b) {

    }

    public void SaveTargetPositions() {
        var wrapper = new SavedDataWrapper();

        foreach (Transform target in targetPositions) {

            TargetPositionData data = new TargetPositionData(target.position);

            // Debug.Log($"targetPositions.Count: {targetPositions.Count}");

            foreach (Transform child in target) {

                var childData = SaveChild(child);
                if (childData != null) {
                    // Debug.Log($"save child: {child.name}");
                    data.children.Add(childData);
                }
            }

            wrapper.allTargets.Add(data);
        }

        saveData = wrapper.allTargets;

        Debug.Log($"Saving {wrapper.allTargets.Count} target position.");
        foreach (var target in wrapper.allTargets) {
            //  Debug.Log($"target {target.position} has {target.children.Count} children.");
        }

        string jsonString = JsonUtility.ToJson(wrapper, true);
        string path = Application.persistentDataPath + "/savefile.json";
        File.WriteAllText(path, jsonString);
        Debug.Log("Target positions saved!");
        Debug.Log($"Saved path: {path}");
        Debug.Log($"Saved {saveData.Count} target to file.");
    }

    private GameObject LoadChild(ChildData data, Transform parent) {
        // Debug.Log($"[LoadChild] Trying to find prefab: {data.objName}");
        GameObject prefab = CustomerObjectPrefabs.FirstOrDefault(x => {
            var id = x.GetComponent<PrefabIdentifier>();
            return id != null && id.prefabKey == data.objName;
        });
        if (prefab == null) {
            //   Debug.LogWarning($"[LoadChild] Could not find prefab with name: {data.objName}");
            return null;
        }

        GameObject obj = Instantiate(prefab, data.position, Quaternion.identity);
        obj.transform.SetParent(parent);
        obj.SetActive(data.isActive);


        foreach (ChildData childData in data.children) {
            LoadChild(childData, obj.transform);
        }

        return obj;
    }
    public void LoadTargetPositions() {
        Debug.Log($"[Load] Loading {saveData.Count} targets");
        string path = Application.persistentDataPath + "/savefile.json";
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        SavedDataWrapper wrapper = JsonUtility.FromJson<SavedDataWrapper>(json);
        saveData = wrapper.allTargets;

        for (int i = 0; i < targetPositions.Count; i++) {
            if (i >= saveData.Count) continue;

            TargetPositionData targetData = saveData[i];
            Transform targetTransform = targetPositions[i];
            targetTransform.position = targetData.position;
            //  Debug.Log($"[Load] Target {i} - Children to load: {targetData.children.Count}");
            // غیرفعال کردن بچه‌های اضافی
            for (int j = targetTransform.childCount - 1; j >= 0; j--) {
                Destroy(targetTransform.GetChild(j).gameObject);
            }

            foreach (ChildData childData in targetData.children) {
                var obj = LoadChild(childData, targetTransform);
                if (obj == null) Debug.LogWarning($"[Load] Failed to instantiate {childData.objName}");
            }

            MarkPositionAsOccupied(targetTransform.position);
        }
        Debug.Log("Target positions loaded!");
    }
    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            SaveTargetPositions();
        }
    }
    private void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            SaveTargetPositions();
        }
    }
    private void OnApplicationQuit() {
        SaveTargetPositions();
    }
    public void EmptyTargetPositions() {
        // Clear the list to ensure it's fresh each time
        emptytargetPotations.Clear();

        foreach (Transform target in targetPositions) {
            if (target.childCount == 0) {
                bool isEmpty = true;
                // Check all children of the current target
                foreach (Transform child in target) {
                    if (child == null || child.gameObject == null || !child.gameObject.activeInHierarchy) {
                        continue;  // Skip invalid or inactive children
                    }

                    // Retrieve required components to validate the object
                    SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                    ObjectMerge childObjectMerge = child.GetComponent<ObjectMerge>();
                    ScatterObjectsWithInterval childScatterObjects = child.GetComponent<ScatterObjectsWithInterval>();

                    if (childSpriteRenderer != null || childObjectMerge != null || childScatterObjects != null) {
                        isEmpty = false; // Mark as not empty if valid components are found
                        break;
                    }
                }
                // If no valid children were found, consider this target empty
                if (isEmpty) {
                    emptytargetPotations.Add(target);
                    target.GetComponent<Collider>().enabled = true;
                }
            }
            else {
                target.GetComponent<Collider>().enabled = false;
            }
        }
    }
    public bool IsPositionInGrid(Vector3 position) {
        float minX = 0;
        float maxX = gridWidth * tileSize;
        float minY = 0;
        float maxY = gridHeight * tileSize;

        float x = Mathf.Round(position.x / tileSize) * tileSize;
        float y = Mathf.Round(position.y / tileSize) * tileSize;

        return (x >= minX && x < maxX) && (y >= minY && y < maxY);
    }

    private void UpdateEmptyTargetPotations() {
        // لیست تغییر کرده است
        OnEmptyTargetPotationsChanged?.Invoke(emptytargetPotations);
    }

    private void UpdateSpiderWebCollider() {
        if (targetPositions == null || SpiderWeb == null) return;

        // Get the SpriteRenderer of the SpiderWeb GameObject
        var spiderWebSpriteRenderer = SpiderWeb.GetComponent<SpriteRenderer>();
        if (spiderWebSpriteRenderer == null) return;  // No SpriteRenderer found on SpiderWeb

        // Iterate over each targetPosition
        foreach (Transform target in targetPositions) {
            bool hasBoxChild = false;

            // Check each child of the targetPosition to see if it has the "Box" tag
            for (int i = 0; i < target.childCount; i++) {
                Transform child = target.GetChild(i);

                // If the child has the "Box" tag, flag it
                if (child.CompareTag("Box")) {
                    hasBoxChild = true;
                    break; // No need to check further if we found a "Box" tagged child
                }
            }

            // Check each child of the targetPosition
            for (int i = 0; i < target.childCount; i++) {
                Transform child = target.GetChild(i);

                // Ensure we don't modify the collider for children with the "Box" tag
                if (child.CompareTag("Box")) continue;


                // Check if the child has a SpriteRenderer (and is the correct one based on SpiderWeb's sprite)
                var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null && childSpriteRenderer.sprite == spiderWebSpriteRenderer.sprite) {
                    // If this child has the same sprite as SpiderWeb, disable its collider if there is a "Box" child
                    var spiderWebCollider = child.GetComponent<Collider>();
                    if (spiderWebCollider != null) {
                        spiderWebCollider.enabled = !hasBoxChild;  // Disable if Box child exists, else enable
                    }
                }
                else {
                    // For non-SpiderWeb children, enable their collider only if there's no Box child
                    var childCollider = child.GetComponent<Collider>();
                    if (childCollider != null) {
                        childCollider.enabled = !hasBoxChild;  // Enable collider only if no Box child exists
                    }
                }
            }
        }
    }
    private void BoxAndSpiderUpdaterObjs() {
        for (int i = 0; i < targetPositions.Count; i++) {
            Transform targetTransform = targetPositions[i].transform;

            for (int j = 0; j < targetTransform.childCount; j++) {
                GameObject child = targetTransform.GetChild(j).gameObject;

                if (child.CompareTag("Box")) {
                    GameObject box = child;

                    // دریافت کامپوننت ObjectMerge از فرزندان target
                    ObjectMerge objectMerge = targetTransform.GetComponentInChildren<ObjectMerge>();
                    if (objectMerge != null) {
                        objectMerge.box = box;
                    }
                    // اگر ObjectMerge پیدا نشد، بررسی کامپوننت ScatterObjectsWithInterval
                    else {
                        ScatterObjectsWithInterval scatter = targetTransform.GetComponentInChildren<ScatterObjectsWithInterval>();
                        if (scatter != null) {
                            scatter.box = box;
                        }
                    }
                }

                if (child.GetComponentInChildren<SpiderScript>() != null) {
                    GameObject spider = child;

                    ObjectMerge objectMerge = targetTransform.GetComponentInChildren<ObjectMerge>();
                    if (objectMerge != null) {
                        objectMerge.SpiderWeb = spider;
                    }
                    // اگر ObjectMerge پیدا نشد، بررسی کامپوننت ScatterObjectsWithInterval
                    else {
                        ScatterObjectsWithInterval scatter = targetTransform.GetComponentInChildren<ScatterObjectsWithInterval>();
                        if (scatter != null) {
                            scatter.SpiderWeb = spider;
                        }
                    }
                }
            }
        }
    }

    public bool IsBox(GameObject obj) {
        return obj.TryGetComponent<ObjectMerge>(out ObjectMerge objectMerge) && objectMerge.box != null;
    }

    public bool IsSpiderWeb(GameObject obj) {
        return obj.TryGetComponent<ObjectMerge>(out ObjectMerge objectMerge) && objectMerge.SpiderWeb != null; // بررسی بر اساس تگ
    }
    void MagnetizeObjectsToTarget() {
        foreach (Transform target in targetPositions) {
            foreach (Transform child in target) {
                Vector3 direction = (target.position - child.position).normalized;
                float distance = Vector3.Distance(child.position, target.position);

                // حرکت به سمت مرکز target با سرعت مشخص
                child.position = Vector3.MoveTowards(child.position, target.position, distance * Time.deltaTime * 7f); // تغییر سرعت به دلخواه
            }
        }
    }
    void BoxBuilder() {
        int boxCount = Mathf.Min(countBox, targetPositions.Count);
        int boxIndex = 0;
        int opaqueIndex = 0;

        for (int i = 0; i < boxCount; i++) {
            Transform target = targetPositions[i];

            // فقط وقتی Child نداره، ایجاد کن
            if (target.childCount > 0)
                continue;

            GameObject currentOpaqueObject = _opaqueList[opaqueIndex];
            Vector3 opaquePosition = new Vector3(target.position.x, target.position.y, ObjectZValue);
            GameObject newOpaqueObject = Instantiate(currentOpaqueObject, opaquePosition, Quaternion.identity);
            newOpaqueObject.transform.SetParent(target);
            newOpaqueObject.transform.localPosition = new Vector3(0, 0, ObjectZValue); // دقیق بچسبه وسط

            GameObject newBox = null;
            GameObject newSpiderObject = null;

            if (i != 31 && i != 38) {
                GameObject currentBox = box[boxIndex];
                Vector3 boxPosition = new Vector3(target.position.x, target.position.y, BoxZValue);
                newBox = Instantiate(currentBox, boxPosition, Quaternion.identity);
                newBox.transform.SetParent(target);
                newBox.transform.localPosition = new Vector3(0, 0, BoxZValue);

                newSpiderObject = Instantiate(SpiderWeb, opaquePosition, Quaternion.identity);
                newSpiderObject.transform.SetParent(target);
                newSpiderObject.transform.localPosition = new Vector3(0, 0, ObjectZValue);

                boxIndex = (boxIndex + 1) % box.Count;
            }

            // وصل کردن به کامپوننت‌ها
            if (newOpaqueObject.TryGetComponent(out ObjectMerge objectMerge)) {
                objectMerge.box = newBox;
                objectMerge.SpiderWeb = newSpiderObject;
            }

            if (newOpaqueObject.TryGetComponent(out ScatterObjectsWithInterval scatter)) {
                scatter.box = newBox;
                scatter.SpiderWeb = newSpiderObject;
            }

            opaqueIndex = (opaqueIndex + 1) % _opaqueList.Count;
        }

        PrefabIdentifierAdder();
        SaveTargetPositions();
    }

    private void PrefabIdentifierAdder() {
        foreach (Transform target in targetPositions) {
            AddPrefabIdentifierRecursive(target);
        }
    }

    private void AddPrefabIdentifierRecursive(Transform obj) {
        if (obj.GetComponent<PrefabIdentifier>() == null) {
            obj.gameObject.AddComponent<PrefabIdentifier>().prefabKey = obj.name;
        }

        foreach (Transform child in obj) {
            AddPrefabIdentifierRecursive(child);
        }
    }

    /*
    public Transform FindBestEmptyTarget(Vector3 fromPosition) {
        return emptytargetPotations
            .Where(t => t.childCount == 0)
            .OrderBy(t => Vector3.Distance(t.position, fromPosition))
            .FirstOrDefault();
    }*/
    public Transform FindBestEmptyTarget(Vector3 fromPosition) {
        float minDistance = float.MaxValue;
        Transform bestTarget = null;

        foreach (Transform target in targetPositions) {
            if (target.childCount == 0) {
                float dist = Vector3.Distance(fromPosition, target.position);
                if (dist < minDistance) {
                    minDistance = dist;
                    bestTarget = target;
                }
            }
        }
        return bestTarget;
    }
    void CreateGrid() {
        tiles = new GameObject[gridWidth, gridHeight];
        targetPositions = new List<Transform>();

        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tiles[x, y] = tile;

                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) {
                    spriteRenderer = tile.AddComponent<SpriteRenderer>();
                }

                spriteRenderer.sprite = (x + y) % 2 == 0 ? sprite1 : sprite2;
                Color initialColor = spriteRenderer.color; // Save Color
                originalColors[tile] = initialColor;

                GameObject targetPositionObject = new GameObject("TargetPosition");
                targetPositionObject.AddComponent<BoxCollider>();
                Vector2 tilePrefabSize = new Vector2(2.6f, 2.6f);
                BoxCollider boxCollider2D = targetPositionObject.GetComponent<BoxCollider>();
                boxCollider2D.size = tilePrefabSize;
                boxCollider2D.enabled = false;

                targetPositionObject.tag = "TargetPosition";
                targetPositionObject.transform.position = position;
                targetPositionObject.transform.SetParent(tile.transform);
                targetPositions.Add(targetPositionObject.transform);
                var objectMarges = FindObjectsOfType<ObjectMerge>();
            }
        }
    }
    public void UpdateCustomerMatchTileColors() {
        foreach (var mergeable in FindObjectsOfType<MergeableBase>()) {
            if (mergeable == null) continue;
            GameObject obj = mergeable.gameObject;
            GameObject currentTile = GetTileForObject(obj);
            if (objectTileMap.TryGetValue(obj, out GameObject lastTile) && lastTile != currentTile) {
                ChangeTileToOriginalColor(lastTile);
                objectTileMap[obj] = currentTile;
            }
            else if (!objectTileMap.ContainsKey(obj)) {
                objectTileMap[obj] = currentTile;
            }

            if (currentTile == null) continue;
            foreach (var target in targetPositions) {
                if (mergeable.isFoundCustomerMatch && mergeable.box == null && mergeable.SpiderWeb == null &&
                    mergeable.CurrentState != MergeableState.Dragging 
                    && mergeable.CurrentState != MergeableState.MovingToTarget) {
                    ChangeTileColor(currentTile, Color.green);
                }
                else if (target.childCount == 0 && mergeable.CurrentState == MergeableState.Dragging) {
                    ChangeTileToOriginalColor(currentTile);
                }
            }
        }
    }
    public GameObject GetTileForObject(GameObject obj) {
        GameObject closestTile = null;
        float minDist = float.MaxValue;
        foreach (GameObject tile in tiles) {
            float dist = Vector3.Distance(tile.transform.position, obj.transform.position);
            if (dist < minDist) {
                minDist = dist;
                closestTile = tile;
            }
        }
        return closestTile;
    }
    public void ChangeTileColor(GameObject obj, Color color) {
        // Find the tile associated with the object's position
        GameObject tileToColor = null;
        foreach (GameObject tile in tiles) {
            if (Vector3.Distance(tile.transform.position, obj.transform.position) < tileSize * 0.4f) {
                tileToColor = tile;
                break;
            }
        }

        // Always update the tile's color, regardless of previous mapping
        if (tileToColor != null) {
            tileToColor.GetComponent<SpriteRenderer>().color = color;
            objectTileMap[obj] = tileToColor;
        }
    }
    public void ResetTileColorForObject(GameObject obj) {
        // Only reset the color if there are no other objects mapped to this tile
        if (objectTileMap.TryGetValue(obj, out GameObject tile)) {
            // Check if any other object is mapped to this tile
            bool otherObjectOnTile = objectTileMap.Any(kvp => kvp.Key != obj && kvp.Value == tile);

            if (!otherObjectOnTile) {
                ChangeTileToOriginalColor(tile);
            }
            objectTileMap.Remove(obj);
        }
    }
    // Change Color
    public void ChangeTileToOriginalColor(GameObject tile) {
        if (originalColors.ContainsKey(tile)) {
            tile.GetComponent<SpriteRenderer>().color = originalColors[tile];
        }
    }
    // Reset Color
    public void CheckAndResetTileColors() {
        foreach (GameObject tile in tiles) {
            if (tile.transform.GetChild(0).childCount == 0) {
                ChangeTileToOriginalColor(tile);
            }
        }
    }
}