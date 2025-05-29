using TMPro;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour {
    public TextMeshProUGUI energyText;

    private void Update() {
        if (EnergyManager.Instance != null) {
            UpdateEnergyText();
        }
    }
    private void UpdateEnergyText() {
        if (energyText != null) {
            energyText.text = EnergyManager.Instance.totalEnergys.ToString();
            EnergyManager.Instance.SaveEnergys();
        }
    }
}
