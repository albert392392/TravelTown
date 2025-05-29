using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour {

    [System.Serializable]
    public class ObjectState {
        public Vector3 position;
        public bool isActive;
    }
    public GameObject[] prefab;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    [SerializeField] private List<List<ObjectState>> objectStates = new List<List<ObjectState>>();

    // Start is called before the first frame update
    void Start()
    {
        RestoreObjectStates();
    }
    public void SaveObjectStates(int sessionIndex) {
        objectStates[sessionIndex].Clear();

        foreach (GameObject obj in spawnedObjects)
        {
            ObjectState state = new ObjectState
            {
                position = obj.transform.position,
                isActive = obj.activeSelf
            };
            objectStates[sessionIndex].Add(state);
        }
    }
    public void RestoreObjectStates(int sessionIndex = 0) {
        foreach(ObjectState state in objectStates[sessionIndex]) {
            GameObject newObj = Instantiate(prefab[sessionIndex], state.position,Quaternion.identity);
            newObj.SetActive(state.isActive);
            spawnedObjects.Add(newObj);
        }
    }
}


