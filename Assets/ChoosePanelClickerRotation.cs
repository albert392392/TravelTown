using UnityEngine;
using UnityEngine.UI;

public class ChoosePanelClickerRotation : MonoBehaviour
{
    private Button button;
    public Transform parentMainGameobj;
    public Transform mainScreenPanel;
    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found on the GameObject.");
            return;
        }
        button.onClick.AddListener(RotationFunc);
    }
    private void LateUpdate() {
        if(mainScreenPanel != null) {
            mainScreenPanel.rotation = Quaternion.identity;
        }
    }
    public void RotationFunc()
    {
        if (parentMainGameobj == null)
        {
            Debug.LogError("Parent GameObject is not assigned.");
            return;
        }
        Debug.Log("Clicked");

        // Use Mathf.DeltaAngle to compare rotation.y, since Quaternion.Euler may not match exactly due to floating point
        
        float yRotation = parentMainGameobj.eulerAngles.y;
        if (Mathf.Approximately(Mathf.DeltaAngle(yRotation, 0f), 0f))
        {
            parentMainGameobj.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }
        if (Mathf.Approximately(Mathf.DeltaAngle(yRotation, 180f), 0f) ||
            Mathf.Approximately(Mathf.DeltaAngle(yRotation, -180f), 0f))
        {
            parentMainGameobj.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
    }
}
