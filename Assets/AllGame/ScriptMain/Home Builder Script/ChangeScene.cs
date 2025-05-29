using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    private Button button;
    [SerializeField] private HelpMergeManager helpMergeManager;
    private void Start() {
        button = GetComponent<Button>();
    }
    private void Update()
    {
        // Only call UpdateGameMain if the active scene is "GameScene"
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "GameScene")
        {
            UpdateGameMain();
        }
    }
    public void GameMainScene() {
        SceneManager.LoadScene("Mosque");
    }
    private void UpdateGameMain() {
        if (helpMergeManager != null && !helpMergeManager.isAnimating) {
            button.enabled = true;
        }
        else {
            button.enabled = false;
        }
    }
    public void MosqueScene() {
        SceneManager.LoadScene("GameScene");
        //SceneManager.LoadScene(SceneManager.GetSceneByName("GameScene").buildIndex);
    }
}
