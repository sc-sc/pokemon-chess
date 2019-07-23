using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int Main_stage;
    public int Sub_stage;
    public bool Start_End;
    public List<Trainer> Trainers;
    public GameManager gameManager;
    public PokemonSafariManager SafariManager;

    public GameObject[] pokemonsInStage;

    [System.Serializable]
    public struct Stages
    {
        [SerializeField] public int mainStage;
        [SerializeField] public Stage[] subStages;
    }

    public Stages[] stages;
    

    private void Awake()
    {
    }

    void Start()
    {
        Main_stage = 1;
        Sub_stage = 1;
        Start_End = true;
        Trainers = gameManager.trainers;
    }
    private void FixedUpdate()
    {
        Trainers = gameManager.trainers;
    }

    // Update is called once per frame
    public void Stage_Update()
    {
        Start_End = !Start_End;
        if (Start_End)
        {
            SafariManager.Refresh();
            Sub_stage += 1;
            if (Sub_stage == 6)
            {
                Main_stage += 1;
                Sub_stage = 1;
            }
            foreach (Trainer trainer in Trainers)
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
            Debug.Log("Battle End!!");
        }
        else
        {
            Debug.Log("Battle Start!!");
            Find_Next_Stage_Enemy();
        }
    }
    public void Find_Next_Stage_Enemy()
    {
        /*
        int Temp_Number = Trainers.Count;
        Debug.Log(Temp_Number);
        int Next_Stage_Enemy = Random.Range(0, Temp_Number);
        if(Next_Stage_Enemy == 0)
        {
            Find_Next_Stage_Enemy();
            return;
        }
        Debug.Log("다음 싸울 적은" + Trainers[Next_Stage_Enemy]);
        int temp_count = Trainers[Next_Stage_Enemy].placedPokemons.Count;
        if (temp_count == 0)
        {
            Trainers[Next_Stage_Enemy].currentHp -= 5;
            Stage_Update();
            return;
        }
        */
        FindObjectOfType<BattleManager>().ReadyBattle(gameManager.trainers[0], stages[0].subStages[0]);
        FindObjectOfType<BattleManager>().StartBattle();
    }
}
