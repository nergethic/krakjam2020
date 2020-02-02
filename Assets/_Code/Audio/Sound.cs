using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.Audio;


[CreateAssetMenu(fileName ="New Sound", menuName="Sound")]
public class Sound : ScriptableObject
{
    public new string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;

    public AudioMixerGroup mixerGroup;
    public float pitch=1;
    [HideInInspector]
    public AudioSource source;
}
