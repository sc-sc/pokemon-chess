using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public GameManager gm;
    public GameObject layout;


    // Start is called before the first frame update
    void Start()
    {

        int num_player = gm.trainers.Count;

        Debug.Log(num_player);


        for (int i = 0; i < num_player; i++)
        {
            Instantiate(Resources.Load("Prefabs/AnotherPlayer") as GameObject, layout.transform);
        }

    }
 }
