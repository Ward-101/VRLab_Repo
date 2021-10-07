using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SCR_SimpleGun : MonoBehaviour
{
    public float shotRange;
    public float shotWidth;

    public Transform MuzzleAnchor;
    public GameObject MuzzleFlash;
    public LayerMask shotableLayer;

    public GameObject bulletDecal;
    public AudioClip fireSFX;

    private AudioSource gunAudioSource;

    public void Start()
    {
        gunAudioSource = GetComponent<AudioSource>();
    }

    public void Fire()
    {
        if (Physics.SphereCast(MuzzleAnchor.position, shotWidth, transform.forward, out RaycastHit hitInfo ,shotableLayer))
        {
            Instantiate(bulletDecal, hitInfo.transform.position, Quaternion.Euler(hitInfo.normal));

            //Add a sound that depend on the surface hit
        }

        gunAudioSource.PlayOneShot(fireSFX);
        MuzzleFlash.GetComponentInChildren<VisualEffect>().Play();
    }
}
