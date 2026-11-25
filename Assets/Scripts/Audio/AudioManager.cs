
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string soundName;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        [Range(0.1f, 3f)]
        public float pitch = 1f;
    }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Sound Library")]
    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    [Header("Audio Settings")]
    [Min(1)]
    public int sfxSourceCount = 12;

    private AudioSource bgmSource;
    private readonly List<AudioSource> sfxSources =
        new List<AudioSource>();

    private readonly Dictionary<string, SoundEffect> soundDictionary =
        new Dictionary<string, SoundEffect>();

    private int sourceIndex = 0;

    private void Awake()
    {
        // Singleton: ป้องกัน AudioManager ซ้ำระหว่างเปลี่ยน Scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeBGM();
        InitializeSFXPool();
        BuildSoundLibrary();
    }

    private void InitializeBGM()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.spatialBlend = 0f;
        bgmSource.outputAudioMixerGroup = bgmGroup;
    }

    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxSourceCount; i++)
        {
            GameObject audioObject =
                new GameObject("SFX_Source_" + i);

            audioObject.transform.SetParent(transform);

            AudioSource source =
                audioObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = sfxGroup;

            sfxSources.Add(source);
        }
    }

    private void BuildSoundLibrary()
    {
        soundDictionary.Clear();

        foreach (SoundEffect sound in soundEffects)
        {
            if (sound == null ||
                string.IsNullOrWhiteSpace(sound.soundName) ||
                sound.clip == null)
            {
                continue;
            }

            if (soundDictionary.ContainsKey(sound.soundName))
            {
                Debug.LogWarning(
                    "Duplicate sound name: " + sound.soundName
                );

                continue;
            }

            soundDictionary.Add(sound.soundName, sound);
        }
    }

    // =====================================
    // PLAY SOUND EFFECT
    // =====================================


    public void PlaySFX(string soundName)
    {
        Debug.Log("Trying to play sound: " + soundName);

        if (!soundDictionary.TryGetValue(soundName, out SoundEffect sound))
        {
            Debug.LogError("Sound not found: " + soundName);
            return;
        }

        if (sound.clip == null)
        {
            Debug.LogError("AudioClip is missing: " + soundName);
            return;
        }

        AudioSource source = GetAvailableSFXSource();

        if (source == null)
        {
            Debug.LogError("No available SFX AudioSource!");
            return;
        }

        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip, sound.volume);

        Debug.Log("Playing: " + soundName +
                  " | Clip: " + sound.clip.name +
                  " | Volume: " + sound.volume);
    }


    private AudioSource GetAvailableSFXSource()
    {
        if (sfxSources.Count == 0)
            return null;

        // หา AudioSource ที่ว่างอยู่ก่อน
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                source.pitch = 1f;
                return source;
            }
        }

        // ถ้าทุกช่องกำลังเล่นอยู่ ให้ใช้ช่องถัดไป
        AudioSource selectedSource =
            sfxSources[sourceIndex];

        sourceIndex++;

        if (sourceIndex >= sfxSources.Count)
            sourceIndex = 0;

        selectedSource.Stop();
        selectedSource.pitch = 1f;

        return selectedSource;
    }

    // =====================================
    // BGM
    // =====================================

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
            return;

        if (bgmSource.clip == clip &&
            bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    // =====================================
    // VOLUME SETTINGS
    // =====================================

    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        SetMixerVolume("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);
    }

    private void SetMixerVolume(
        string parameterName,
        float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("Audio Mixer is missing!");
            return;
        }

        // แปลงค่า 0-1 เป็น Decibel
        float volume = Mathf.Log10(
            Mathf.Clamp(value, 0.0001f, 1f)
        ) * 20f;

        audioMixer.SetFloat(
            parameterName,
            volume
        );
    }
}
