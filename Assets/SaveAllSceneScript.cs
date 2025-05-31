using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.UI;

public class SaveAllSceneScript : MonoBehaviour {
    private Button button;
    private void Start() {
        button = GetComponent<Button>();

    }
    public void SaveMainGameScene() {
        //UIManager.Instance.SaveWaveCustomer();
        if (!inventorySaveLoadScript.Instance.hasSaved) {
            inventorySaveLoadScript.Instance.SaveInventory();
            inventorySaveLoadScript.Instance.hasSaved = true;
        }
        GridManager.Instance.SaveTargetPositions();
    }
    public void SaveMosqueScene() {
        TaskCreator.Instance.SaveTaskCount();
        PlayerCharacter.Instance.SavePositionAndScale();
        DraggableObjectCustomizer.instance.SavePositionAndScale();
        ButtonManager.instance.gamePlay.LastLevelGamePlay();
        SimpleScrollSnap.instance.SaveAndLoadNearestPanel();
    }
}
