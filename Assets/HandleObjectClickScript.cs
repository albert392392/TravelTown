using UnityEngine;
using UnityEngine.UI;

public class HandleObjectClickScript : MonoBehaviour
{
    private Button button;
    public bool isClickbtn = false;
    public AddedAndChangeScriptMenu addedAndChangeScriptMenuObj = null;
    void Start()
    {
        button = GetComponent<Button>();
    }
    void Update()
    {
        HandleClickFunc();
    }
    public void HandleClickFunc() {
        if (addedAndChangeScriptMenuObj != null) {
            if (!isClickbtn) {
                button.onClick.AddListener(() => addedAndChangeScriptMenuObj.HandleObjectClick());
                isClickbtn = true;
            }
        }
    }
}
