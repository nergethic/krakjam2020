using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
   [SerializeField] private AudioSource musicAudioSource;
   public List<Sound> sounds;
   public static AudioManager audioManagerInstance;
   
   [ContextMenu("AddAudioToList")]
   void AddAudioToList()
   {
      sounds.Add(null);
   }
   
   void Awake()
   {
      if (audioManagerInstance == null)
      {
         audioManagerInstance = this;
      }
      else
      {
         Destroy(gameObject);
         return;
      }
      DontDestroyOnLoad(gameObject);
   }
   
   public void PlaySound(string name)
   {
      Sound s = sounds.Find(sound => sound.name == name);
      if (s != null)
      {
         s.source = musicAudioSource;
         s.source.volume = s.volume;
         s.source.clip = s.clip;
         musicAudioSource.PlayOneShot(s.source.clip);
      }
      else
      {
         Debug.LogError("Can't find " + name + " clip");
      }
   }


}
