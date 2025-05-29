using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootDownDraggable : MonoBehaviour {
    [SerializeField] private DraggableObjectCustomizer parentScript;
    [SerializeField] private Collider2D footCollider;

    [Header("Ground & Things")]
    [SerializeField] private List<Collider2D> groundColliders;
    [SerializeField] private List<Collider2D> thingColliders;

    private Vector2 savedPosition;
    private Vector2 savedScale;

    public bool isOnGround = false;
    public bool isOverThing = false;

    public void Start() {
        if (parentScript == null) {
            parentScript = GetComponentInParent<DraggableObjectCustomizer>();
        }
        footCollider = GetComponent<Collider2D>();

    }

    public void Update() {
   

        Bounds footBounds = footCollider.bounds;

        isOnGround = CheckCollision(footBounds, groundColliders);

        isOverThing = CheckCollision(footBounds, thingColliders);



        if (isOnGround && parentScript.isDragging) {
            savedPosition = parentScript.transform.position;
            savedScale = parentScript.transform.localScale;
            SetAlpha(1f);
            isOnGround = true;
        }
        if (isOverThing) {
            if (!parentScript.isDragging) {
                parentScript.SnapBack(savedPosition, savedScale);
                isOverThing = false;
                SetAlpha(1f);
            }
            else {
                SetAlpha(0.6f);
            }
            isOnGround = false;
        }
        else {
            if (parentScript.isDragging) {
                SetAlpha(1f);
            }
        }
    }

    private bool CheckCollision(Bounds myBounds, List<Collider2D> targetColliders) {
        foreach (var col in targetColliders) {
            if (col.bounds.Intersects(myBounds)) {
                return true;
            }
        }
        return false;
    }
    private void SetAlpha(float alpha) {
        SpriteRenderer sprite = parentScript.GetComponent<SpriteRenderer>();
        if (sprite != null) {
            Color c = sprite.color;
            c.a = Mathf.Lerp(c.a, alpha, Time.deltaTime * 10f);
            sprite.color = c;
        }
    }
}
