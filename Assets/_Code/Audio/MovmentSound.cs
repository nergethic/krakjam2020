using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MovmentSound : MonoBehaviour
{
    private string step1 = "step1";
    private string step2 = "step2";
    private string step3 = "step3";
    [SerializeField] private Sound step1Sound;
    [SerializeField] private Sound step2Sound;
    [SerializeField] private Sound step3Sound;
    private List<Sound> stepList;
    private List<String> stepStringList;
    
    private void Start()
    {
        stepStringList=new List<String>()
        {
            step1,
            step2,
            step3
    
        };
      stepList=new List<Sound>()
        {
            step1Sound,
            step2Sound,
            step3Sound
    
        };
    }


    void PlayStepSound()
    {
        int index = Random.Range(0, 3);

        float pitchRnd = Random.Range(0.7f, 1.3f);
        stepList[index].pitch = pitchRnd;
        AudioManager.audioManagerInstance.PlaySound(stepStringList[index]);

    }
    
}
