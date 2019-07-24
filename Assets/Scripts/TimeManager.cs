using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timeText;
    private float time;

    private void Awake()
    {
        time = 30f;
    }


    private void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
        }

        timeText.text = Mathf.Ceil(time).ToString();
    }



}
