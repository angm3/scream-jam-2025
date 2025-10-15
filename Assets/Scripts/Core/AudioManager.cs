using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Clips (optional)")]
    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, AudioClip> soundDictionary;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Try to find sources if not assigned
        if (musicSource == null)
        {
            Transform musicChild = transform.Find("MusicSource");
            if (musicChild != null)
                musicSource = musicChild.GetComponent<AudioSource>();
            else
                musicSource = CreateChildSource("MusicSource", loop: true);
        }

        if (sfxSource == null)
        {
            Transform sfxChild = transform.Find("SFXSource");
            if (sfxChild != null)
                sfxSource = sfxChild.GetComponent<AudioSource>();
            else
                sfxSource = CreateChildSource("SFXSource", loop: false);
        }

        // Initialize dictionary for quick lookup
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
                soundDictionary.Add(sound.name, sound.clip);
        }

        UpdateVolumes();
    }
    
    private AudioSource CreateChildSource(string name, bool loop)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);
        AudioSource source = child.AddComponent<AudioSource>();
        source.loop = loop;
        source.spatialBlend = 0f; // 2D sound
        return source;
    }

    // --- Play SFX ---
    public void PlaySFX(string name, float volumeScale = 1f, float startTime = 0f)
    {
        if (soundDictionary.TryGetValue(name, out AudioClip clip))
            PlaySFX(clip, volumeScale, startTime);
        else
            Debug.LogWarning($"AudioManager: Sound '{name}' not found!");
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float startTime = 0f)
    {
        if (clip == null) return;

        // Play normally if starting from the beginning
        if (startTime <= 0f)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
            return;
        }

        // Otherwise, create a temporary AudioSource to start mid-clip
        GameObject tempGO = new GameObject($"SFX_{clip.name}");
        tempGO.transform.SetParent(transform);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        tempSource.clip = clip;
        tempSource.volume = sfxVolume * volumeScale;
        tempSource.loop = false;
        tempSource.spatialBlend = 0f;

        // Clamp timestamp so it doesn’t exceed clip length
        startTime = Mathf.Clamp(startTime, 0f, clip.length);
        tempSource.time = startTime;
        tempSource.Play();

        Destroy(tempGO, clip.length - startTime + 0.1f); // auto cleanup
    }

    public void PlayMusic(string name, bool loop = true)
    {
        Debug.Log($"AudioManager: PlayMusic '{name}'");
        if (soundDictionary.TryGetValue(name, out AudioClip clip))
            PlayMusic(clip, loop);
        else
            Debug.LogWarning($"AudioManager: Music '{name}' not found!");
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    private void UpdateVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}