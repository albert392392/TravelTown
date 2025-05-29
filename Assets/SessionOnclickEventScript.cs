using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionOnclickEventScript : MonoBehaviour
{
    [SerializeField] private GameObject sourceSessionMenu;
    [SerializeField] private GameObject sourceGamePlay;
    [SerializeField] private Transform targetSession;
    [SerializeField] private Transform targetGamePlay;
    
    private Button thisButton;

    public static GameObject activeSessionMenu;
    public static GameObject activeGamePlay;

    [SerializeField] private List<GameObjectStateManager> gameObjectStateManagers = new List<GameObjectStateManager>();
    private void Start() {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(() => AddSession());
    }
    public void AddSession() {


        if (targetGamePlay.childCount == 0 || targetGamePlay.GetChild(0).name.Replace("(Clone)", "") != sourceGamePlay.name) {
            if (activeSessionMenu != null) {
                Destroy(activeSessionMenu);
            }
            if (activeGamePlay != null) {
                Destroy(activeGamePlay);
            }

            activeSessionMenu = Instantiate(sourceSessionMenu, targetSession);

            UiManagerMosque.instance.snap = sourceSessionMenu.GetComponent<SimpleScrollSnap>(); 
            UiManagerMosque.instance.snap.LoadAndLoadNearstPanel();

            activeGamePlay = Instantiate(sourceGamePlay, targetGamePlay);
            gameObjectStateManagers.Clear();

            AddStateManagersRecursive(activeGamePlay.transform);

            foreach (var stateManager in gameObjectStateManagers) {
                stateManager.LoadState();
            }
        }
    }
    public void AddStateManagersRecursive(Transform parentTransform) {
        foreach (Transform child in parentTransform) {
            GameObjectStateManager stateManager = child.GetComponent<GameObjectStateManager>();
            if (stateManager != null) {
                gameObjectStateManagers.Add(stateManager);
            }
            AddStateManagersRecursive(child);
        }
    }
}
