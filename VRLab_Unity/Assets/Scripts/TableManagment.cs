using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TableManagment : NetworkBehaviour
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
        initPos = _transform.localPosition;
        _transform.localPosition = tableFollow.localPosition + new Vector3(initPos.x,0,initPos.z);
    }
/*    private void Update()
    {
        _transform.position = tableFollow.position+initPos;
    }
*/
    public GameObject[] ActiavetTetro()
    {
        tetro1[gameObjectActived].SetActive(true);
        tetro2[gameObjectActived].SetActive(true);
        flockMen[gameObjectActived].SetActive(true);

        GameObject[] objectToSpawn = new GameObject[3];
        objectToSpawn[0] = tetro1[gameObjectActived];
        objectToSpawn[1] = tetro2[gameObjectActived];
        objectToSpawn[2] = flockMen[gameObjectActived];
        return objectToSpawn;
        if (gameObjectActived < flockMen.Length - 1)
            gameObjectActived++;
    }


}
