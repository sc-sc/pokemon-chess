using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainerInfo : MonoBehaviour
{
    public Button goChessFieldButton;
    public Text nickname;
    [SerializeField]
    private Text level;
    [SerializeField]
    private Image hpGauge;
    public Image Icon;

    public void SetHp(int hp, int currentHp)
    {
        hpGauge.fillAmount = (float) currentHp / hp;
    }

    public void SetLevel(int level)
    {
        this.level.text = "Lv." + level.ToString();
    }

    public void SetHpColor(Color color)
    {
        hpGauge.color = color;
    }
}
