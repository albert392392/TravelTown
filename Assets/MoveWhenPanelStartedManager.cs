using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWhenPanelStartedManager : MonoBehaviour
{

    [SerializeField] private GameObject scenebutton;
    [SerializeField] private GameObject barHolder;
    [SerializeField] private GameObject settingbutton;
    public GameObject panelChooseItem;
    public GameObject ScrollBar;
    public bool isStartMove;
    public bool isMoveToFirst;
    private Vector3 rectTransformScenebutton;
    private Vector3 rectTransformBarHolder;
    private Vector3 rectTransformSettingbutton;
    private Vector3 rectTransformPanel;
    private void Start() {
        isStartMove = false;
        isMoveToFirst = false;
        panelChooseItem.SetActive(false);
        rectTransformScenebutton = scenebutton.transform.position;
        rectTransformBarHolder = barHolder.transform.position;
        rectTransformSettingbutton = settingbutton.transform.position;
        rectTransformPanel = panelChooseItem.transform.position;
    }
    private void Update() {

        if (isStartMove == true) {
            StartCoroutine(funcMove());
        }
        if (isMoveToFirst == true) {
            StartCoroutine(funcBack());
        }
    }
    private IEnumerator funcMove() {

        barHolder.transform.DOMoveY(barHolder.transform.position.y+110f,1f);
        settingbutton.transform.DOMoveY(settingbutton.transform.position.y + 110f, 1f);
        scenebutton.transform.DOMoveX(scenebutton.transform.position.x + 140f, 1f);
        panelChooseItem.gameObject.SetActive(true);
        ScrollBar.SetActive(true);
        panelChooseItem.transform.GetChild(0).gameObject.SetActive(true);
        panelChooseItem.transform.DOMoveY(panelChooseItem.transform.position.y+28,1f);
        yield return null;
        isStartMove = false;
    }
    private IEnumerator funcBack() {

        barHolder.transform.DOMoveY(rectTransformBarHolder.y, 1f);
        settingbutton.transform.DOMoveY(rectTransformSettingbutton.y, 1f);
        scenebutton.transform.DOMoveX(rectTransformScenebutton.x, 1f);
        ClearRequestPanelObjs();
        panelChooseItem.gameObject.SetActive(false);
        panelChooseItem.transform.DOMoveY(rectTransformPanel.y, 1f);
        yield return null;
        isMoveToFirst = false;
    }

    private void ClearRequestPanelObjs() {
        for (int i = 0; i < ScrollBar.transform.childCount; i++) {
            Destroy(ScrollBar.transform.GetChild(i).gameObject);
        }
    }
}
