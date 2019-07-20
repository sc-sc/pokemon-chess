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
    public Trainer trainer;

    public void SetTrainer(Trainer trainer)
    {
        this.trainer = trainer;
        nickname.text = trainer.nickname;
        SetLevel(trainer.level);
        SetHp(trainer.hp, trainer.currentHp);

        goChessFieldButton.onClick.AddListener(() =>
        {
            Vector2 cameraPosition = FindObjectOfType<GameManager>().chessBoards[trainer].transform.position;
            Camera.main.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, Camera.main.transform.position.z);
        });
    }

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

    public void UpdateInfo()
    {
        SetTrainer(trainer);
    }
}
