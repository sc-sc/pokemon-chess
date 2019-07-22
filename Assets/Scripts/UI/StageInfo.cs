using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour
{
    public Text stage;
    public StageManager StageManger;

    void Start()
    {
        SetStageInfoText();
    }

    void FixedUpdate()
    {
        SetStageInfoText();
    }

    void SetStageInfoText()
    {
        stage.text = StageManger.Main_stage + " - " + StageManger.Sub_stage;
    }
}
