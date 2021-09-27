using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SCR_SimpleGun : MonoBehaviour
{
    public Transform MuzzleAnchor;
    public GameObject MuzzleFlash;

    public void Fire()
    {
        //raycast forward 
        //player sound
        MuzzleFlash.GetComponentInChildren<VisualEffect>().Play();
    }
}
