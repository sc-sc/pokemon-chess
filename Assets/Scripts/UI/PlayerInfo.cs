using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Text playerLevel;
    public Text playerHp;
    public Text playerMoney;
    public Text playerExp;
    public Trainer trainer;
    public Text playerCount;

    void Start()
    {
        SetPlayerInfoText();
    }

    void FixedUpdate()
    {
        SetPlayerInfoText();
    }

    void SetPlayerInfoText()
    {
        //playerNicknme.text = "Nick: " + trainer.nickname;
        playerLevel.text = "Lv: " + trainer.level;
        playerHp.text = "Hp: " + trainer.currentHp;
        playerMoney.text = "Money: " + trainer.money;
        playerCount.text = "w/l: " + trainer.count_win + "/" + trainer.count_lose;
        playerExp.text = "Exp: " + trainer.exp_present + "/" + trainer.exp_expect;
        //player_Bug.text = "Bug: " + trainer.Bug;
        //player_Water.text = "Water: " + trainer.Water;
        //player_Fire.text = "Fire: " + trainer.Fire;
    }
}
