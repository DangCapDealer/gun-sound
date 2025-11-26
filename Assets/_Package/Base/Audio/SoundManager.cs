using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SoundManager : MonoSingletonGlobal<SoundManager>
{
    // Chỉ dùng một class SoundTable duy nhất, với mảng AudioClip
    [System.Serializable]
    public class SoundTable
    {
        public Sound sound;
        public AudioClip[] clips; // Dùng mảng để chứa 1 hoặc nhiều clip
    }

    [SerializeField] AudioSource audioSourceNormal;
    [SerializeField] AudioSource audioSourceSpecial;

    // Chỉ dùng một mảng SoundTable duy nhất
    [SerializeField] private SoundTable[] sounds;

    // Thay đổi từ Dictionary<Sound, AudioClip> sang Dictionary<Sound, AudioClip[]>
    private Dictionary<Sound, AudioClip[]> soundDics = new Dictionary<Sound, AudioClip[]>();
    private Queue<Audio3D> queue3d = new Queue<Audio3D>();

    protected override void Awake()
    {
        base.Awake();
        // Vòng lặp duyệt qua một mảng sounds duy nhất
        foreach (var _s in sounds)
        {
            if (_s.clips != null && _s.clips.Length > 0)
            {
                if (soundDics.ContainsKey(_s.sound) == false)
                    soundDics.Add(_s.sound, _s.clips);
                else
                    Debug.LogWarning($"[SoundManager] Key already exists for sound: {_s.sound}.");
            }
            else
            {
                Debug.LogWarning($"[SoundManager] Sound '{_s.sound}' has no clips assigned.");
            }
        }
    }

    private void Start()
    {
        queue3d.Clear();
        SetMute();
    }

    public void SetMute()
    {
        //StaticVariable.ClearLog();
        Debug.Log($"[SoundManager] Set sound mute: {!RuntimeStorageData.Sound.isSound}");

        audioSourceNormal.mute = !RuntimeStorageData.Sound.isSound;
        audioSourceSpecial.mute = !RuntimeStorageData.Sound.isSound;
    }    

    public void VolumeChange(float toVolume)
    {
        DOTween.To(() => audioSourceNormal.volume, x => {
            audioSourceNormal.volume = x;
            audioSourceSpecial.volume = x;
        }, toVolume, 1f);
    }

    public void PlayOnShot(Sound sound, float volume = 1f)
    {
        //Debug.Log($"[SoundManager] PLAY ONE SHOT {sound}");
        AudioClip clip = GetRandomClip(sound);

        if (clip != null)
        {
            //audioSourceNormal.volume = volume;
            audioSourceNormal.PlayOneShot(clip, volume);
        }
    }

    // Giữ nguyên hàm này nếu bạn muốn phát 1 AudioClip cụ thể
    public void PlayOnShot(AudioClip sound, float volume = 1f)
    {
        //audioSourceNormal.volume = volume;
        audioSourceNormal.PlayOneShot(sound, volume);
    }

    public void PlaySpecialSound(Sound sound, float volume = 1.0f)
    {
        AudioClip clip = GetRandomClip(sound);

        if (clip != null)
        {
            audioSourceSpecial.volume = volume;
            audioSourceSpecial.clip = clip;
            audioSourceSpecial.Play();
        }
    }

    public void StopSpecialSound()
    {
        audioSourceSpecial.Stop();
    }

    public AudioClip GetRandomClip(Sound sound)
    {
        if (soundDics.ContainsKey(sound))
        {
            AudioClip[] clips = soundDics[sound];
            if (clips != null && clips.Length > 0)
            {
                return clips[UnityEngine.Random.Range(0, clips.Length)];
            }
        }
        Debug.LogWarning($"[SoundManager] Sound not found or has no clips: {sound}");
        return null;
    }

    public AudioClip ConvertToClip(Sound sound)
    {
        return GetRandomClip(sound);
    }

    public void PlaySoundAtLocation(Sound id, Vector3 worldPosition, float volumeMultiply = 1)
    {
        if (id == Sound.NONE)
            return;
        if (queue3d.Count == 0)
        {
            var _obj = new GameObject("AudioSource", typeof(Audio3D), typeof(AudioSource));
            _obj.transform.parent = transform;
            queue3d.Enqueue(_obj.GetComponent<Audio3D>());
        }

        var clip = GetRandomClip(id);
        if (clip == null)
            return;
        var audio3D = queue3d.Dequeue();
        audio3D.SpawnAudio3D(clip, worldPosition, volumeMultiply, () => { queue3d.Enqueue(audio3D); });
    }
}