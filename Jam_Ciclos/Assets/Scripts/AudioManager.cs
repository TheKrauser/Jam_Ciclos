using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public SoundClass[] sfxSounds, sfxAmbience, sfxSoundtrack;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (SoundClass s in sfxSounds)
        {
            s.audioS = gameObject.AddComponent<AudioSource>();
            s.audioS.clip = s.clip;
            s.audioS.volume = s.volume;
            s.audioS.pitch = s.pitch;
            s.audioS.loop = s.loop;
        }

        foreach (SoundClass s in sfxAmbience)
        {
            s.audioS = gameObject.AddComponent<AudioSource>();
            s.audioS.clip = s.clip;
            s.audioS.volume = s.volume;
            s.audioS.pitch = s.pitch;
            s.audioS.loop = s.loop;
        }

        foreach (SoundClass s in sfxSoundtrack)
        {
            s.audioS = gameObject.AddComponent<AudioSource>();
            s.audioS.clip = s.clip;
            s.audioS.volume = s.volume;
            s.audioS.pitch = s.pitch;
            s.audioS.loop = s.loop;
        }
    }

    public void PlaySoundEffect(string soundName)
    {
        SoundClass s = Array.Find(sfxSounds, sound => sound.name == soundName);
        if (s == null)
            return;

        s.audioS.Play();
    }

    public void PlaySoundtrack(string soundName)
    {
        SoundClass s = Array.Find(sfxSoundtrack, sound => sound.name == soundName);
        if (s == null)
            return;

        s.audioS.Play();
    }

    public void StopSoundtrack(string soundName)
    {
        SoundClass s = Array.Find(sfxSoundtrack, sound => sound.name == soundName);
        if (s == null)
            return;

        StartCoroutine(FadeSound(s, 0.01f));
    }

    public void PlayAmbience(string soundName)
    {
        SoundClass s = Array.Find(sfxAmbience, sound => sound.name == soundName);
        if (s == null)
            return;

        s.audioS.Play();
    }

    public void StopAmbience(string soundName)
    {
        SoundClass s = Array.Find(sfxAmbience, sound => sound.name == soundName);
        if (s == null)
            return;

        StartCoroutine(FadeSound(s, 0.05f));
    }

    private IEnumerator FadeSound(SoundClass sound, float time)
    {
        float value = sound.audioS.volume * 0.01f;

        while (sound.audioS.volume > 0)
        {
            yield return new WaitForSecondsRealtime(time);
            sound.audioS.volume -= value;
        }

        if (sound.audioS.volume <= 0)
        {
            sound.audioS.volume = 0;
        }

        sound.audioS.Stop();
    }
}
