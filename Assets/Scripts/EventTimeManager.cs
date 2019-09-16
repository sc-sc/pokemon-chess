    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTimeManager : MonoBehaviour
{
    public enum EventState
    {
        WaitBattle, ReadyBattle, StartBattle
    }

    public Text timeText;
    public Text eventStateText;

    private float time;
    private int timeFrame;
    public StageManager stageManager;
    public BattleManager battleManager;
    private EventState currentEventState;

    private bool timeCanGo = true;
    private void Awake()
    {
        WaitBattle();
    }

    private void Update()
    {
        if (timeCanGo)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                switch (currentEventState)
                {
                    case EventState.WaitBattle:
                        ReadyBattle();
                        break;
                    case EventState.ReadyBattle:
                        StartBattle();
                        break;
                    case EventState.StartBattle:
                        BattleTimeEnd();
                        break;
                }
            }
            timeText.text = Mathf.Ceil(time).ToString();
        }
    }

    private void ReadyBattle()
    {
        stageManager.ReadyNextStage();
        currentEventState = EventState.ReadyBattle;
        time = 5f;
        eventStateText.text = currentEventState.ToString();
    }

    private void StartBattle()
    {
        battleManager.StartBattle();
        currentEventState = EventState.StartBattle;
        time = 40f;
        eventStateText.text = currentEventState.ToString();
    }

    private void BattleTimeEnd()
    {
        timeCanGo = false;
        battleManager.BattleTimeEnd();
    }
    public void WaitBattle()
    {
        timeCanGo = true;
        currentEventState = EventState.WaitBattle;
        time = 30;
        eventStateText.text = currentEventState.ToString();
    }
    public void Skip()
    {
        time = 0f;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Play(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
