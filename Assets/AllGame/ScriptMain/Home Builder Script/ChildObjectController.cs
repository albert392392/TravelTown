using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChildObjectController : MonoBehaviour
{
    [System.Serializable]
    public class ChildObject
    {
        public int number;
        public GameObject prefabObj;
    }

    public List<ChildObject> childObjects = new List<ChildObject>();
    public Button actionButton;
    public Text numberText;
    private int currentObjectIndex = 0;
    public CoinDisplay coinDisplay;
    public string actionButtonKey = "CoinParentActionKey";
    private void Start()
    {
        LoadChildObjectsState();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnButtonClick);

        UpdateNumberText();
    }

    private void OnButtonClick()
    {
        if (currentObjectIndex >= childObjects.Count)
        {
            return;
        }

        var currentChild = childObjects[currentObjectIndex];
        /*
        if (coinDisplay.GetCoinCount() >= currentChild.number)
        {
 
            coinDisplay.RemoveCoins(currentChild.number);

            if (currentChild.prefabObj != null)
            {
                currentChild.prefabObj.SetActive(true);
            }

            currentObjectIndex++;

            UpdateNumberText();

            SaveChildObjectsState();
        }*/
    }

    private void UpdateNumberText()
    {
        if (currentObjectIndex < childObjects.Count)
        {
            numberText.text = "" + childObjects[currentObjectIndex].number;
        }
        else
        {
            numberText.text = "All objects activated!";
        }
    }

    public void LoadChildObjectsState()
    {
        currentObjectIndex = PlayerPrefs.GetInt("CurrentObjectIndex", 0);

        for (int i = 0; i < childObjects.Count; i++)
        {
            bool isActive = PlayerPrefs.GetInt("PrefabObjActive_" + i, 0) == 1;
            if (childObjects[i].prefabObj != null)
            {
                childObjects[i].prefabObj.SetActive(isActive);
            }
        }
    }
    public void OnActionButtonClick()
    {
        PlayerPrefs.SetInt(actionButtonKey,1);
        PlayerPrefs.Save();
    }
    public void SaveChildObjectsState()
    {
        PlayerPrefs.SetInt("CurrentObjectIndex", currentObjectIndex);

        for (int i = 0; i < childObjects.Count; i++)
        {
            bool isActive = childObjects[i].prefabObj != null && childObjects[i].prefabObj.activeSelf;
            PlayerPrefs.SetInt("PrefabObjActive_" + i, isActive ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    public void OnApplicationQuit()
    {
        SaveChildObjectsState();
    }
}
