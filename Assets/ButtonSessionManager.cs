using DG.Tweening;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSessionManager : MonoBehaviour {
   // [SerializeField] private Button thisButtton;
    [SerializeField] private List<Button> childrenButton;
    [SerializeField] private List<Transform> transformChildren;
    //[SerializeField] private GameObject firstObj;

    private List<Vector3> originalPositions = new List<Vector3>();
    public bool isExpanded = false;
    public bool isAnimating = false;


    public GameObject activeSessionMenu;
    public GameObject activeGameplay;
    
    private void Start() {
        LoadButtonOrder();

        foreach (var btn in childrenButton) {
            originalPositions.Add(btn.transform.position);
        }
        for (var i = 0; i < childrenButton.Count; i++) {
            childrenButton[i].gameObject.SetActive(i == 0);
        }
        childrenButton[0].GetComponent<SessionOnclickEventScript>().AddSession();

        for (int i = 0; i < childrenButton.Count; i++) {
            Button btn = childrenButton[i];
            btn.onClick.AddListener(() => OnclickButtonEvent(btn));
        }
    }
    private void SaveButtonOrder() {
        List<string> butttonNames = new List<string>();

        foreach (Button btn in childrenButton) {
            butttonNames.Add(btn.gameObject.name);
        }

        string saveString = string.Join(",", butttonNames);
        PlayerPrefs.SetString("ButtonOrder", saveString);
        PlayerPrefs.Save();
    }
    private void LoadButtonOrder() {
        string savedOrder = PlayerPrefs.GetString("ButtonOrder","");

        if (string.IsNullOrEmpty(savedOrder)) return;

        string[] orderedNames = savedOrder.Split(',');

        List<Button> reorderedButtons = new List<Button>();

        foreach(string name in orderedNames) {
            Button found = childrenButton.Find(b => b.gameObject.name == name);
            if (found != null) {
                reorderedButtons.Add(found);
            }
        }
        foreach (Button btn in childrenButton) {
            if (!reorderedButtons.Contains(btn)) {
                reorderedButtons.Add(btn);
            }
        }

        childrenButton = reorderedButtons;
    }
    private void OnApplicationPause(bool pause) {
        if(pause) {
            SaveButtonOrder();
        }
    }
    private void OnApplicationFocus(bool focus) {
        if (!focus) {
            SaveButtonOrder();
        }
    }
    private void OnApplicationQuit() {
        SaveButtonOrder();
    }
    public void ToggleChildrenMovement() {

        if (isAnimating) return;

        if (!isExpanded) {
            StartCoroutine(MoveChildrenOut());
        }
        else {
            StartCoroutine(MoveChildrenBack());
        }
    }
    private IEnumerator MoveChildrenOut() {
        isAnimating = true;

        for (int i = 0; i < childrenButton.Count && i < transformChildren.Count; i++) {
            Button btn = childrenButton[i];
            childrenButton[i].gameObject.SetActive(true);
            Transform targetPos = transformChildren[i];

            btn.transform.SetParent(targetPos.parent, false);
            btn.transform.DOMove(targetPos.position, 0.5f);
            yield return new WaitForSeconds(0.6f);
        }

        isAnimating = false;
        isExpanded = true;
    }
    public void OutMoveChildrenOut() {
        StartCoroutine(MoveChildrenOut());
    }
    public IEnumerator MoveChildrenBack() {

        isAnimating = true;

        if (childrenButton.Count < 2 || transformChildren.Count < 2) {
            isAnimating = false;
            yield break;
        }

        int lastIndex = childrenButton.Count - 1;

        Button lastBtn = childrenButton[lastIndex];
        Button secondBtn = childrenButton[1];

        Vector3 lastOriginal = originalPositions[lastIndex];
        Vector3 secondOriginal = originalPositions[1];

        // مرحله 1: حرکت دکمه آخر به نقطه دوم
        yield return lastBtn.transform.DOMove(transformChildren[1].position, 0.5f).WaitForCompletion();
        // مرحله 3: حرکت دکمه آخر از نقطه دوم به محل اصلی خودش
        yield return lastBtn.transform.DOMove(lastOriginal, 0.5f);

        // مرحله 2: حرکت دکمه دوم به محل اصلی خودش
        yield return secondBtn.transform.DOMove(secondOriginal, 0.5f);

        // سپس سایر دکمه‌ها به محل اولیه برگردن (به جز دکمه 1 و آخر)
        for (int i = childrenButton.Count - 2; i >= 0; i--) {
            if (i == 1) continue;
            Button btn = childrenButton[i];
            Vector3 originalPos = originalPositions[i];

            yield return btn.transform.DOMove(originalPos,0.5f).WaitForCompletion();
        }
        for (var i = 0; i < childrenButton.Count; i++) {
            childrenButton[i].gameObject.SetActive(i == 0);
        }

        isAnimating = false;
        isExpanded = false;
    }
    public void OnclickButtonEvent(Button clickedButton) {
        int clickedIndex = childrenButton.IndexOf(clickedButton);

        if (activeSessionMenu != null) {
            Destroy(activeSessionMenu);
        }
        if(activeGameplay != null) {
            Destroy(activeGameplay);
        }
        SessionOnclickEventScript sessionScript = clickedButton.GetComponent<SessionOnclickEventScript>();
        sessionScript.AddSession();

        if (clickedIndex == 0) {
            ToggleChildrenMovement();
            return;
        }
        List<Button> reorderedList = new List<Button>();
        reorderedList.Add(clickedButton);

        for (int i = 0; i < childrenButton.Count; i++) {
            if (childrenButton[i] != clickedButton) {
                reorderedList.Add(childrenButton[i]);
            }
        }

        childrenButton = reorderedList;

        StartCoroutine(RepositionButtons());
    }

    private IEnumerator RepositionButtons() {
        isAnimating = true;

        for (int i = 0; i < childrenButton.Count && i < transformChildren.Count; i++) {
            Button btn = childrenButton[i];
            btn.gameObject.SetActive(true);
            btn.transform.SetParent(transformChildren[i].parent, false);
            btn.transform.DOMove(transformChildren[i].position, 0.5f);
            yield return new WaitForSeconds(0.6f);
        }
        
        isAnimating = false;
    }
}
