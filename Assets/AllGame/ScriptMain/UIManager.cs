using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    public Transform CustomerSetParent;
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
        if (ButtinPinSpider == null) {
            Debug.LogError("ButtinPinSpider is not assigned in UIManager.");
        }
        Instance = this;
    }

    private void Start() {
        merge_particleSystem.Stop();
        _canAddCoinPanel = true;
        initialAddCountCoinPanel = AddCountCoinPanel;
        FindCustomer();
        if (ButtinPinSpider == null) {
            Debug.LogError("ButtinPinSpider is not assigned in UIManager.");
        }
        InitializeWaveProgress();
        ButtinPinSpider.interactable = false;
        ButtinPinSpider.onClick.RemoveAllListeners();
        ButtinPinSpider.onClick.AddListener(OnSpiderButtonClick);
    }
    private void InitializeWaveProgress() {
        waveIndex = PlayerPrefs.GetInt(waveIndexSave, 0);
        for (int i = 0; i <= waveIndex; i++) {
            waveHolders[i].LoadState(i);
            waveHolders[i].SpawnInitialCustomers(CustomerSetParent.transform); // ✅ اصلاح‌شده
        }
    }


    public void SetCurrentSpider(SpiderScript spider) {
        spiderScript = spider;
        ButtinPinSpider.interactable = true;
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
            Destroy(spiderScript.gameObject);
            TextPin.text = string.Empty;
            ButtinPinSpider.interactable = false;
            spiderScript = null;
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
            CoinManager.Instance.AddCoins(AddCountCoinPanel);
        }

        if (LastMargeObject) {
            LastMargeObject.SetActive(!LastMargeObject.activeSelf);
        }

        if (IsFirstClick) {
            CoinManager.Instance.AddCoins(AddCountCoinPanel);
        }
        else {
            CoinManager.Instance.SpendCoins(AddCountCoinPanel);
        }

        IsFirstClick = !IsFirstClick;
        placeCountCoin.text = Convert.ToString(IsFirstClick ? AddCountCoinPanel : initialAddCountCoinPanel);
    }
    public void CheckWave(CustomerScript customer) {
        if (customer.EndOrders) {
            Destroy(customer.gameObject);
            waveHolders[waveIndex].OnCustomerRemoved(CustomerSetParent.transform, waveIndex, customer);

            if (waveHolders[waveIndex]._customerList.Count == 0) {
                waveIndex++;
                PlayerPrefs.SetInt(waveIndexSave, waveIndex);
                PlayerPrefs.Save();

                if (waveIndex < waveHolders.Length) {
                    waveHolders[waveIndex].SpawnInitialCustomers(CustomerSetParent.transform);
                }
            }
        }
    }

    public CustomerScript GetCustomer(CustomerScript customerScript) {
        customerScript = waveHolders[waveIndex].customersPrefabs[waveIndex].customer;
        return customerScript;
    }
    [Serializable]
    public class CustomerSaveData {
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        // سایر ویژگی‌های مورد نیاز
    }

    [Serializable]
    private class WaveHolderSaveData {
        public List<CustomerSaveData> activeCustomers;
        public List<string> removedCustomerIds;
        public int nextSpawnIndex;
    }

    [Serializable]
    private class WaveHolder {
        public CustomerHolder[] customersPrefabs;
        public List<CustomerScript> _customerList = new();
        public int customerControlInGame;
        private HashSet<string> removedIds = new();
        private Dictionary<string, CustomerHolder> idToHolder;
        public int nextSpawnIndex;
        public void InitDictionary() {
            idToHolder = customersPrefabs
              .Where(h => h != null && h.customer != null && h.data != null && !string.IsNullOrEmpty(h.id))

                .ToDictionary(h => h.id, h => h);
        }
        public void SpawnInitialCustomers(Transform spawnPoint) {
            _customerList.RemoveAll(c => c == null);

            InitDictionary();

            while (_customerList.Count < customerControlInGame && nextSpawnIndex < customersPrefabs.Length) {
                var holder = customersPrefabs[nextSpawnIndex];
                nextSpawnIndex++;

                if (holder == null || removedIds.Contains(holder.id)) continue;
                if (string.IsNullOrEmpty(holder.id) || _customerList.Any(c => c.CustomerId == holder.id)) continue;



                var customer = GameObject.Instantiate(holder.customer, spawnPoint.position, spawnPoint.rotation);
                customer.CustomerId = holder.id;
                customer.transform.SetParent(spawnPoint);
                customer.coinValue = holder.data.CoinValue;
                customer.CustomerCoinText.text = holder.data.CoinValue.ToString();

                int[] indexer = { 1, 0, 2 };
                int currentIndex = 0;
                foreach (var prefab in holder.data.CustomerObjectPrefabs) {
                    var parent = customer.papper.GetChild(indexer[currentIndex]);
                    var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                    obj.transform.localScale = Vector3.one;
                    if (obj.TryGetComponent(out RectTransform rt))
                        rt.anchoredPosition = Vector2.zero;
                    customer.CustomerObjectPrefabs.Add(obj);
                    currentIndex = (currentIndex + 1) % indexer.Length;
                }

                _customerList.Add(customer);
            }
        }
        public void OnCustomerRemoved(Transform spawnPoint, int waveIdx, CustomerScript removedCustomer) {
            string id = removedCustomer.CustomerId;

            if (!string.IsNullOrEmpty(id)) {
                removedIds.Add(id);
            }

            _customerList.Remove(removedCustomer);
            SpawnInitialCustomers(spawnPoint);
            SaveState(waveIdx);
        }

        public void SaveState(int waveIdx) {
            var data = new WaveHolderSaveData {
                activeCustomers = _customerList.Select(c => new CustomerSaveData {
                    id = c.CustomerId,
                    position = c.transform.position,
                    rotation = c.transform.rotation
                    // ذخیره سایر ویژگی‌ها
                }).ToList(),
                removedCustomerIds = removedIds.ToList(),
                nextSpawnIndex = this.nextSpawnIndex
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString($"waveholder_{waveIdx}_data", json);
        }


        public void LoadState(int waveIdx) {
            InitDictionary();
            string json = PlayerPrefs.GetString($"waveholder_{waveIdx}_data", "");
            if (string.IsNullOrEmpty(json)) return;

            var data = JsonUtility.FromJson<WaveHolderSaveData>(json);
            nextSpawnIndex = data.nextSpawnIndex;
            removedIds = new HashSet<string>(data.removedCustomerIds);
            _customerList.Clear();

            foreach (var customerData in data.activeCustomers) {
                if (idToHolder.TryGetValue(customerData.id, out var holder)) {
                    var customer = GameObject.Instantiate(holder.customer, customerData.position, customerData.rotation, UIManager.Instance.CustomerSetParent);
                    customer.transform.localScale = Vector3.one; // Ensure correct scale
                    customer.CustomerId = holder.id;
                    customer.coinValue = holder.data.CoinValue;
                    customer.CustomerCoinText.text = holder.data.CoinValue.ToString();
                    // بازیابی سایر ویژگی‌ها
                    _customerList.Add(customer);

                    int[] indexer = { 1, 0, 2 };
                    int currentIndex = 0;
                    foreach (var prefab in holder.data.CustomerObjectPrefabs) {
                        var parent = customer.papper.GetChild(indexer[currentIndex]);
                        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
                        obj.transform.localScale = Vector3.one;
                        if (obj.TryGetComponent(out RectTransform rt))
                            rt.anchoredPosition = Vector2.zero;
                        customer.CustomerObjectPrefabs.Add(obj);
                        currentIndex = (currentIndex + 1) % indexer.Length;
                    }
                }

            }

        }


    }
    [Serializable]
    private class CustomerHolder {
        public string id; // شناسه یکتا
        public CustomerScript customer;
        public CustomerData data;
    }


    [Serializable]
    public class CustomerData {
        public List<GameObject> CustomerObjectPrefabs;
        public int CoinValue;
    }
}