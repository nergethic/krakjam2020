using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Sound", menuName="Sound")]
public class Sound : ScriptableObject
{
    public new string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;

    public float pitch=1;
    [HideInInspector]
    public AudioSource source;
}
