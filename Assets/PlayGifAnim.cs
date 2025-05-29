using UnityEngine.Video;
using UnityEngine;

public class PlayGifAnim : MonoBehaviour {
    public static PlayGifAnim instance { get; private set; }
    public VideoPlayer videoPlayer;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject); // جلوگیری از چند نسخه همزمان
        }
    }

    void Start() {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Play();
    }

    public void PlayVideo() {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null) {
            videoPlayer.Play();
            Debug.Log("PlayVideo");
        }
        else {
            Debug.LogWarning("VideoPlayer not found.");
        }
    }
}
