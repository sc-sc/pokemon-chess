using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timeText;
    private float time;
    private bool toggle_key;
    public StageManager StageManager;
    public BattleManager BattleManager;

    private void Awake()
    {
        toggle_key = false;
        time = 20f;
    }


    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            toggle_key = !toggle_key;
            Start_End(toggle_key);
        }
        timeText.text = Mathf.Ceil(time).ToString();
    }

    private void Start_End(bool toggle)
    {
        if (toggle)
        {
            time = 30f;
            Debug.Log("Battle Start");
            StageManager.ReadyNextStage();
            BattleManager.StartBattle();
        }
        else
        {
            time = 20f;
            Debug.Log("Battle Wait");
        }
    }
    public void Reset_Time_Battle_Wait()
    {
        Debug.Log("확인해보자.");
        toggle_key = false;
        time = 20f;
    }
    public void Reset_Time_Battle_Start()
    {
        toggle_key = true;
        time = 30f;
    }
}
