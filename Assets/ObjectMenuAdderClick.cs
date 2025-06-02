
using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuAdderClick : MonoBehaviour {
    public Button btnItems;

    [Header("Target Scripts")]
    public AddedAndChangeScriptMenu addedAndChangeScriptMenu;
    public AddedAndChangeScriptMenu2 addedAndChangeScriptMenu2;

    private void Start() {
        RegisterButtonListeners();
    }

    private void RegisterButtonListeners() {
        if (btnItems != null)
            btnItems.onClick.AddListener(OnItemsClick);
    }
    public void OnItemsClick() {
        addedAndChangeScriptMenu?.OnclickObjectsMenu(btnItems);
        addedAndChangeScriptMenu2?.OnclickObjectsMenu(btnItems);
    }
}
