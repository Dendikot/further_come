using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [SerializeField]
    public AudioSource musicSource;
    [SerializeField]
    public AudioSource sphereSource;
    [SerializeField]
    public AudioSource SFXSource;

    public AudioClip background;
    public AudioClip sphereBackgorund;
    public AudioClip mouseClick;
    public AudioClip sphere;
    public AudioClip particles;
    public AudioClip lerp;
    public AudioClip[] signals;

    private float lastPlayTime = 0f;
    public float minInterval = 0.5f;

    private AudioClip randomSignal;

    public static Audio_Manager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
        sphereSource.clip = sphereBackgorund;
        sphereSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.volume = 1;
        SFXSource.PlayOneShot(clip);
    }

    public void StopBackground(AudioSource audioSource)
    {
        audioSource.Stop();
    }
    public void PlayBackground(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void PlayRandomSignal()
    {
        SFXSource.volume = 0.3f;
        if (Time.time - lastPlayTime < minInterval)
        {
            return;
        }
        lastPlayTime = Time.time;
        randomSignal = signals[Random.Range(0, signals.Length)];
        SFXSource.PlayOneShot(randomSignal);
    }
}
