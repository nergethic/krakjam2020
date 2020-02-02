using System;
using System.Collections;
using System.Collections.Generic;
using _Code;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private StartMenu startMenu;
    private const string playerTag = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(playerTag))
        {startMenu.ShowWinOverlay(other.GetComponent<PlayerController>().PadIndex == 1);
            Time.timeScale = 0;
            
        }
    }
}
