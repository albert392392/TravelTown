using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.UI;
using static ButtonManager;
public class LoadAllSceneScript : MonoBehaviour {
    private Button button;
    private void Start() {
        button = GetComponent<Button>();
    }
    public void LoadForGameMainScene() {
       //IManager.Instance.LoadWaveCustomer();
        GridManager.Instance.LoadTargetPositions();
        inventorySaveLoadScript.Instance.LoadInventory();
    }
    public void LoadForMosqueScene() {
        if (ButtonManager.instance.gamePlay != null) {
            ButtonManager.instance.gamePlay.RefreshSessions();
            ButtonManager.instance.gamePlay.EnsureAtLeastOneLevelActive();
            ButtonManager.instance.gamePlay.LoadAndActivateLastLevel();
        }
        PlayerCharacter.Instance.LoadPositionAndScale();
        DraggableObjectCustomizer.instance.LoadPositionAndScale();
        TaskCreator.Instance.LoadTaskCount();
        TaskCreator.Instance.CreateTasks();
        SimpleScrollSnap.instance.LoadAndLoadNearstPanel();
    }
}
