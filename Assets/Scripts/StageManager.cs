using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int Main_stage;
    public int Sub_stage;
    public List<Trainer> Trainers;
    public GameManager GameManager;
    public PokemonSafariManager SafariManager;

    void Start()
    {
        Main_stage = 1;
        Sub_stage = 1;
        Trainers = GameManager.trainers;
    }
    private void FixedUpdate()
    {
        Trainers = GameManager.trainers;
    }

    // Update is called once per frame
    public void Stage_Update()
    {
        Sub_stage += 1;
        if(Sub_stage == 6)
        {
            Main_stage += 1;
            Sub_stage = 1;
        }
        foreach(Trainer trainer in Trainers)
        {
            int plus_money = trainer.money;
            int temp = 0;
            while (plus_money >= 10)
            {
                temp += 1;
                plus_money -= 10;
            }
            trainer.money += temp;
            trainer.money += 5;
            trainer.exp_present += 2;
            if (trainer.exp_present >= trainer.exp_expect)
            {
                SafariManager.Level_Up(trainer);
            }
        }
        //Find_Next_Stage_Enemy();
        //FindObjectOfType<BattleManager>().ReadyBattle(Trainers[0], Trainers[1]);
    }
    public void Find_Next_Stage_Enemy()
    {
        int Temp_Number = Trainers.Count;
        int Next_Stage_Enemy = Random.Range(0, Temp_Number);
        Debug.Log("다음 싸울 적은" + Trainers[Next_Stage_Enemy]);
        //FindObjectOfType<BattleManager>().ReadyBattle(Trainers[0], Trainers[1]);
    }
    public void Start_Button()
    {
        //FindObjectOfType<BattleManager>().StartBattle();
    }
}
