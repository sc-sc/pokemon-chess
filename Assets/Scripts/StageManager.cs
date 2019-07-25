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

    public AudioClip finalBattleBgm;
    public AudioClip wildBattleBgm;
    public AudioClip trainerBattleBgm;
    public AudioClip gymLeaderBattleBgm;

    [System.Serializable]
    public struct Stages
    {
        [SerializeField] public int mainStage;
        [SerializeField] public Stage[] subStages;
    }

    public Stages[] stages;
    

    void Start()
    {
        mainStage = 1;
        subStage = 1;
    }
    public void ReadyNextStage()
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

        Stage stage = stages[mainStage - 1].subStages[subStage - 1];
        switch (stage.stageType)
        {
            case StageType.Wild:
                gameManager.PlayBgm(wildBattleBgm);
                break;

            case StageType.GymLeader:
                gameManager.PlayBgm(gymLeaderBattleBgm);
                break;

            default:
                gameManager.PlayBgm(trainerBattleBgm);
                break;
        }

        foreach (Trainer trainer in gameManager.trainers)
        {
            Stage currentStage = Instantiate(stage).GetComponent<Stage>();
            currentStages.Add(currentStage);
            FindObjectOfType<BattleManager>().ReadyBattle(trainer, currentStage);
        }
    }

    public void EndStage()
    {
        SafariManager.Refresh();

        subStage += 1;
        if (subStage > stages[mainStage - 1].subStages.Length)
        {
            mainStage += 1;
            subStage = 1;
        }

        foreach (Stage currentStage in currentStages)
        {
            currentStage.DestroySelf();
        }
    }
}
