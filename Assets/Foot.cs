using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Foot : MonoBehaviour {
    [SerializeField] private PlayerCharacter parentScript;
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
            parentScript = GetComponentInParent<PlayerCharacter>();
        }
        footCollider = GetComponent<Collider2D>();
        //FindColliderWithCollisionTag();
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
    /*private void FindColliderWithCollisionTag() {
        Collider2D[] collisionCollider = FindObjectsOfType<Collider2D>();
        foreach (var filterColliders in collisionCollider) {
            if(filterColliders.CompareTag("Collision")) {
                thingColliders.Add(filterColliders);
            }
        }
    }*/
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
