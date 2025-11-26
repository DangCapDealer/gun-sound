using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SoundManager : SingletonGlobal<SoundManager>
{
    public enum Sound
    {
        NONE,
        BUTTON,
        LOSE,
        WIN,
    }

    [Serializable]
    public class SoundTable
    {
        public Sound sound;
        public AudioClip[] clips;
    }

    [SerializeField] private SoundTable[] sounds;
    [SerializeField] private AudioSource audioSourceSpecial;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float minIntervalSameSound = 0.05f;

    [Header("Volume Limiter")]
    public bool EnableVolumeLimiter = true;
    [Range(0f, 1f)] public float MaxVolumeDifference = 0.3f;
    [Range(0f, 1f)] public float LimitedVolume = 0.7f;

    private Dictionary<Sound, AudioClip[]> soundDics = new();
    private List<AudioSource> audioSourcePool = new();
    private int poolIndex = 0;
    private Dictionary<Sound, float> lastPlayTime = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var s in sounds)
        {
            if (s.clips != null && s.clips.Length > 0)
            {
                if (!soundDics.ContainsKey(s.sound))
                    soundDics.Add(s.sound, s.clips);
            }
        }
        InitAudioSourcePool();
    }

    private void InitAudioSourcePool()
    {
        audioSourcePool.Clear();
        for (int i = 0; i < poolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.mute = !RuntimeStorageData.Sound.isSound;
            audioSourcePool.Add(src);
        }
    }

    private void Start()
    {
        SetMute();
    }

    public void SetMute()
    {
        bool mute = !RuntimeStorageData.Sound.isSound;
        foreach (var src in audioSourcePool)
            src.mute = mute;
        if (audioSourceSpecial) audioSourceSpecial.mute = mute;
    }

    public void VolumeChange(float toVolume)
    {
        DOTween.To(() => audioSourcePool[0].volume, x =>
        {
            foreach (var src in audioSourcePool)
                src.volume = x;
            if (audioSourceSpecial) audioSourceSpecial.volume = x;
        }, toVolume, 1f);
    }

    public void PlayOnShot(Sound sound, float volume = 1f)
    {
        if (!soundDics.TryGetValue(sound, out var clips) || clips.Length == 0) return;

        // Giảm spam: Nếu âm này vừa phát, chờ một chút mới cho phát tiếp
        float now = Time.unscaledTime;
        if (lastPlayTime.TryGetValue(sound, out float last) && now - last < minIntervalSameSound)
            return;
        lastPlayTime[sound] = now;

        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        var src = GetAvailableAudioSource();
        if (src && clip)
        {
            float finalVolume = volume;
            if (EnableVolumeLimiter)
            {
                float maxVol = 0f, minVol = 1f;
                foreach (var s in audioSourcePool)
                {
                    if (s.isPlaying)
                    {
                        if (s.volume > maxVol) maxVol = s.volume;
                        if (s.volume < minVol) minVol = s.volume;
                    }
                }
                if (maxVol - minVol > MaxVolumeDifference)
                {
                    finalVolume = LimitedVolume;
                    // Optionally, đồng bộ lại các AudioSource đang phát
                    foreach (var s in audioSourcePool)
                        if (s.isPlaying) s.volume = LimitedVolume;
                }
            }
            src.volume = finalVolume;
            src.PlayOneShot(clip, finalVolume);
        }
    }

    public void PlayOnShot(AudioClip clip, float volume = 1f)
    {
        var src = GetAvailableAudioSource();
        if (src && clip)
        {
            float finalVolume = volume;
            if (EnableVolumeLimiter)
            {
                float maxVol = 0f, minVol = 1f;
                foreach (var s in audioSourcePool)
                {
                    if (s.isPlaying)
                    {
                        if (s.volume > maxVol) maxVol = s.volume;
                        if (s.volume < minVol) minVol = s.volume;
                    }
                }
                if (maxVol - minVol > MaxVolumeDifference)
                {
                    finalVolume = LimitedVolume;
                    foreach (var s in audioSourcePool)
                        if (s.isPlaying) s.volume = LimitedVolume;
                }
            }
            src.volume = finalVolume;
            src.PlayOneShot(clip, finalVolume);
        }
    }

    public void PlaySpecialSound(Sound sound, float volume = 1.0f)
    {
        if (!soundDics.TryGetValue(sound, out var clips) || clips.Length == 0) return;
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (audioSourceSpecial && clip)
        {
            audioSourceSpecial.volume = volume;
            audioSourceSpecial.clip = clip;
            audioSourceSpecial.Play();
        }
    }

    public void StopSpecialSound()
    {
        if (audioSourceSpecial) audioSourceSpecial.Stop();
    }

    public AudioClip GetRandomClip(Sound sound)
    {
        if (soundDics.TryGetValue(sound, out var clips) && clips.Length > 0)
            return clips[UnityEngine.Random.Range(0, clips.Length)];
        return null;
    }

    public AudioClip ConvertToClip(Sound sound) => GetRandomClip(sound);

    private AudioSource GetAvailableAudioSource()
    {
        // Tìm AudioSource chưa phát hoặc đã phát xong
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            int idx = (poolIndex + i) % audioSourcePool.Count;
            if (!audioSourcePool[idx].isPlaying)
            {
                poolIndex = (idx + 1) % audioSourcePool.Count;
                return audioSourcePool[idx];
            }
        }
        // Nếu tất cả đang phát, lấy vòng tròn (ghi đè cái cũ nhất)
        poolIndex = (poolIndex + 1) % audioSourcePool.Count;
        return audioSourcePool[poolIndex];
    }
}