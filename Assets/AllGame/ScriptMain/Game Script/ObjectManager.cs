using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;
    public bool isDragging = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
}
