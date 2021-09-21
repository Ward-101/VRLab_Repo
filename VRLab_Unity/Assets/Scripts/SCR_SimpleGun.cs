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
        Instantiate(MuzzleFlash, MuzzleAnchor.position, Quaternion.identity, MuzzleAnchor);
        MuzzleAnchor.GetComponentInChildren<VisualEffect>().Play();
    }
}
