using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuClickButton : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnCorrect;
    public Button btnNotCorrect;

    [Header("Target Scripts")]
    public AddedAndChangeScriptMenu addedAndChangeScriptMenu;
    public AddedAndChangeScriptMenu2 addedAndChangeScriptMenu2;
    private void Start() {
        RegisterButtonListeners();
    }
    private void RegisterButtonListeners() {
        if (btnCorrect != null)
            btnCorrect.onClick.AddListener(OnCorrectClick);

        if (btnNotCorrect != null)
            btnNotCorrect.onClick.AddListener(OnNotCorrectClick);

    }
    public void OnCorrectClick() {
        addedAndChangeScriptMenu2?.OnCorrectClickBtn();
        addedAndChangeScriptMenu?.OnCorrectClickBtn();
    }

    public void OnNotCorrectClick() {
        addedAndChangeScriptMenu2?.OnNotCorrectClickBtn();
        addedAndChangeScriptMenu?.OnNotCorrectClickBtn();
    }

}
