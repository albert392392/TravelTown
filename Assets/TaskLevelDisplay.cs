using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskLevelDisplay : MonoBehaviour {
    public string TaskKey; // Key for this display
    public Image totalLevelImage; // Progress bar
    public TextMeshProUGUI fillPercentageText; // Percentage display
    public float currentValue;
    private void Start() {
        if (TaskLevelManager.Instance != null) {
            // Initialize with current value from TaskLevelManager
            currentValue = TaskLevelManager.Instance.GetTaskLevel(TaskKey);
            UpdateDisplay(currentValue);
        }
    }

    private void Update() {
        // Continuously update the display to match TaskLevels
        if (TaskLevelManager.Instance != null) {
            currentValue = TaskLevelManager.Instance.GetTaskLevel(TaskKey);
            UpdateDisplay(currentValue);
        }
    }

    public void UpdateDisplay(float totalLevelsFill) {
        if (totalLevelImage != null) {
            totalLevelImage.fillAmount = totalLevelsFill / 100f; // Scale to [0, 1]
        }
        if (fillPercentageText != null) {
            fillPercentageText.text = $"{Mathf.FloorToInt(totalLevelsFill)}%"; // Display percentage
        }
    }
}
