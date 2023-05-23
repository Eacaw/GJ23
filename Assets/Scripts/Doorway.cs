using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    public Light hoverIndicatorLight;
    public Transform destination;
    public bool isLocked = false;

    private void Start()
    {
        hoverIndicatorLight.intensity = 0;
    }

    public void OnMouseDown()
    {
        // If the door is locked and hte player doesnt have the key, return
        if (
            isLocked
            && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().hasKey
                == false
        ) { }
        // Move the player to the destination
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = destination.position;
    }

    public void OnMouseEnter()
    {
        hoverIndicatorLight.intensity = 5;
    }

    public void OnMouseExit()
    {
        hoverIndicatorLight.intensity = 0;
    }
}
