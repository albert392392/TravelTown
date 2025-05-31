using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InventorySaveData {
    public List<ChildData> children = new List<ChildData>();
}

public class inventorySaveLoadScript : MonoBehaviour {
    public static inventorySaveLoadScript Instance { get; private set; }
    public InventoryManager inventoryManager;
    public List<GameObject> customerPrefabs; // Prefabs that exist in Inventory
    public bool hasSaved = false;
    private string savePath => Application.persistentDataPath + "/inventory.json";

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // حفظ آبجکت هنگام تغییر سین
        }
        else {
            Destroy(gameObject);
        }
    }
    private IEnumerator Start() {
        yield return new WaitForSeconds(0.1f); // مطمئن شو Canvas آماده است
        LoadInventory();
    }

    public void SaveInventory() {
        InventorySaveData data = new InventorySaveData();

        foreach (Transform item in inventoryManager.mergeableParent) {
            if (item.GetComponent<PrefabIdentifier>() == null) continue;
            var childData = SaveChild(item);
            if (childData != null)
                data.children.Add(childData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Inventory saved to: " + savePath);
    }

    public void LoadInventory() {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        foreach (Transform child in inventoryManager.mergeableParent) {
            Destroy(child.gameObject);
        }

        inventoryManager.inventorySlots.Clear();

        foreach (ChildData childData in data.children) {
            LoadChild(childData, inventoryManager.mergeableParent);
        }

        Debug.Log("Inventory loaded from: " + savePath);
    }

    private ChildData SaveChild(Transform child) {
        var id = child.GetComponent<PrefabIdentifier>();

        return new ChildData() {
            position = child.position,
            isActive = child.gameObject.activeSelf,
            scale = child.localScale,
            objName = id != null ? id.prefabKey : child.name.Replace(" (Clone)", "").Trim(),
            children = new List<ChildData>()
        };
    }

    private void LoadChild(ChildData data, Transform parent) {
        GameObject prefab = customerPrefabs.FirstOrDefault(x => {
            var id = x.GetComponent<PrefabIdentifier>();
            return id != null && id.prefabKey == data.objName;
        });

        if (prefab == null) {
            Debug.LogWarning("Missing prefab: " + data.objName);
            return;
        }

        GameObject obj = Instantiate(prefab, data.position, Quaternion.identity,parent);
        obj.transform.localScale = data.scale;

        //obj.GetComponent<MergeableBase>().In_inventory();

        inventoryManager.inventorySlots.Add(obj);

        var mergeableBase = obj.GetComponent<MergeableBase>();
        if(mergeableBase != null) {
            mergeableBase.originalScale = data.scale; // Set original scale for mergeable items
            mergeableBase.inventoryItemDragHandler = FindObjectOfType<InventoryItemDragHandler>();
            obj.SetActive(false);
        }
        // Add to inventorySlots list
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus)
            SaveInventory();
    }
    private void OnApplicationQuit() {
        if (!hasSaved) {
            SaveInventory();
            hasSaved = true;
        }
    }
}
