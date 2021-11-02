using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMouvement : MonoBehaviour
{

    public Transform _transform;
    public Transform playerTransform;


    // Update is called once per frame
    void Update()
    {
        _transform.rotation = Quaternion.LookRotation(-playerTransform.position, Vector3.up);
    }
}
