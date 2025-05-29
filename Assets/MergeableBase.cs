using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MergeableState {
    Idle,
    Dragging,
    Spawning,
    MovingToTarget,
    Animating,
    Merge,
    Swap,
    ReturnBackPos
}
public abstract class MergeableBase : MonoBehaviour
{
    public GameObject box;
    public GameObject SpiderWeb;
    public GameObject touchedObject;
    protected Transform parentTransformTargetCollider;
    protected Vector3 originalPosition;
    public GameObject mergedPrefab;
    public GameObject mergedObject;
    public CustomerScript customerScript;
    public CustomerScript CustomerScript;
    [SerializeField] protected Transform lastTargetPosition = null;
    public Transform lastlastTargetPosition = null;
    public bool isTouchUp = false;
    public bool isTouchDown = false;
    public bool isNearBox = false;
    public bool isFoundCustomerMatch;
    protected Collider targetCollider;
    protected Collider objectCollider;
    public static List<GameObject> objectsList = new List<GameObject>();
    private readonly Queue<Collider> colliderQueue = new Queue<Collider>();
    [SerializeField] private float detectionRadius = 11f;
    public MergeableState CurrentState { get; private set; } = MergeableState.Idle;
    public bool IsBusy => CurrentState != MergeableState.Idle;

    public MergeableBase lastEnteredBy = null;

    public GameObject otherGameObjectToswap = null;
    protected virtual void Awake()
    {
        var mover = GetComponent<ObjectMover>();
        if (mover != null) mover.enabled = false;
        var draggable = GetComponent<DraggableObject>();
        if (draggable != null) draggable.enabled = false;
        
        objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
            objectCollider.layerOverridePriority = 5;
        originalPosition = transform.position;
        lastTargetPosition = transform.parent;
    }
    protected void SetState(MergeableState newState) {
        CurrentState = newState;
    }
    protected bool CanInteract()
    {
        return CurrentState != MergeableState.MovingToTarget && CurrentState != MergeableState.Swap && objectCollider != null && objectCollider.enabled && CurrentState != MergeableState.Merge;
    }

    public virtual void SwapWith(GameObject other)
    {
        if (other == null || other.transform.parent == null || transform.parent == null) return;
        if (CurrentState == MergeableState.MovingToTarget || CurrentState == MergeableState.Swap) return;

        var otherMergeable = other.GetComponent<MergeableBase>();
        if (otherMergeable == null) return;

        if (box != null || SpiderWeb != null ||
            otherMergeable.box != null || otherMergeable.SpiderWeb != null)
        {
            Debug.Log("[SwapWith] Swap prevented due to box or spider web.");
            return;
        }

        SetState(MergeableState.Swap);

        Vector3 otherPos = other.transform.parent.position;
        Vector3 thisPos = transform.parent.position;

        Transform otherParent = other.transform.parent;
        Transform thisParent = transform.parent;

        transform.SetParent(otherParent);
        other.transform.SetParent(thisParent);

        DOTween.Sequence()
            .Append(transform.DOMove(otherPos, 0.3f))
            .Join(other.transform.DOMove(thisPos, 0.3f))
            .OnComplete(() => { SetState(MergeableState.Idle); });

        otherGameObjectToswap = other;
    }

    public bool CanMergeWith(GameObject otherObject)
    {
        var otherMerge = otherObject.GetComponent<MergeableBase>();
        if (otherMerge == null || mergedPrefab == null) return false;
        return IsSimilarTo(otherObject);
    }

    public bool CanMerge(GameObject otherObject)
    {
        if (CanMergeWith(otherObject))
        {
            isNearBox = false;
            lastTargetPosition = otherObject.transform.parent;
            MergeWith(otherObject);
            return true;
        }
        var thisBase = GetComponent<MergeableBase>();
        var otherBase = otherObject.GetComponent<MergeableBase>();
        if (thisBase != null && otherBase != null && !thisBase.IsSimilarTo(otherObject))
        {
            if (otherBase.SpiderWeb != null)
            {
                ReturnToOriginalPosition();
                return false;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (!colliderQueue.Contains(other))
            colliderQueue.Enqueue(other);

        if (colliderQueue.Count > 0)
            ProcessNextCollider();

        if (GridManager.Instance != null)
        {
            foreach (var emptyTargetPosition in GridManager.Instance.emptytargetPotations)
            {
                if (other.transform == emptyTargetPosition && CurrentState != MergeableState.Swap)
                {
                    lastTargetPosition = emptyTargetPosition;
                    break;
                }
            }
        }

        if (other.CompareTag("Box"))
            isNearBox = true;

        if (targetCollider != null) return;

        var otherMerge = other.GetComponent<MergeableBase>();

        bool canMerge = false;

        if (otherMerge != null && otherMerge.CurrentState != MergeableState.Dragging && otherMerge != this && otherMerge.CurrentState != MergeableState.MovingToTarget)
            HandleMergeOrSwap(otherMerge.gameObject, ref canMerge);

        if (canMerge)
        {
            targetCollider = other;
            parentTransformTargetCollider = other.transform.parent;        
        }
        else if (!canMerge && other.GetComponent<MergeableBase>().otherGameObjectToswap != null){
            otherGameObjectToswap = null;
        }
    }

    private void ProcessNextCollider()
    {
        if (colliderQueue.Count > 0)
            colliderQueue.Dequeue();
    }

    private void HandleMergeOrSwap(GameObject otherObject, ref bool canMerge)
    {
        if (IsSimilarTo(otherObject))
        {
            PlayMergeEffect(otherObject.transform.position);
            canMerge = true;
        }
    }

    public void MoveToTarget(Transform target)
    {
        if (GridManager.Instance == null || !GridManager.Instance.IsPositionInGrid(target.position))
        {
            Debug.LogWarning("Target position is invalid!");
            ReturnToOriginalPosition();
            return;
        }

        DOTween.Kill(transform);
        if (CurrentState != MergeableState.MovingToTarget)
        {
            SetState(MergeableState.MovingToTarget);
            transform.DOMove(target.position, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.SetParent(target);
                transform.position = target.position;
                originalPosition = transform.position;
                lastTargetPosition = transform.parent;
                SetChooseOver(true, lastTargetPosition);
                SetState(MergeableState.Idle);
            });
        }
    }

    public void UpdateChooseOver(Transform target)
    {
        if (UIManager.Instance.chooseOver != null)
        {
            UIManager.Instance.chooseOver.SetActive(false);
            UIManager.Instance.chooseOver.transform.position = target.position;
            UIManager.Instance.chooseOver.SetActive(true);
        }
    }

    public void SetChooseOver(bool active, Transform target = null)
    {
        if (UIManager.Instance.chooseOver == null) return;
        UIManager.Instance.chooseOver.SetActive(false);
        if (target != null)
            UIManager.Instance.chooseOver.transform.position = target.position;
        UIManager.Instance.chooseOver.SetActive(active);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;

        if (targetCollider == other)
        {
            targetCollider = null;
            if (UIManager.Instance != null && UIManager.Instance.merge_particleSystem != null)
                UIManager.Instance.merge_particleSystem.Stop();
            parentTransformTargetCollider = null;
        }

        if (other.CompareTag("Box"))
            isNearBox = false;

        if (GridManager.Instance != null)
        {
            foreach (var emptyTargetPosition in GridManager.Instance.emptytargetPotations)
            {
                if (other.transform == emptyTargetPosition)
                {
                    lastTargetPosition = null;
                    break;
                }
            }
        }
        //otherGameObjectToswap = null;
    }

    private void PlayMergeEffect(Vector3 position)
    {
        if (UIManager.Instance.merge_particleSystem != null)
        {
            UIManager.Instance.merge_particleSystem.transform.position = position;
            UIManager.Instance.merge_particleSystem.Play();
        }
    }

    public void MergeWith(GameObject otherObject)
    {
        if (mergedPrefab == null || otherObject == null) return;

        // Stop merge particle effect if active
        if (UIManager.Instance?.merge_particleSystem != null)
            UIManager.Instance.merge_particleSystem.Stop();

        Vector3 mergePosition = otherObject.transform.position;
        Transform mergeParent = parentTransformTargetCollider != null ? parentTransformTargetCollider : otherObject.transform.parent;
        SetState(MergeableState.Merge);

        // Instantiate merged object
        mergedObject = Instantiate(mergedPrefab, mergePosition, Quaternion.identity, mergeParent);

        // Register merged object with customer script if available
        CustomerScript?.objectMerged.Add(mergedObject.GetComponent<MergeableBase>());

        // Increment merge count for rotation hint if present
        var objectRotationHint = FindObjectOfType<ObjectRotationHint>();
        if (objectRotationHint != null)
            objectRotationHint._MergeSpawnCount++;

        // Add merged object to global list
        objectsList.Add(mergedObject);

        // Check for spider in parent and handle accordingly
        Transform parent = otherObject.transform.parent;
        bool spiderFound = false;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child.GetComponent<SpiderScript>())
                {
                    Destroy(child.gameObject);
                    mergedObject.transform.position = mergePosition;
                    Destroy(gameObject, 0.2f);
                    Destroy(otherObject, 0.2f);
                    spiderFound = true;
                    break;
                }
            }
        }

        if (!spiderFound)
        {
            objectsList.Remove(gameObject);
            objectsList.Remove(otherObject);
            Destroy(gameObject);
            Destroy(otherObject);
        }


        // Kill any ongoing tweens for both objects
        DOTween.Kill(transform);
        if (otherObject != null)
            DOTween.Kill(otherObject.transform);

        // Update chooseOver UI to new merged object
        if (mergedObject != null && UIManager.Instance?.chooseOver != null)
        {
            UIManager.Instance.chooseOver.SetActive(false);
            if (mergedObject.transform.parent != null)
                UIManager.Instance.chooseOver.transform.position = mergedObject.transform.parent.position;
            UIManager.Instance.chooseOver.SetActive(true);
        }
        //SetState(MergeableState.Idle);
    }

    public void CheckForTargetsAndDestroy(Transform MainTransform)
    {
        // Use Vector3 for directions to support 3D raycasting
        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.up, Vector3.down };
        foreach (Vector3 dir in directions)
        {
            // RaycastHit is a struct, so we can reuse a single variable
            RaycastHit hit;
            // Use LayerMask to only hit "Box" objects if possible (optional, for optimization)
            if (Physics.Raycast(MainTransform.parent.parent.position, dir, out hit, detectionRadius))
            {
                // Use CompareTag for performance and null safety
                if (hit.collider != null && hit.collider.CompareTag("Box"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    public bool IsSimilarTo(GameObject other)
    {
        var thisSR = GetComponent<SpriteRenderer>();
        var thisTMP = GetComponentInChildren<TextMeshPro>();
        var otherSR = other.GetComponent<SpriteRenderer>();
        var otherTMP = other.GetComponentInChildren<TextMeshPro>();

        if (thisSR == null || thisTMP == null || otherSR == null || otherTMP == null)
            return false;

        return thisSR.sprite == otherSR.sprite &&
               thisSR.color == otherSR.color &&
               thisTMP.text == otherTMP.text;
    }

    protected void ReturnToOriginalPosition()
    {
        SetState(MergeableState.ReturnBackPos);
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SetState(MergeableState.Idle);
            UpdateChooseOver(transform.parent);
        });
    }
}
