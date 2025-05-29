using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonManagerSmall : MonoBehaviour {
    public void OnLevelButtonClickBack(GameObject parent) {
        parent.SetActive(false);
    }
    public void OnLevelButtonClickOpen(GameObject child) {
        child.SetActive(true);
    }
}
