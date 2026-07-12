using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    private Animator anim;
    private PlayerController player;
    private bool isOpened;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.TryGetComponent<PlayerController>(out var input))
        {
            player = input;
        }
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.TryGetComponent<PlayerController>(out var input))
        {
            if (input == player)
            {
                player = null;
            }
        }
    }

    private void Update()
    {
        if(isOpened && player == null) 
        { 
            return;
        }
    }
}
