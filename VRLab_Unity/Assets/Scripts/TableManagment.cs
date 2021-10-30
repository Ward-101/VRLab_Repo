using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TableManagment : MonoBehaviour
{
    public GameObject[] pieces;
    private int gameObjectActived;
    private Vector3 initPos;
    public Transform tableFollow;
    public Transform _transform;
    private void Start()
    {
        initPos = _transform.localPosition;
    }
/*    private void Update()
    {
        _transform.position = tableFollow.position+initPos;
    }
*/
    public GameObject[] ActiavetTetro()
    {
        return pieces;
    }


}
