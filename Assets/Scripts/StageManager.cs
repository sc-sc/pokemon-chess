using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int mainStage;
    public int subStage;
    public bool Start_End;
    public GameManager gameManager;
    public PokemonSafariManager SafariManager;

    public GameObject[] pokemonsInStage;

    private List<Stage> currentStages;

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
        mainStage = 1;
        subStage = 1;
        Start_End = true;
    }

    // Update is called once per frame
    public void Stage_Update()
    {
        Start_End = !Start_End;
        if (Start_End)
        {
            SafariManager.Refresh();
            subStage += 1;
            if (subStage > stages[mainStage - 1].subStages.Length)
            {
                mainStage += 1;
                subStage = 1;
            }
            foreach (Trainer trainer in gameManager.trainers)
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

            foreach (Stage currentStage in currentStages)
            {
                currentStage.DestroySelf();
            }
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
        currentStages = new List<Stage>();

        foreach (Trainer trainer in gameManager.trainers)
        {
            Stage stage = Instantiate(stages[mainStage - 1].subStages[subStage - 1]).GetComponent<Stage>();
            currentStages.Add(stage);
            FindObjectOfType<BattleManager>().ReadyBattle(trainer, stage);
            FindObjectOfType<BattleManager>().StartBattle();
        }
    }
}
