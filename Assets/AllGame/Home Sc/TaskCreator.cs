
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class TaskCreator : MonoBehaviour {
    public static TaskCreator Instance { get; private set; }
    [SerializeField] private string saveKey;
    [SerializeField] private List<TaskDataList> taskDataLists = new List<TaskDataList>();
    public GameObject slotTaskPrefab;
    public RectTransform parentTransform;
    private List<GameObject> spawnedTasks = new List<GameObject>();
    private List<int> removedTaskIDs = new List<int>();
    private int moneyCount;
    [SerializeField] private string taskKey;
    public GameObject Panel;
    public Transform Ground;
    private void Start() {
        moneyCount = CoinManager.Instance.totalCoins;
        LoadTaskCount();
        CreateTasks();
    }
    private void OnEnable() {
        LoadTaskCount();
        CreateTasks();
    }

    private void OnDisable() {
        SaveTaskCount();
    }

    public void CreateTasks() {


        foreach (var task in spawnedTasks) {
            Destroy(task);
        }
        spawnedTasks.Clear();

        bool taskCreated = false;

        for (int i = 0; i < taskDataLists.Count; i++) {
            TaskDataList taskDataList = taskDataLists[i];

            if (taskDataList.taskDataList.Count > 0) {
                taskCreated = true;
                foreach (var taskData in taskDataList.taskDataList) {
                    if (removedTaskIDs.Contains(taskData.moneyAdd)) {
                        continue;
                    }

                    GameObject newSlotTask = Instantiate(slotTaskPrefab, parentTransform);
                    spawnedTasks.Add(newSlotTask);

                    TextMeshProUGUI[] textComponents = newSlotTask.GetComponentsInChildren<TextMeshProUGUI>();

                    TextMeshProUGUI textMoneyAddClone = textComponents[0];
                    TextMeshProUGUI textTotalAddClone = textComponents[1];
                    Text nameOfTaskClone = newSlotTask.GetComponentInChildren<Text>();
                    Button[] moneyButtonaddClone = newSlotTask.GetComponentsInChildren<Button>();

                    textMoneyAddClone.text = taskData.moneyAdd.ToString();
                    textTotalAddClone.text = taskData.totalLevelAdd.ToString();
                    nameOfTaskClone.text = taskData.taskName;
                    if (moneyCount >= taskData.moneyAdd) {
                        moneyButtonaddClone[0].interactable = true;
                        moneyButtonaddClone[0].onClick.AddListener(() => OnMoneyButtonClick(newSlotTask, taskDataList, taskData));

                        moneyButtonaddClone[1].interactable = true;
                        moneyButtonaddClone[1].onClick.AddListener(() => OnMoneyButtonClick(newSlotTask, taskDataList, taskData));
                    }
                    else {
                        moneyButtonaddClone[0].interactable = false;
                        moneyButtonaddClone[1].interactable = false;
                    }
                    newSlotTask.transform.localPosition = Vector3.zero;
                }
                break;
            }
        }

        if (!taskCreated) {
            Debug.Log("All tasks are completed!");
        }
        SaveTaskCount();
    }

    private void OnMoneyButtonClick(GameObject slotTask, TaskDataList taskDataList, TaskData taskData) {
        Panel.SetActive(false);
        // Deduct money
        CoinManager.Instance.SpendCoins(taskData.moneyAdd);
        
        if (taskData.selectObject.gameObject.GetComponent<AddedAndChangeScriptMenu>() != null) {
            var script = taskData.selectObject.gameObject.GetComponent<AddedAndChangeScriptMenu>();
            script.isObjectActive = true;
            script.gameObject.SetActive(true);

            ObjectMenuScrollBarChoose scrollBarChoose = UiManagerMosque.instance.menuScrollBarChoose;
            ButtonMenuScrollChoose buttonMenuScroll = UiManagerMosque.instance.ButtonMenuScrollChoose;

            script.objectMenuScrollBarChoose = scrollBarChoose;
            script.buttonMenuScrollChoose = buttonMenuScroll;
            script.btnCorrect = buttonMenuScroll.correctButton.gameObject.GetComponent<Button>();
            script.btnNotCorrect = buttonMenuScroll.notCorrectButton.gameObject.GetComponent<Button>();
            script.objectMenu = scrollBarChoose.objectMenu;
            script.scrollBarMenu = UiManagerMosque.instance.ScrollBarMenu;
            PlayerPrefs.SetInt(script.addedChangeKey, script.isObjectActive ? 1 : 0);
            PlayerPrefs.Save();
        }
        else if(taskData.selectObject.gameObject.GetComponent<AddedAndChangeScriptMenu2>() != null) {
            var script = taskData.selectObject.gameObject.GetComponent<AddedAndChangeScriptMenu2>();
            script.isObjectActive = true;
            script.gameObject.SetActive(true);

            ObjectMenuScrollBarChoose scrollBarChoose = UiManagerMosque.instance.menuScrollBarChoose;
            ButtonMenuScrollChoose buttonMenuScroll = UiManagerMosque.instance.ButtonMenuScrollChoose;

            script.objectMenuScrollBarChoose = scrollBarChoose;
            script.buttonMenuScrollChoose = buttonMenuScroll;
            script.btnCorrect = buttonMenuScroll.correctButton.gameObject.GetComponent<Button>();
            script.btnNotCorrect = buttonMenuScroll.notCorrectButton.gameObject.GetComponent<Button>();
            script.objectMenu = scrollBarChoose.objectMenu;
            script.scrollBarMenu = UiManagerMosque.instance.ScrollBarMenu;
            PlayerPrefs.SetInt(script.addedChangeKey, script.isObjectActive ? 1 : 0);
            PlayerPrefs.Save();
        }
        if(taskData.selectObject.gameObject.GetComponent<DraggableObjectCustomizer>() != null) {
            var script = taskData.selectObject.gameObject.GetComponent<DraggableObjectCustomizer>();
            string keyDraggbleObject = taskData.selectObject.gameObject.GetComponent<DraggableObjectCustomizer>().activeObjectKey;
            script.isObjectActive = true;
            script.gameObject.SetActive(true);
            taskData.selectObject.GetComponent<DraggableObjectCustomizer>().SavePositionAndScale();
            PlayerPrefs.SetInt(script.activeObjectKey, script.isObjectActive ? 1 : 0);
            PlayerPrefs.Save();

        }
        /*
        if (taskData.selectObject.GetComponent<DraggableObjectCustomizer>() != null) {
            taskData.selectObject.gameObject.SetActive(true);
        }
        else if (taskData.selectObject.gameObject.GetComponent<AddedAndChangeScriptMenu>() != null) {
            taskData.selectObject.gameObject.SetActive(true);
        }
        else {
            print("CantFindAny!");
        }*/
        // Add level to TaskLevelManager
        if (TaskLevelManager.Instance != null) {
            TaskLevelManager.Instance.AddTaskLevel(taskKey, taskData.taskAdd);
        }
        if (TotalLevelManager.Instance != null) {
            TotalLevelManager.Instance.AddTotalLevelsFill(taskData.totalLevelAdd);
        }
        // Clean up task
        if (spawnedTasks.Contains(slotTask)) {
            spawnedTasks.Remove(slotTask);
            Destroy(slotTask);
        }

        taskDataList.taskDataList.Remove(taskData);

        if (taskDataList.taskDataList.Count == 0) {
            taskDataLists.Remove(taskDataList);
        }

        SaveTaskCount();
        CreateTasks();
    }

    public void SaveTaskCount() {
        string taskDataJson = JsonUtility.ToJson(new TaskDataContainer { taskDataLists = taskDataLists });
        PlayerPrefs.SetString(saveKey, taskDataJson);
        for (int i = 0; i < taskDataLists.Count; i++) {
            foreach (var taskData in taskDataLists[i].taskDataList) {
                if (taskData.selectObject != null) {
                    PlayerPrefs.SetString("Task_" + i + "_SelectObject_" + taskData.taskName, taskData.selectObject.name);
                    PlayerPrefs.SetInt("Task_" + i + "_SelectObjectActive_" + taskData.taskName, taskData.selectObject.activeSelf ? 1 : 0);
                }
            }
        }
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("RemovedTaskCount", removedTaskIDs.Count);
        for (int i = 0; i < removedTaskIDs.Count; i++) {
            PlayerPrefs.SetInt("RemovedTask_" + i, removedTaskIDs[i]);
        }
        PlayerPrefs.Save();
    }

    public void LoadTaskCount() {
        if (PlayerPrefs.HasKey(saveKey)) {
            string taskDataJson = PlayerPrefs.GetString(saveKey);
            TaskDataContainer loadedContainer = JsonUtility.FromJson<TaskDataContainer>(taskDataJson);
            taskDataLists = loadedContainer.taskDataLists;
        }
        for(int i = 0;i < taskDataLists.Count;i++) {
            foreach(var taskData in taskDataLists[i].taskDataList) {
                string selectObjectName = PlayerPrefs.GetString("Task_" + i + "_SelectObject_" + taskData.taskName);
                if(!string.IsNullOrEmpty(selectObjectName)) {
                    GameObject selectObject = GameObject.Find(selectObjectName);
                    if(selectObject != null) {
                        taskData.selectObject = selectObject;

                        bool isActive = PlayerPrefs.GetInt("Task_" + i + "_SelectObjectActive_" + taskData.taskName) == 1;
                        taskData.selectObject.SetActive(isActive);
                    }
                }
            }

        }
        if (PlayerPrefs.HasKey("RemovedTaskCount")) {
            int removedTaskCount = PlayerPrefs.GetInt("RemovedTaskCount");
            removedTaskIDs.Clear();

            for (int i = 0; i < removedTaskCount; i++) {
                removedTaskIDs.Add(PlayerPrefs.GetInt("RemovedTask_" + i));
            }
        }
    }
    private void OnApplicationFocus(bool focus) {
        SaveTaskCount();
    }
    private void OnApplicationPause(bool pause) {
        SaveTaskCount();
    }
    private void OnApplicationQuit() {
        SaveTaskCount();
    }
    [Serializable]
    public class TaskData {
        public GameObject selectObject;
        public int moneyAdd;
        public float taskAdd;
        public float totalLevelAdd;
        public string taskName;
    }

    [Serializable]
    public class TaskDataList {
        public List<TaskData> taskDataList = new List<TaskData>();
    }

    [Serializable]
    public class TaskDataContainer {
        public List<TaskDataList> taskDataLists = new List<TaskDataList>();
    }
}