using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public GameObject startButton;
    public string sceneToLoad = "Home";

    public void PlayIntroVideo()
    {
        Debug.Log("▶️ 播放视频");

        startButton.SetActive(false);
        videoImage.gameObject.SetActive(true);

        videoPlayer.loopPointReached += OnVideoFinished;

        // 推荐：先Prepare，准备好后再Play
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("视频准备完成，开始播放");
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("✅ 视频播放结束，加载场景");
        SceneManager.LoadScene(sceneToLoad);
    }
}
