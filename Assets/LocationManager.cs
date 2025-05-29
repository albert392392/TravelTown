using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour {
    // List of sessions, each containing a list of location levels
    public List<LocationSession> LocationSessions = new List<LocationSession>();

    private void Start() {
        // Example initialization: Create sessions and levels dynamically
        for (int i = 0; i < 3; i++) {
            LocationSession newSession = new LocationSession {
                SessionName = $"Session{i + 1}",
                LocationLevelLists = new List<GameObject>()
            };

            // Add levels to each session
            for (int j = 0; j < 5; j++) { // Assume 5 levels per session
                GameObject level = new GameObject($"Level{i + 1}-{j + 1}");
                newSession.LocationLevelLists.Add(level);
            }

            // Add the session to the list
            LocationSessions.Add(newSession);
        }
    }
}

[System.Serializable]
public class LocationSession {
    public string SessionName; // Name of the session (e.g., Session1, Session2)
    public List<GameObject> LocationLevelLists = new List<GameObject>(); // Levels within the session
}
