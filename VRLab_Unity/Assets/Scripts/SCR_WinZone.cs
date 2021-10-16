using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_WinZone : MonoBehaviour
{
    public LayerMask pieceLayer;

    public float countdown;

    private float startTime;
    private bool canCount;

    private List<Collider> colliderList = new List<Collider>();
 

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Piece")
        {
            colliderList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Piece")
        {
            colliderList.Remove(other);
        }
    }

    private void StartCountdown()
    {
        startTime = (float)AudioSettings.dspTime;
        canCount = true;
    }

    private void Update()
    {
        if (colliderList.Count > 0 && !canCount)
        {
            StartCountdown();
        }
        else if (colliderList.Count == 0)
        {
            canCount = false;
        }

        if (canCount && AudioSettings.dspTime - startTime >= countdown)
        {
            print("you win");
        }
    }

}
