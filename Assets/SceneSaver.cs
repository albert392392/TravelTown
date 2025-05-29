using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class SceneData {
    public string sceneName;
    public List<ObjectData> objectsData = new List<ObjectData>();
}

[System.Serializable]
public class ObjectData {
    public string objectName;
    public string parentName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool isActive;
    public Dictionary<string, string> componentData = new Dictionary<string, string>();
}

[System.Serializable]
public class SceneDataContainer {
    public List<SceneData> scenes = new List<SceneData>();
}

public class SceneSaver : MonoBehaviour {
    private static SceneSaver instance;
    private SceneDataContainer allScenesData = new SceneDataContainer();
    private string savePath;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "sceneData.json");
        }
        else {
            Destroy(gameObject);
        }
    }

    public static SceneSaver Instance => instance;

    public void SaveAllScenes() {
        allScenesData.scenes.Clear();

        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            SceneData sceneData = new SceneData { sceneName = scene.name };

            foreach (GameObject rootObj in scene.GetRootGameObjects()) {
                SaveGameObjectRecursive(rootObj, sceneData.objectsData, null);
            }

            allScenesData.scenes.Add(sceneData);
        }

        string json = JsonUtility.ToJson(allScenesData, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"All scenes saved to: {savePath}");
    }

    private void SaveGameObjectRecursive(GameObject obj, List<ObjectData> objectsData, string parentName) {
        ObjectData objData = new ObjectData {
            objectName = obj.name,
            parentName = parentName,
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            scale = obj.transform.localScale,
            isActive = obj.activeSelf,
            componentData = SaveComponentData(obj)
        };

        objectsData.Add(objData);

        foreach (Transform child in obj.transform) {
            SaveGameObjectRecursive(child.gameObject, objectsData, obj.name);
        }
    }

    private Dictionary<string, string> SaveComponentData(GameObject obj) {
        Dictionary<string, string> componentData = new Dictionary<string, string>();

        TextMeshProUGUI textComponent = obj.GetComponent<TextMeshProUGUI>();
        if (textComponent != null) {
            componentData["TextMeshProUGUI"] = textComponent.text;
        }

        Button buttonComponent = obj.GetComponent<Button>();
        if (buttonComponent != null) {
            componentData["ButtonInteractable"] = buttonComponent.interactable.ToString();
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) {
            componentData["RendererMaterial"] = renderer.material.name;
        }

        return componentData;
    }

    public void LoadAllScenes() {
        if (!File.Exists(savePath)) {
            Debug.LogWarning("Save file not found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        allScenesData = JsonUtility.FromJson<SceneDataContainer>(json);

        foreach (SceneData sceneData in allScenesData.scenes) {
            if (SceneManager.GetActiveScene().name == sceneData.sceneName) {
                foreach (ObjectData objData in sceneData.objectsData) {
                    GameObject obj = GameObject.Find(objData.objectName) ?? new GameObject(objData.objectName);

                    obj.transform.position = objData.position;
                    obj.transform.rotation = objData.rotation;
                    obj.transform.localScale = objData.scale;
                    obj.SetActive(objData.isActive);

                    if (!string.IsNullOrEmpty(objData.parentName)) {
                        GameObject parent = GameObject.Find(objData.parentName);
                        if (parent != null) obj.transform.SetParent(parent.transform);
                    }

                    LoadComponentData(obj, objData.componentData);
                }
            }
        }

        Debug.Log("All scenes loaded.");
    }

    private void LoadComponentData(GameObject obj, Dictionary<string, string> componentData) {
        if (componentData.TryGetValue("TextMeshProUGUI", out string textValue)) {
            TextMeshProUGUI textComponent = obj.GetComponent<TextMeshProUGUI>();
            if (textComponent != null) {
                textComponent.text = textValue;
            }
        }

        if (componentData.TryGetValue("ButtonInteractable", out string interactableValue)) {
            Button buttonComponent = obj.GetComponent<Button>();
            if (buttonComponent != null) {
                buttonComponent.interactable = bool.Parse(interactableValue);
            }
        }

        if (componentData.TryGetValue("RendererMaterial", out string materialName)) {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null) {
                Material loadedMaterial = Resources.Load<Material>(materialName);
                if (loadedMaterial != null) {
                    renderer.material = loadedMaterial;
                }
            }
        }
    }

    private void OnApplicationQuit() {
        SaveAllScenes();
    }

    private void OnApplicationPause(bool pause) {
        if (pause) SaveAllScenes();
    }
}
