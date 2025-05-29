using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DanielLochner.Assets.SimpleScrollSnap;

public class ButtonIndex : MonoBehaviour {
    public static ButtonIndex Instance { get; private set; }

    public int SessionIndex;
    public int LevelIndex;
    public Button button;
    private ButtonSessionManager buttonSessionManager;
    private void Start() {
        button = GetComponent<Button>();
        buttonSessionManager = FindObjectOfType<ButtonSessionManager>();
    }
    public void OnButtonClick() {
        // Find the ButtonManager in the scene
        ButtonManager buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager != null) {
            // Call the OnLevelButtonClick method with the SessionIndex and LevelIndex
            buttonManager.OnLevelButtonClick(SessionIndex, LevelIndex);
        }
        else {
            Debug.LogError("ButtonManager not found in the scene.");
        }
    }
    
    public void OnLevelButtonClickBackLowSmall() {
        StartCoroutine(BackSequence());
    }
    private IEnumerator BackSequence() {
        if (buttonSessionManager.isExpanded) {
            yield return StartCoroutine(buttonSessionManager.MoveChildrenBack());
        }
        if (!buttonSessionManager.isExpanded) {
            OnButtonClick();
        }
    }
}
