
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    public int AddCountCoinPanel { get; set; }
    public bool _canAddCoinPanel { get; set; }
    public GameObject LastMargeObject { get; set; }
    private bool IsFirstClick = true;

    public GridManager gridManager;
    [HideInInspector] public List<Sprite> AllMergeSprites;
    [SerializeField] private WaveHolder[] waveHolders;

    [SerializeField] private GameObject CustomerSetParent;
    public ParticleSystem merge_particleSystem;
    public Text placeTextLevel;
    public Text placePanelText;
    public TextMeshProUGUI placeCountCoin;
    public Button placeButtonPanel;
    public Image PlaceSpritePanel;
    [SerializeField] private int initialAddCountCoinPanel;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI TextPin;
    public Button ButtonPin;
    public Button ButtinPinSpider;
    public GameObject selector;
    public GameObject ScatterObjectTimerClock;
    public GameObject MoneyOnGrid;
    private int waveIndex;
    private SpiderScript spiderScript;
    [SerializeField] private string nextCustomerIndexSave;
    [SerializeField] private string waveIndexSave;
    public GameObject chooseOver;
    private void Awake() {
        // بررسی اینکه دکمه به درستی تنظیم شده باشد
        if (ButtinPinSpider == null) {
            Debug.LogError("ButtinPinSpider is not assigned in UIManager.");
        }
        Instance = this;
    }
    private void Start() {
        InitializeWaveProgress();
        merge_particleSystem.Stop();
        _canAddCoinPanel = true;
        initialAddCountCoinPanel = AddCountCoinPanel;
        FindCustomer();
        if (ButtinPinSpider == null) {
            Debug.LogError("ButtinPinSpider is not assigned in UIManager.");
        }
        if (ButtinPinSpider == null) {
            Debug.LogError("ButtinPinSpider is not assigned.");
        }
        ButtinPinSpider.interactable = false; // ابتدا غیرفعال
        ButtinPinSpider.onClick.RemoveAllListeners();
        ButtinPinSpider.onClick.AddListener(OnSpiderButtonClick);
    }
    private void InitializeWaveProgress() {
        LoadWaveCustomer();
        for (int i = 0; i <= waveIndex; i++) {
            waveHolders[i].Spawn(CustomerSetParent.transform);
        }
    }
    public void SetCurrentSpider(SpiderScript spider) {
        spiderScript = spider;
        ButtinPinSpider.interactable = true; // فعال کردن دکمه
        TextPin.text = $"Spider Info: Name: {spider.name}, Diamond Price: {spider.DiamondCount}";
    }

    public void OnSpiderButtonClick() {
        if (spiderScript == null) {
            Debug.LogWarning("No spider assigned to the button.");
            return;
        }

        if (DiamondManager.Instance.GetDiamondCount() >= spiderScript.DiamondCount) {
            DiamondManager.Instance.SpendDiamonds(spiderScript.DiamondCount);
            Debug.Log($"DiamondCount decreased by {spiderScript.DiamondCount}");
            Destroy(spiderScript.gameObject); // نابود کردن آبجکت
            TextPin.text = string.Empty;
            ButtinPinSpider.interactable = false; // غیرفعال کردن دکمه
            spiderScript = null; // پاک کردن مرجع
        }
        else {
            Debug.LogWarning("Not enough diamonds.");
        }
    }
    public void OnClickPinButton() {
        ScatterObjectsWithInterval scatterObjectsWithInterval = FindObjectOfType<ScatterObjectsWithInterval>();
        if (DiamondManager.Instance.GetDiamondCount() >= scatterObjectsWithInterval.DiamondNeedForClock) {
            DiamondManager.Instance.SpendDiamonds(scatterObjectsWithInterval.DiamondNeedForClock);
            TextPin.text = "";
            ButtonPin.interactable = false;
            scatterObjectsWithInterval.timerRemaining = 0;
            scatterObjectsWithInterval.spawnedObjectCount = 0;
            scatterObjectsWithInterval.scatterObjectTimerClock.SetActive(false);
        }
    }
    private void FindCustomer() {
        placeButtonPanel.onClick.AddListener(OnClickPanelButton);
        for (int i = 0; i < waveHolders[waveIndex]._customerList.Count; i++) {
            var customer = waveHolders[waveIndex]._customerList[i];
        }
    }

    public void OnClickPanelButton() {
        for (int i = 0; i < AllMergeSprites.Count; i++) {
            if (AllMergeSprites[i] == PlaceSpritePanel.sprite) {
                _canAddCoinPanel = false;
                break;
            }
            else {
                _canAddCoinPanel = true;
            }
        }

        AllMergeSprites.Add(PlaceSpritePanel.sprite);
        if (_canAddCoinPanel) {
            CoinManager.Instance.AddCoins(AddCountCoinPanel); // افزودن به سکه‌ها
        }

        if (LastMargeObject) {
            LastMargeObject.SetActive(!LastMargeObject.activeSelf);
        }

        if (IsFirstClick) {
            CoinManager.Instance.AddCoins(AddCountCoinPanel); // افزودن سکه در کلیک اول
        }
        else {
            CoinManager.Instance.SpendCoins(AddCountCoinPanel); // کسر سکه در کلیک دوم
        }

        IsFirstClick = !IsFirstClick;
        placeCountCoin.text = Convert.ToString(IsFirstClick ? AddCountCoinPanel : initialAddCountCoinPanel);
    }

    public void CheckWave(CustomerScript customer) {

        if (customer.EndOrders) {
            var parent = customer.transform.parent;
            Destroy(customer.gameObject);
            waveHolders[waveIndex]._customerList.Remove(customer);
            waveHolders[waveIndex].OnCustomerRemoved(CustomerSetParent.transform);
            // If all customers are done and no more to spawn, go to next wave
            if (waveHolders[waveIndex]._customerList.Count == 0 &&
                waveHolders[waveIndex].customerControlInGame + waveHolders[waveIndex].nextCustomerIndex >= waveHolders[waveIndex].customersPrefabs.Length) {
                waveIndex++;
                if (waveIndex < waveHolders.Length)
                    waveHolders[waveIndex].Spawn(CustomerSetParent.transform);
            }
            SaveWaveCustomer();
        }
    }

    public CustomerScript GetCustomer(CustomerScript customerScript) {
        customerScript = waveHolders[waveIndex].customersPrefabs[waveIndex].customer;
        return customerScript;
    }

    [Serializable]
    private class WaveHolder {
        public CustomerHolder[] customersPrefabs;
        public List<CustomerScript> _customerList = new();
        public int customerControlInGame;
        public int nextCustomerIndex;

        public void Spawn(Transform spawnPoint) {

            // Ensure only customerControlInGame customers are present
            while (_customerList.Count < customerControlInGame && nextCustomerIndex < customersPrefabs.Length) {
                var holder = customersPrefabs[nextCustomerIndex];
                var customer = GameObject.Instantiate(holder.customer, spawnPoint.position, spawnPoint.rotation);
                customer.transform.SetParent(spawnPoint);

                customer.coinValue = holder.data.CoinValue;
                customer.CustomerCoinText.text = holder.data.CoinValue.ToString();

                if (holder.data != null) {
                    int[] indexer = { 1, 0, 2 };
                    int currentIndex = 0;

                    foreach (var prefab in holder.data.CustomerObjectPrefabs) {
                        var parent = customer.papper.GetChild(indexer[currentIndex]);
                        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                        obj.name = prefab.name;
                        obj.transform.localScale = Vector3.one;

                        var rt = obj.GetComponent<RectTransform>();
                        if (rt != null) rt.anchoredPosition = Vector2.zero;

                        customer.CustomerObjectPrefabs.Add(obj);
                        currentIndex = (currentIndex + 1) % indexer.Length;
                    }
                }

                _customerList.Add(customer);
                nextCustomerIndex++;
            }
        }

        public void OnCustomerRemoved(Transform spawnPoint) {
            // Remove nulls or destroyed customers
            _customerList.RemoveAll(c => c == null);
            // Spawn new customer if needed
            if (_customerList.Count < customerControlInGame && nextCustomerIndex < customersPrefabs.Length) {
                Spawn(spawnPoint);
            }
        }

        /*
        public void DestoyAllCustomer()
        {
            foreach (var customer in _customerList)
            {
                if (customer != null)
                    GameObject.Destroy(customer.gameObject);
            }
            _customerList.Clear();
        }*/
    }
    public void SaveWaveCustomer() {
        for (int i = 0; i < waveHolders.Length; i++) {
            PlayerPrefs.SetInt($"{nextCustomerIndexSave}_{i}", waveHolders[i].nextCustomerIndex);

            // ذخیره ID مشتری‌های فعلی
            List<int> activeCustomerIndices = new List<int>();
            foreach (var customer in waveHolders[i]._customerList) {
                int index = Array.IndexOf(waveHolders[i].customersPrefabs, waveHolders[i].customersPrefabs.FirstOrDefault(h => h.customer.name == customer.name.Replace("(Clone)", "").Trim()));
                if (index >= 0) {
                    activeCustomerIndices.Add(index);
                }
            }

            string json = JsonUtility.ToJson(new IntListWrapper { list = activeCustomerIndices });
            PlayerPrefs.SetString($"active_customers_{i}", json);
        }
        PlayerPrefs.SetInt(waveIndexSave, waveIndex);
        PlayerPrefs.Save();
    }
    [Serializable]
    private class IntListWrapper {
        public List<int> list;
    }


    public void LoadWaveCustomer() {
        for (int i = 0; i < waveHolders.Length; i++) {
            waveHolders[i].nextCustomerIndex = PlayerPrefs.GetInt($"{nextCustomerIndexSave}_{i}", waveHolders[i].nextCustomerIndex);
        }
        waveIndex = PlayerPrefs.GetInt(waveIndexSave, waveIndex);

        for (int i = 0; i < waveHolders.Length; i++) {
            string json = PlayerPrefs.GetString($"active_customers_{i}", "");
            if (!string.IsNullOrEmpty(json)) {
                IntListWrapper wrapper = JsonUtility.FromJson<IntListWrapper>(json);
                foreach (int customerIndex in wrapper.list) {
                    if (customerIndex >= 0 && customerIndex < waveHolders[i].customersPrefabs.Length) {
                        var holder = waveHolders[i].customersPrefabs[customerIndex];
                        var customer = GameObject.Instantiate(holder.customer, CustomerSetParent.transform.position, Quaternion.identity, CustomerSetParent.transform);
                        customer.transform.localScale = Vector3.one;
                        customer.coinValue = holder.data.CoinValue;
                        customer.CustomerCoinText.text = holder.data.CoinValue.ToString();

                        if (holder.data != null) {
                            int[] indexer = { 1, 0, 2 };
                            int currentIndex = 0;

                            foreach (var prefab in holder.data.CustomerObjectPrefabs) {
                                var parent = customer.papper.GetChild(indexer[currentIndex]);
                                var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                                obj.name = prefab.name;
                                obj.transform.localScale = Vector3.one;

                                var rt = obj.GetComponent<RectTransform>();
                                if (rt != null) rt.anchoredPosition = Vector2.zero;

                                customer.CustomerObjectPrefabs.Add(obj);
                                currentIndex = (currentIndex + 1) % indexer.Length;
                            }
                        }

                        waveHolders[i]._customerList.Add(customer);
                    }
                }
            }
        }
    }

    [Serializable]
    private class CustomerHolder {
        public CustomerScript customer;
        public CustomerData data;
    }

    [Serializable]
    public class CustomerData {
        public List<GameObject> CustomerObjectPrefabs;
        public int CoinValue;
    }
}