using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManagment : MonoBehaviour
{

    public GameObject[] tetro1;
    public GameObject[] tetro2;
    public GameObject[] flockMen;
    private int gameObjectActived;

    public static TableManagment instance;
    public GameObject table;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else instance = this;
    }
    public void ActiavetTetro()
    {
        tetro1[gameObjectActived].SetActive(true);
        tetro2[gameObjectActived].SetActive(true);
        flockMen[gameObjectActived].SetActive(true);

        if (gameObjectActived < flockMen.Length - 1)
            gameObjectActived++;
    }


}
