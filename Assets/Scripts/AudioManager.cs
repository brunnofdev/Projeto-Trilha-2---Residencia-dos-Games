using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Músicas")]
    [SerializeField] private AudioClip lofiMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("Efeitos")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip missClip;
    [SerializeField] private AudioClip shatterClip;
    [SerializeField] private AudioClip uiClickClip;


    private float minPitch = 0.92f;
    private float maxPitch = 1.08f;

    public void PlayLofiMusic()
    {
        if (musicSource == null || lofiMusic == null)
            return;

        if (musicSource.clip == lofiMusic && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = lofiMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        if (musicSource == null || victoryMusic == null)
            return;

        musicSource.Stop();
        musicSource.clip = victoryMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayHitSound()
    {
        if (sfxSource == null || hitClip == null)
            return;

        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(hitClip);
    }

    public void PlayMissSound()
    {
        if (sfxSource == null || missClip == null)
            return;

        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(missClip);
    }

    public void PlayShatterSound()
    {
        if (sfxSource == null || shatterClip == null)
            return;

        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(shatterClip);
    }

    public void PlayUIClick()
    {
        if (sfxSource == null || uiClickClip == null)
            return;

        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(uiClickClip);
    }
}