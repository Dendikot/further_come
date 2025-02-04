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

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
        sphereSource.clip = sphereBackgorund;
        sphereSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
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
}
