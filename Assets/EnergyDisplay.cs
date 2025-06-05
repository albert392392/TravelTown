using System.Collections;
using TMPro;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour {
    public static EnergyDisplay Instance { get; private set; }

    public TextMeshProUGUI energyText;
    /*
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // حفظ آبجکت هنگام تغییر سین
        }
        else {
            Destroy(gameObject);
        }
    }*/
    private void Start() {
        if (EnergyManager.Instance != null && EnergyManager.Instance.totalEnergys != int.Parse(energyText.text)) {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    private void Update() {
        /*  if (EnergyManager.Instance != null) {
              UpdateEnergyText();
          }*/
        if (EnergyManager.Instance != null && EnergyManager.Instance.totalEnergys != int.Parse(energyText.text)) {
            StartCoroutine(NumberAnimationCounter());
        }
    }
    /*
    private void UpdateEnergyText() {
        if (energyText != null) {
            energyText.text = EnergyManager.Instance.totalEnergys.ToString();
            EnergyManager.Instance.SaveEnergys();
        }
    }*/
    public IEnumerator NumberAnimationCounter() {
        int currentCoins = int.Parse(energyText.text);
        int targetCoins = EnergyManager.Instance.totalEnergys;

        // Animate the coin count increment or decrement
        while (currentCoins != targetCoins) {
            if (currentCoins < targetCoins) {
                currentCoins += Mathf.CeilToInt((targetCoins - currentCoins) * 0.1f); // Increase speed by incrementing faster
                if (currentCoins > targetCoins) currentCoins = targetCoins; // Ensure it doesn't overshoot
            }
            else {
                currentCoins -= Mathf.CeilToInt((currentCoins - targetCoins) * 0.1f); // Decrease speed by decrementing faster
                if (currentCoins < targetCoins) currentCoins = targetCoins; // Ensure it doesn't undershoot
            }

            energyText.text = currentCoins.ToString();
            yield return new WaitForSeconds(0.02f); // Adjusted for faster animation
        }
    }
}
