using UnityEngine;
using UnityEngine.UI;

public class ButtonDeleteObject : MonoBehaviour
{
    public static ButtonDeleteObject Instance { get; private set; }
    public MergeableBase objectDelete;
    private Button button;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        objectDelete = null;
        button = GetComponent<Button>();
        button.onClick.AddListener(DestroyerObject);
    }

    public void DestroyerObject()
    {
        if (objectDelete == null)
            return;

        // Only delete if all conditions are true
        if (objectDelete.box == null &&
            objectDelete.SpiderWeb == null &&
            objectDelete.CurrentState == MergeableState.Idle)
        {
            UIManager.Instance.chooseOver.SetActive(false);
            Destroy(objectDelete.gameObject);
            objectDelete = null;
        }
    }
}
