using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;

public class CustomerScript : MonoBehaviour
{
    public Button CustomerButton;
    public TextMeshProUGUI CustomerCoinText;
    public int coinValue; // Amount added to coins
    private GridManager gridManager;
    private List<MergeableBase> cachedObjectMerges = new List<MergeableBase>();
    public List<GameObject> CustomerObjectPrefabs;
    public List<MergeableBase> objectMerged = new List<MergeableBase>();
    public Transform papper;
    public bool EndOrders { get; private set; }
    private bool matchFound;

    private void Start()
    {
        // Initialize cachedObjectMerges with all MergeableBase objects in the scene
        cachedObjectMerges.AddRange(FindObjectsOfType<MergeableBase>());
        CustomerButton.interactable = false;
        CustomerButton.onClick.AddListener(OnCustomerButtonClick);
    }

    private void Update()
    {
        // Ensure GridManager is initialized
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("GridManager is not found!");
                return;
            }
        }

        // Update cached object merges
        UpdateCachedObjectMerges();

        if (GridManager.Instance != null) {
            GridManager.Instance.UpdateCustomerMatchTileColors();
        }
        /*
        // Ensure that the color of the merged objects persists
        foreach (var mergeObj in cachedObjectMerges)
        {
            if (mergeObj != null)
            {
                var parentTile = mergeObj.transform.parent?.parent?.gameObject;
                MergeableBase mergeable = mergeObj.GetComponent<MergeableBase>();
                if (parentTile != null && objectMerged.Contains(mergeObj.gameObject) && !mergeable.isDragging && !mergeable.isMovingToTarget && !mergeable.isSpawning)
                {
                    // Set the tile color to green permanently
                    GridManager.Instance.ChangeTileColor(parentTile, Color.green);
                }
            }
        }*/

        CustomerButton.interactable = objectMerged.Count == CustomerObjectPrefabs.Count;
    }

    private bool TryMatchWithObjects(GameObject customerPrefab)
    {
        var img = customerPrefab.GetComponent<Image>();
        if (img == null) return false;

        var sprite = img.sprite;
        var color = img.color;
        var textCustomer = customerPrefab.transform.GetComponentInChildren<TextMeshProUGUI>()?.text;
        int customerNum = ExtractNumberFromText(textCustomer);
        bool foundMatch = false;

        // Reset parent color before matching
        var parentImg = customerPrefab.transform.parent.GetComponent<Image>();
        if (parentImg != null)
        {
            parentImg.color = Color.clear;
        }

        // Only allow one match per customerPrefab
        foreach (Transform target in GridManager.Instance.targetPositions)
        {
            foreach (Transform child in target)
            {
                if (GridManager.Instance.IsBox(child.gameObject) || GridManager.Instance.IsSpiderWeb(child.gameObject))
                    continue;

                if (child.TryGetComponent<MergeableBase>(out MergeableBase targetMerge))
                {
                    if (objectMerged.Contains(targetMerge))
                        continue; // Already matched to another customerPrefab

                    var sr = child.GetComponent<SpriteRenderer>();
                    var textTarget = targetMerge.transform.GetComponentInChildren<TextMeshPro>()?.text;
                    int objectNum = ExtractNumberFromText(textTarget);

                    if (customerNum == objectNum && sr != null && sprite == sr.sprite && color == sr.color)
                    {
                        targetMerge.isFoundCustomerMatch = true;
                        objectMerged.Add(targetMerge);
                        if (parentImg != null)
                            parentImg.color = Color.green;
                        foundMatch = true;
                        return true; // Stop after first match for this customerPrefab
                    }
                }
            }
        }

        return foundMatch;
    }
    private void UpdateCachedObjectMerges()
    {
        // Remove nulls from cache
        cachedObjectMerges.RemoveAll(item => item == null);

        // Add any new MergeableBase objects not already cached
        var foundObjects = UnityEngine.Object.FindObjectsByType<MergeableBase>(FindObjectsSortMode.None);
        foreach (var obj in foundObjects)
        {
            if (!cachedObjectMerges.Contains(obj))
            {
                cachedObjectMerges.Add(obj);
            }
        }

        // Reset match state
        matchFound = false;
        objectMerged.Clear();

        // Try to match each customer prefab
        foreach (var customerPrefab in CustomerObjectPrefabs)
        {
            TryMatchWithObjects(customerPrefab);
        }
    }

    private int ExtractNumberFromText(string text)
    {
        Match match = Regex.Match(text, @"\d+");
        return match.Success ? int.Parse(match.Value) : -1; // Return -1 if no number is found
    }

    private void OnCustomerButtonClick()
    {
        CoinManager.Instance.AddCoins(coinValue);
        UIManager.Instance.chooseOver.SetActive(false);
        foreach (var objectMerge in objectMerged) Destroy(objectMerge.gameObject);
        EndOrders = true;
        UIManager.Instance.CheckWave(this);
    }
}
