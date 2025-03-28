using UnityEngine;
using UnityEngine.SceneManagement;

public class HomepageAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private string homepageSceneName = "HomePage";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        HandleAudio(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleAudio(scene);
    }

    private void HandleAudio(Scene scene)
    {
        if (scene.name == homepageSceneName)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
