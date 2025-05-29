using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HelpMergeManager : MonoBehaviour {
    private List<MergeableBase> mergeables = new List<MergeableBase>();
    public float idleTimer = 0f;
    [SerializeField] private float idleThreshold = 6f;
    private Queue<System.Action> animationQueue = new Queue<System.Action>();
    public bool isAnimating = false;
    private void Update() {
        UpdateMergeableList();

        if (mergeables.Count < 2) return;

        if (AllObjectsIdle()) {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleThreshold) {
                idleTimer = 0f;

                var pair = FindClosestSimilarPair();
                if (pair.obj1 != null && pair.obj2 != null) {
                    if (pair.obj1.CurrentState == MergeableState.Idle && pair.obj2.CurrentState == MergeableState.Idle) {
                        EnqueueAnimation(() => AnimatedMergeBoth(pair.obj1.gameObject, pair.obj2.gameObject));
                    }
                    else {
                        idleTimer = 0f;
                    }
                }
            }
        }
        else {
            idleTimer = 0f;
        }
    }
    private void EnqueueAnimation(System.Action animation) {
        animationQueue.Enqueue(animation);
        TryExecuteNextAnimation();
    }
    private void TryExecuteNextAnimation() {
        if (isAnimating || animationQueue.Count == 0) return;
        isAnimating = true;
        var nextAnimation = animationQueue.Dequeue();
        nextAnimation.Invoke();
    }
    private void UpdateMergeableList() {
        mergeables.Clear();
        foreach (var m in FindObjectsOfType<MergeableBase>()) {
            if (m != null && m.gameObject.activeInHierarchy) {
                mergeables.Add(m);
            }
        }
    }
    private bool AllObjectsIdle() {
        foreach (var obj in mergeables) {
            if (obj == null || obj.CurrentState != MergeableState.Idle) {
                idleTimer = 0f;
                return false;
            }
        }
        return true;
    }
    private (MergeableBase obj1, MergeableBase obj2) FindClosestSimilarPair() {
        float minDist = Mathf.Infinity;
        MergeableBase result1 = null, result2 = null;

        for (int i = 0; i < mergeables.Count; i++) {
            for (int j = i + 1; j < mergeables.Count; j++) {
                var m1 = mergeables[i];
                var m2 = mergeables[j];

                if (m1 == null || m2 == null || m1 == m2) continue;
                if (!m1.IsSimilarTo(m2.gameObject)) continue;

                float dist = Vector3.Distance(m1.transform.position, m2.transform.position);
                if (dist < minDist) {
                    minDist = dist;
                    result1 = m1;
                    result2 = m2;
                }
            }
        }

        return (result1, result2);
    }
    private void AnimatedMergeBoth(GameObject obj1, GameObject obj2)
    {
        if (!CanAnimate(obj1) || !CanAnimate(obj2)) {
            isAnimating = false;
            TryExecuteNextAnimation();
            return;
        }

        Transform parent1 = obj1.transform.parent;
        Transform parent2 = obj2.transform.parent;

        Vector3 start1 = obj1.transform.position;
        Vector3 start2 = obj2.transform.position;

        Vector3 mid1 = Vector3.Lerp(start1, start2, 0.3f) + Vector3.up * 1f;
        Vector3 mid2 = Vector3.Lerp(start2, start1, 0.3f) + Vector3.up * 1f;

        DOTween.Kill(obj1.transform);
        DOTween.Kill(obj2.transform);


        var col1 = obj1.GetComponent<Collider>();
        var col2 = obj2.GetComponent<Collider>();

        if (col1 != null) col1.enabled = false;
        if (col2 != null) col2.enabled = false;


        Sequence seq1 = DOTween.Sequence();
        seq1.Append(obj1.transform.DOMove(mid1, 0.3f).SetEase(Ease.OutQuad));
        seq1.Append(obj1.transform.DOMove(start1, 0.3f).SetEase(Ease.InQuad));
        seq1.OnComplete(() => {
            if (col1 != null) col1.enabled = true;
        });

        Sequence seq2 = DOTween.Sequence();
        seq2.Append(obj2.transform.DOMove(mid2, 0.3f).SetEase(Ease.OutQuad));
        seq2.Append(obj2.transform.DOMove(start2, 0.3f).SetEase(Ease.InQuad));
        seq2.OnComplete(() => {
            if (col2 != null) col2.enabled = true;
            isAnimating = false;
            TryExecuteNextAnimation();
        });
    }


    private bool CanAnimate(GameObject obj) {
        return
            !IsUnderBox(obj) &&
            !IsUnderSpider(obj) && obj.TryGetComponent(out MergeableBase mb) && !mb.IsBusy;
    }
    private bool IsUnderBox(GameObject obj) {
        return
            obj.TryGetComponent<ObjectMerge>(out var om) && om.box != null ||
            obj.TryGetComponent<ScatterObjectsWithInterval>(out var so) && so.box != null;
    }

    private bool IsUnderSpider(GameObject obj) {
        return
            obj.TryGetComponent<ObjectMerge>(out var om) && om.SpiderWeb != null ||
            obj.TryGetComponent<ScatterObjectsWithInterval>(out var so) && so.SpiderWeb != null;
    }
}
