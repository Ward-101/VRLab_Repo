using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManagment : MonoBehaviour
{

    public GameObject[] tetro1;
    public GameObject[] tetro2;
    public GameObject[] flockMen;
    private int gameObjectActived;
    private Vector3 initPos;
    public Transform tableFollow;
    public Transform _transform;
    private void Start()
    {
        initPos = _transform.position;
    }
/*    private void Update()
    {
        _transform.position = tableFollow.position+initPos;
    }
*/

    public void ActiavetTetro()
    {
        tetro1[gameObjectActived].SetActive(true);
        tetro2[gameObjectActived].SetActive(true);
        flockMen[gameObjectActived].SetActive(true);

        if (gameObjectActived < flockMen.Length - 1)
            gameObjectActived++;
    }


}
