using UnityEngine;
using UnityEngine.UI;
public class LoadAllSceneScript : MonoBehaviour {
    private Button button;
    private void Start() {
        button = GetComponent<Button>();
    }
    public void LoadForGameMainScene() {
        UIManager.Instance.LoadWaveCustomer();
        GridManager.Instance.LoadTargetPositions();
    }
    public void LoadForMosqueScene() {
        ButtonManager.instance.gamePlay.EnsureAtLeastOneLevelActive();
        ButtonManager.instance.gamePlay.LoadAndActivateLastLevel();
        PlayerCharacter.Instance.LoadPositionAndScale();
        DraggableObjectCustomizer.instance.LoadPositionAndScale();
        TaskCreator.Instance.LoadTaskCount();
        TaskCreator.Instance.CreateTasks();
    }
}
