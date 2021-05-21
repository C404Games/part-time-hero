using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endVideo : MonoBehaviour
{
    private void Start()
    {
        // Will attach a VideoPlayer to the main camera.
        GameObject camera = GameObject.Find("Main Camera");

        var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.playOnAwake = false;

        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer.targetCameraAlpha = 1F;

        videoPlayer.url = "Assets/Resources/Videos/Intro C404.mp4";

        videoPlayer.frame = 100;
        videoPlayer.SetDirectAudioMute(0, true);

        videoPlayer.isLooping = false;

        // Each time we reach the end, we slow down the playback by a factor of 10.
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.Play();
    }
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        Debug.Log("Cambio a MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}
