using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuAdderClick : MonoBehaviour {
    public Button btnCorrect;
    public Button btnNotCorrect;
    public Button btnItems;
    public AddedAndChangeScriptMenu addedAndChangeScriptMenu; // این متغیر باید از طریق Inspector تنظیم شود.
    public AddedAndChangeScriptMenu2 AddedAndChangeScriptMenu2;
    // Start method برای تنظیم listeners
    private void Start() {
        if (btnCorrect != null) {
            btnCorrect.onClick.AddListener(() => OnCorrectClickBtn());
        }
        if (btnNotCorrect != null) {
            btnNotCorrect.onClick.AddListener(() => OnNotCorrectClickBtn());
        }
        if (btnItems != null) {
            btnItems.onClick.AddListener(() => OnclickObjectsMenu());
        }
    }

    // تابع OnCorrectClickBtn
    public void OnCorrectClickBtn() {
        // اجرای کد مورد نظر در اینجا
        if (addedAndChangeScriptMenu != null) {
            addedAndChangeScriptMenu.OnCorrectClickBtn(btnCorrect);
        }
        if(AddedAndChangeScriptMenu2 != null) {
            AddedAndChangeScriptMenu2.OnCorrectClickBtn(btnCorrect);
        }
    }

    // تابع OnNotCorrectClickBtn
    public void OnNotCorrectClickBtn() {
        // اجرای کد مورد نظر در اینجا
        if (addedAndChangeScriptMenu != null) {
            addedAndChangeScriptMenu.OnNotCorrectClickBtn(btnNotCorrect);
        }
        if (AddedAndChangeScriptMenu2 != null) {
            AddedAndChangeScriptMenu2.OnNotCorrectClickBtn(btnNotCorrect);
        }
    }

    // تابع OnclickObjectsMenu
    public void OnclickObjectsMenu() {
        // اجرای کد مورد نظر در اینجا
        if (addedAndChangeScriptMenu != null) {
            addedAndChangeScriptMenu.OnclickObjectsMenu(btnItems);
        }
        if (AddedAndChangeScriptMenu2 != null) {
            AddedAndChangeScriptMenu2.OnclickObjectsMenu(btnItems);
        }
    }
}
