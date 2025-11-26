using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Audio3D : MonoBehaviour
{
    private AudioSource sound;
    private Action c_Action;
    private float timer;

    public void SpawnAudio3D(AudioClip clip, Vector3 worldPosition, float volumeMultiply, Action action)
    {
        if (sound == null)
            sound = this.GetComponent<AudioSource>();

        c_Action = action;
        timer = clip.length;

        sound.enabled = true;
        transform.position = worldPosition;
        sound.PlayOneShot(clip, volumeMultiply);

        //StartCoroutine(Timer());
        DOVirtual.DelayedCall(timer, () =>
        {
            c_Action?.Invoke();
            sound.enabled = false;
        });
    }

    //private IEnumerator Timer()
    //{
    //    yield return WaitForSecondCache.GetWFSCache(timer);

    //}
}