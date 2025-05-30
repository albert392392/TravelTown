
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using static ButtonManager;
using UnityEngine.SceneManagement;
using DanielLochner.Assets.SimpleScrollSnap;

public class ButtonManager : MonoBehaviour {
    public static ButtonManager instance { get; private set; }
    public Items items;
    public Buttons buttons;
    public GamePlay gamePlay;
    public LocationCanvas locationCanvas;
    public GameObject levelNowObjectGameplay;
    [SerializeField] private List<GameObject> _gameObjectsSession = new List<GameObject>();
    [SerializeField] private GameObject _sessionNowGameplay;
    [SerializeField] private ButtonSessionManager buttonSessionManager;
    private float refreshTimer = 0f;
    private void Awake() {
        
        if (!this.enabled) {
            this.enabled = true;
        }

        Debug.Log("ButtonManager enabled: " + this.GetComponent<ButtonManager>().enabled);
    }

    private void Start() {
        if (gamePlay != null) {
            gamePlay.RefreshSessions();
            gamePlay.EnsureAtLeastOneLevelActive();
            gamePlay.LoadAndActivateLastLevel();
        }
        // print(items.Panel.gameObject.name);
        if (items.Panel.gameObject != null) {
            items.Panel.gameObject.SetActive(false);
        }
        InitializeButtons();
        // gamePlay.InitializeSessions();
        //  locationCanvas.InitializeSessions();
        if (!this.enabled) {
            this.enabled = true;
        }
    }
    private void Update() {
        refreshTimer += Time.deltaTime;
        if(refreshTimer >= 0.5) {
            refreshTimer = 0f;

            if (gamePlay != null)
                gamePlay.RefreshSessions();
            if (locationCanvas != null)
                locationCanvas.RefreshSessions();
        }
        if (locationCanvas != null) {
            //  Debug.Log("Running CheckConditions in Update");
            locationCanvas.CheckConditions();
        }

        if (_sessionNowGameplay != null && _sessionNowGameplay.transform.childCount > 0) {
            _gameObjectsSession.Clear();
            Transform firstChild = _sessionNowGameplay.transform.GetChild(0);
            if (items.Panel.activeSelf == true) {
                DisableWhenPanelOpen(firstChild);
            }
            else if (items.Panel.activeSelf == false) {
                AddStateSessionRecursiveOn(firstChild);
            }
        }
    }
    private void InitializeButtons() {
        buttons.BackFromPanelButton.onClick.AddListener(OnBackButtonClicked);
        buttons.LocationButton.onClick.AddListener(OnLocationButton);
       // buttons.SessionButton.onClick.AddListener(OnSessionButton);
    }

    // ...

    public void OnLevelButtonClick(int sessionIndex, int levelIndex)
    {
        // Validate indices
        if (!IsValidSessionLevel(sessionIndex, levelIndex))
        {
            Debug.LogWarning($"Invalid sessionIndex ({sessionIndex}) or levelIndex ({levelIndex}) in OnLevelButtonClick.");
            return;
        }
        var selectedLevel = gamePlay.Sessions[sessionIndex].Levels[levelIndex];

        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player != null) {
            player.LoadPositionAndScale();
            print("Player CHHHHHHHHHHHHHH");
        }
        DraggableObjectCustomizer[] customizers = FindObjectsOfType<DraggableObjectCustomizer>();
        foreach (var customizer in customizers) {
            if (customizer != null) {
                customizer.LoadPositionAndScale();
                print("Draggable DDDDDDDD");
            }
        }

        /*
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        Foot foot = FindObjectOfType<Foot>();
        if (player != null) {
            player.Awake();
            player.Start();
            player.Update();
            print("Player CHHHHHHHHHHHHHH");
        }
        if (foot != null) {
            foot.Start();
            foot.Update();
            print("Foot OOOOOOO");
        }
        DraggableObjectCustomizer[] customizers = FindObjectsOfType<DraggableObjectCustomizer>();

        FootDownDraggable[] footDown = FindObjectsOfType<FootDownDraggable>();
        foreach (var customizer in customizers) {
            customizer.Awake();
            customizer.Start();
            customizer.Update();
            print("Draggable DDDDDDDD");
        }
        foreach(var footDownDraggable in footDown) {
            footDownDraggable.Start();
            footDownDraggable.Update();
            print("FOOT DOWN NNNNNNNNN");
        }
        */
        // Always deactivate all levels in session and activate the selected one
        DeactivateAllLevelsInSession(sessionIndex);
        ActivateLevel(sessionIndex, levelIndex);

        // Reload scripts after activation to ensure scripts are refreshed for every click
        ReloadAllScripts();

        // Update ButtonIndex value for the clicked button
        var buttonIndex = selectedLevel.GetComponent<ButtonIndex>();
        if (buttonIndex != null)
        {
            buttonIndex.SessionIndex = sessionIndex;
            buttonIndex.LevelIndex = levelIndex;
            Debug.Log("LevelIndex ButtonIndex :" + buttonIndex.LevelIndex + " OnLevelButtonClick func :" + levelIndex);
        }

        // Update button interactable states for all levels in all sessions
        foreach (var session in locationCanvas.Sessions)
        {
            foreach (var level in session.Levels)
            {
                var btnIdx = level.GetComponent<ButtonIndex>();
                var button = btnIdx != null ? btnIdx.GetComponent<Button>() : null;
                if (button != null)
                {
                    button.interactable = (selectedLevel != level);
                }
            }
        }
    }
    public void ReloadAllScripts() {
        // Refresh gameplay sessions
        if (gamePlay != null)
            gamePlay.RefreshSessions();

        // Refresh location canvas sessions
        if (locationCanvas != null)
            locationCanvas.RefreshSessions();

        // Optionally, reset UI panels or other states
        if (items != null && items.Panel != null)
            items.Panel.SetActive(false);

    }

    private bool IsValidSessionLevel(int sessionIndex, int levelIndex) {
        if (sessionIndex < 0 || sessionIndex >= gamePlay.Sessions.Count) return false;
        if (levelIndex < 0 || levelIndex >= gamePlay.Sessions[sessionIndex].Levels.Count) return false;
        return true;
    }

    private void DeactivateAllLevelsInSession(int sessionIndex) {
        foreach (var level in gamePlay.Sessions[sessionIndex].Levels) {
            if (level != null) level.SetActive(false);
        }
    }

    private void ActivateLevel(int sessionIndex, int levelIndex) {
        var level = gamePlay.Sessions[sessionIndex].Levels[levelIndex];
        if (level != null) {
            level.SetActive(true);
            levelNowObjectGameplay = level;
          //  PlayGifAnim.instance.PlayVideo();
            Debug.Log($"Activated Level {levelIndex} in Session {sessionIndex}");
        }
    }


    public void OnBackButtonClicked() {
        items.Panel.SetActive(false);
    }

    public void OnLocationButton() {

        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player != null) {
            player.SavePositionAndScale();
            print("Player CHHHHHHHHHHHHHH");
        }
        DraggableObjectCustomizer[] customizers = FindObjectsOfType<DraggableObjectCustomizer>();
        foreach (var customizer in customizers) {
            if (customizer != null) {
                customizer.SavePositionAndScale();
                print("Draggable DDDDDDDD");
            }
        }
        items.Panel.SetActive(true);
        levelNowObjectGameplay.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void DisableWhenPanelOpen(Transform parentTransform) {
        if (_sessionNowGameplay != null) {
            AddStateSessionRecursiveOff(parentTransform);
        }
    }
    public void AddStateSessionRecursiveOff(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            var scriptMenu = child.GetComponent<AddedAndChangeScriptMenu>();
            if (scriptMenu != null) scriptMenu.enabled = false;

            var scriptMenu2 = child.GetComponent<AddedAndChangeScriptMenu2>();
            if (scriptMenu2 != null) scriptMenu2.enabled = false;

            var playerCharacter = child.GetComponent<PlayerCharacter>();
            if (playerCharacter != null) playerCharacter.enabled = false;

            var draggableCustomizer = child.GetComponent<DraggableObjectCustomizer>();
            if (draggableCustomizer != null) draggableCustomizer.enabled = false;



            _gameObjectsSession.Add(child.gameObject);
            AddStateSessionRecursiveOff(child);
        }
    }
    private void AddStateSessionRecursiveOn(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            var scriptMenu = child.GetComponent<AddedAndChangeScriptMenu>();
            if (scriptMenu != null) scriptMenu.enabled = true;

            var scriptMenu2 = child.GetComponent<AddedAndChangeScriptMenu2>();
            if (scriptMenu2 != null) scriptMenu2.enabled = true;

            var playerCharacter = child.GetComponent<PlayerCharacter>();
            if (playerCharacter != null) playerCharacter.enabled = true;

            var draggableCustomizer = child.GetComponent<DraggableObjectCustomizer>();
            if (draggableCustomizer != null) draggableCustomizer.enabled = true;
            
            AddStateSessionRecursiveOn(child);
        }
        ReloadAllScripts();
    }
    public void OnLevelButtonClickBackLow(GameObject parent) {
        StartCoroutine(BackSequence(parent));
    }
    public void OnLevelButtonClickBack(GameObject parent) {
        parent.SetActive(false);
    }

    private IEnumerator BackSequence(GameObject parent) {
        if (buttonSessionManager.isExpanded) {
            yield return StartCoroutine(buttonSessionManager.MoveChildrenBack());
        }
        if (buttonSessionManager.isAnimating == false) {
            parent.SetActive(false);
        }
    }

    public void OnLevelButtonClickOpen(GameObject child) {
        child.SetActive(true);
    }

    [System.Serializable]
    public class Session {
        public GameObject SessionObject;
        public List<GameObject> Levels = new List<GameObject>();
    }


    [System.Serializable]
    public class LocationCanvas {
        public Transform LocationSessionParent;
        public List<Session> Sessions = new List<Session>();

        public void RefreshSessions() {
            if (LocationSessionParent == null) return; 
            Sessions.Clear();
            foreach (Transform sessionTransform in LocationSessionParent.transform.GetChild(1).transform.GetChild(0)) {
                Session newSession = new Session {
                    SessionObject = sessionTransform.gameObject,
                    Levels = new List<GameObject>()
                };
                foreach (Transform levelTransform in sessionTransform) {
                    newSession.Levels.Add(levelTransform.gameObject);
                }
                Sessions.Add(newSession);
            }
        }

        public void CheckConditions() {
            if (Sessions == null || Sessions.Count == 0) return;

            foreach (var session in Sessions) {
                if (session == null || session.Levels == null) continue;

                foreach (var levelObject in session.Levels) {
                    if (levelObject == null) continue;

                    // Check if the Level has the TaskLevelDisplay script
                    var taskLevelDisplay = levelObject.GetComponentInChildren<TaskLevelDisplay>();
                    if (taskLevelDisplay == null) continue;

                    if (taskLevelDisplay.currentValue == 100f) {
                        var nextImageIndexScript = levelObject.GetComponent<NextImageIndex>();
                        Image nextImageIndex = nextImageIndexScript.nextImage;
                        if (nextImageIndex != null) {
                            Image levelImage = levelObject.GetComponent<Image>();
                            if (levelImage != null) {
                                levelImage.sprite = nextImageIndexScript.nextImage.sprite;
                            }
                        }

                        // Get the next Level
                        int currentIndex = session.Levels.IndexOf(levelObject);
                        if (currentIndex >= 0 && currentIndex < session.Levels.Count - 1) {
                            var nextLevel = session.Levels[currentIndex + 1];

                            // Destroy the next Level and activate children
                            if (nextLevel != null && nextLevel) {
                               // Debug.Log($"Destroying next Level: {nextLevel.transform.GetChild(0).gameObject.name}");
                                if (nextLevel.transform.GetChild(0).GetComponent<Button>()) {
                                    Destroy(nextLevel.transform.GetChild(0).gameObject);
                                }/*
                                else {
                                    Debug.Log("Not Found Button in nextLevel child");
                                }*/
                            }
                            for (int i = 0; i < nextLevel.transform.childCount; i++) {
                                if (nextLevel.transform.GetChild(i) == null) continue;

                                nextLevel.transform.GetChild(i).gameObject.SetActive(true);
                                //Debug.Log($"Activated child: {nextLevel.transform.GetChild(i).name}");

                                // Enable the button in each child
                                var buttonIndex = nextLevel.GetComponent<ButtonIndex>();
                                if (buttonIndex != null) {
                                    var button = nextLevel.GetComponentInChildren<Button>();
                                    if (button != null) {
                                        button.enabled = true;
                                       // Debug.Log($"Enabled button on {nextLevel.name}");
                                    }/*
                                    else {
                                        //Debug.LogWarning($"No Button component found on {nextLevel.name}. Skipping this child.");
                                    }*/
                                }/*
                                else {
                                   // Debug.LogWarning($"No ButtonIndex component found on {nextLevel.name}. Skipping this child.");
                                }*/

                            }
                        }
                        /*
                        else {
                          //  Debug.LogWarning($"Next level does not exist or is out of bounds for {levelObject.name}.");
                        }*/
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class GamePlay
    {
        public Transform SessionParent;
        public List<Session> Sessions = new List<Session>();

        private const string LastLevelKey = "LastActiveLevelPath";

        // Call this to save the last active level
        public void SaveLastActiveLevel(GameObject levelObj)
        {
            if (levelObj == null) return;
            // Save the hierarchy path for the level GameObject
            string path = GetGameObjectPath(levelObj);
            PlayerPrefs.SetString(LastLevelKey, path);
            PlayerPrefs.Save();
        }

        // Call this at game start to load and activate the last active level
        public void LoadAndActivateLastLevel()
        {
            if (SessionParent == null) return;
            string path = PlayerPrefs.GetString(LastLevelKey, "");
            if (string.IsNullOrEmpty(path)) return;

            GameObject lastLevel = FindGameObjectByPath(SessionParent.gameObject, path);
            if (lastLevel != null)
            {
                // Deactivate all levels first
                foreach (var session in Sessions)
                {
                    foreach (var level in session.Levels)
                    {
                        if (level != null) level.SetActive(false);
                    }
                }
                lastLevel.SetActive(true);
                // Optionally, set reference in ButtonManager if needed
                ButtonManager bm = GameObject.FindObjectOfType<ButtonManager>();
                if (bm != null)
                {
                    bm.levelNowObjectGameplay = lastLevel;
                }
            }
        }
        public void EnsureAtLeastOneLevelActive()
        {
            if (Sessions == null || Sessions.Count == 0) return;

            bool anyLevelActive = false;
            foreach (var session in Sessions)
            {
                foreach (var level in session.Levels)
                {
                    if (level != null && level.activeSelf)
                    {
                        anyLevelActive = true;
                        break;
                    }
                }
                if (anyLevelActive) break;
            }

            if (!anyLevelActive)
            {
                foreach (var session in Sessions)
                {
                    foreach (var level in session.Levels)
                    {
                        if (level != null)
                        {
                            level.SetActive(true);
                            // Set the reference in the existing ButtonManager instance if possible
                            var bm = GameObject.FindObjectOfType<ButtonManager>();
                            if (bm != null)
                            {
                                bm.levelNowObjectGameplay = level;
                            }
                            return;
                        }
                    }
                }
            }
        }
        // Utility: Get hierarchy path for a GameObject
        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform current = obj.transform.parent;
            while (current != null && current != SessionParent)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
        }

        // Utility: Find GameObject by hierarchy path
        private GameObject FindGameObjectByPath(GameObject root, string path)
        {
            string[] parts = path.Split('/');
            Transform current = root.transform;
            foreach (string part in parts)
            {
                current = current.Find(part);
                if (current == null) return null;
            }
            return current.gameObject;
        }

        // Call this after RefreshSessions to ensure Sessions are up to date
        public void LastLevelGamePlay()
        {
            if (SessionParent == null) return;
            foreach (Transform sessionTrs in SessionParent)
            {
                Session session = new Session
                {
                    SessionObject = sessionTrs.gameObject,
                    Levels = new List<GameObject>()
                };
                foreach (Transform levelTrs in sessionTrs)
                {
                    if (levelTrs.gameObject.activeSelf)
                    {
                        ButtonManager button = new ButtonManager();
                        button.levelNowObjectGameplay = levelTrs.gameObject;
                        SaveLastActiveLevel(levelTrs.gameObject);
                    }
                }
            }
        }
        public void RefreshSessions()
        {
            if (SessionParent == null) return;
            Sessions.Clear();

            foreach (Transform sessionTransform in SessionParent)
            {
                Session newSession = new Session
                {
                    SessionObject = sessionTransform.gameObject,
                    Levels = new List<GameObject>()
                };
                foreach (Transform levelTransform in sessionTransform)
                {
                    newSession.Levels.Add(levelTransform.gameObject);
                }
                Sessions.Add(newSession);
            }
        }
    }
    private void OnApplicationFocus(bool focus) {
        gamePlay.LastLevelGamePlay();
    }
    private void OnApplicationPause(bool pause) {
        gamePlay.LastLevelGamePlay();
    }
    private void OnApplicationQuit() {
        gamePlay.LastLevelGamePlay();
    }

    [System.Serializable]
    public class Items {
        public GameObject Panel;
        public GameObject Session;

    }
    [System.Serializable]
    public class Buttons {
        public Button LocationButton;
        public Button BackFromPanelButton;
        public Button SessionButton;
        public Button SettingButton;
    }
}