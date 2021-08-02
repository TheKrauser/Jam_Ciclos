using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClass
{ 
    public string name;

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;
 
    [HideInInspector]
    public AudioSource audioS;
}
