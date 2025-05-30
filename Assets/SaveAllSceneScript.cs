using UnityEngine;
using UnityEngine.UI;

public class SaveAllSceneScript : MonoBehaviour {
    private Button button;
    private void Start() {
        button = GetComponent<Button>();

    }
    public void SaveMainGameScene() {
        //UIManager.Instance.SaveWaveCustomer();
        GridManager.Instance.SaveTargetPositions();
    }
    public void SaveMosqueScene() {
        TaskCreator.Instance.SaveTaskCount();
        PlayerCharacter.Instance.SavePositionAndScale();
        DraggableObjectCustomizer.instance.SavePositionAndScale();
        ButtonManager.instance.gamePlay.LastLevelGamePlay();
    }
}
