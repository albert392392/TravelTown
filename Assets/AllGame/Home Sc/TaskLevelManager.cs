using System.Collections.Generic;
using UnityEngine;

public class TaskLevelManager : MonoBehaviour {
    public static TaskLevelManager Instance { get; private set; }
    public Dictionary<string, float> TaskLevels = new Dictionary<string, float>(); // Centralized TaskLevels storage

    private const string SaveKeyPrefix = "TaskLevel_"; // Prefix for saved keys

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            LoadTaskLevels(); // Load saved TaskLevels on startup
        } else {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void AddTaskLevel(string key, float value) {
        if (TaskLevels.ContainsKey(key)) {
            TaskLevels[key] += value;
        } else {
            TaskLevels[key] = value;
        }
        SaveTaskLevel(key); // Save the updated value
    }

    public float GetTaskLevel(string key) {
        if (TaskLevels.TryGetValue(key, out float value)) {
            return value;
        }
        return 0f; // Default to 0 if the key does not exist
    }

    private void SaveTaskLevel(string key) {
        if (TaskLevels.TryGetValue(key, out float value)) {
            PlayerPrefs.SetFloat(SaveKeyPrefix + key, value); // Save to PlayerPrefs
            PlayerPrefs.Save();
        }
    }

    public void LoadTaskLevels() {
        TaskLevels.Clear();
        foreach (var key in PlayerPrefsKeys()) {
            TaskLevels[key] = PlayerPrefs.GetFloat(SaveKeyPrefix + key, 0f); // Load existing values
        }
    }

    public void SaveAllTaskLevels() {
        foreach (var kvp in TaskLevels) {
            PlayerPrefs.SetFloat(SaveKeyPrefix + kvp.Key, kvp.Value);
        }
        PlayerPrefs.Save();
    }

    private IEnumerable<string> PlayerPrefsKeys() {
        foreach (var key in PlayerPrefs.GetString(SaveKeyPrefix + "Keys", "").Split(',')) {
            if (!string.IsNullOrWhiteSpace(key)) {
                yield return key;
            }
        }
    }

    private void SaveKeysList() {
        var keys = string.Join(",", TaskLevels.Keys);
        PlayerPrefs.SetString(SaveKeyPrefix + "Keys", keys);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() {
        SaveAllTaskLevels(); // Save all levels on game exit
        SaveKeysList(); // Save the list of keys
    }

    private void OnDisable() {
        SaveAllTaskLevels();
        SaveKeysList();
    }
}
