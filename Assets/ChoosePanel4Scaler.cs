using UnityEngine;
using UnityEngine.UI;

public class ChoosePanel4Scaler : MonoBehaviour
{
    public Button scalePlus;
    public Button scaleMinus;
    public DraggableObjectCustomizer draggableObjectCustomizer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scalePlus.onClick.AddListener(() => Plus());
        scaleMinus.onClick.AddListener( () => Minus());
    }
    public void Plus() {
        draggableObjectCustomizer.inCreaseScaler = 1;
        draggableObjectCustomizer.IncreaseScale(draggableObjectCustomizer.inCreaseScaler);
    }
    public void Minus() {
        draggableObjectCustomizer.deCreaseScaler = 1;
        draggableObjectCustomizer.DecreaseScale(draggableObjectCustomizer.deCreaseScaler);
    }
}
