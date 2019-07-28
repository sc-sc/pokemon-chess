using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool isInBattle = false;
    private GameManager gameManager;
    private List<ChessBoard> homeChessBoards;
    public StageManager stageManager;

    private List<Trainer> winners;
    private List<Trainer> losers;

    private int finishBattleCount;

    public EventTimeManager timeManager;

    public AudioClip wildVictoryBgm;
    public AudioClip trainerVictoryBgm;
    public AudioClip gymLeaderVictoryBgm;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        homeChessBoards = new List<ChessBoard>();
        winners = new List<Trainer>();
        losers = new List<Trainer>();
    }
    public void ReadyBattle(Trainer home, Trainer away)
    {
        isInBattle = true;
        gameManager.chessBoards[home].ReadyBattle(away);
        homeChessBoards.Add(gameManager.chessBoards[home]);
    }

    public void StartBattle()
    {
        finishBattleCount = 0;
        foreach (ChessBoard chessBoard in homeChessBoards)
        {
            chessBoard.StartBattle();
        }
    }

    public void FinishBattleIn(ChessBoard homeChessBoard, Trainer winner, Trainer loser)
    {
        finishBattleCount++;
        winners.Add(winner);
        losers.Add(loser);
        winner.status = 1;
        loser.status = 0;

        if (winner is Player && loser is Stage)
        {
            Stage stage = loser as Stage;

            switch (stage.stageType)
            {
                case StageType.Wild:
                    gameManager.PlayBgm(wildVictoryBgm);
                    break;

                case StageType.GymLeader:
                    gameManager.PlayBgm(gymLeaderVictoryBgm);
                    break;

                default:
                    gameManager.PlayBgm(trainerVictoryBgm);
                    break;
            }
        }

        if (finishBattleCount == homeChessBoards.Count)
        {
            EndBattle();
        }
    }

    public void DrawBattleIn(ChessBoard homeChessBoard, Trainer home, Trainer away)
    {
        finishBattleCount++;
        losers.Add(home);
        losers.Add(away);
        home.status = 0;
        away.status = 0;

        if (finishBattleCount == homeChessBoards.Count)
        {
            EndBattle();
        }
    }
    public void BattleTimeEnd()
    {
        foreach (ChessBoard homeChessBoard in homeChessBoards)
        {
            homeChessBoard.BattleTimeEnd();
        }
    }
    private void EndBattle()
    {   
        StartCoroutine(BattleEndAction());
    }

    private IEnumerator BattleEndAction()
    {
        yield return new WaitForSeconds(0.2f);

        isInBattle = false;
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
            trainer.ExpUp(2);
            if(trainer.status == 1)
            {
                trainer.money += 1;
                trainer.count_win += 1;
                trainer.count_lose = 0;
            }
            else
            {
                trainer.count_lose += 1;
                trainer.count_win = 0;
            }

            if(trainer.count_win > 1 && trainer.count_win <= 4)
            {
                trainer.money += 1;
            }
            else if(trainer.count_win > 4 && trainer.count_win <= 7)
            {
                trainer.money += 2;
            }
            else if (trainer.count_win > 7)
            {
                trainer.money += 3;
            }

            if (trainer.count_lose > 1 && trainer.count_lose <= 4)
            {
                trainer.money += 1;
            }
            else if (trainer.count_lose > 4 && trainer.count_lose <= 7)
            {
                trainer.money += 2;
            }
            else if (trainer.count_lose > 7)
            {
                trainer.money += 3;
            }

            if (!(trainer is Stage))
            {
                gameManager.chessBoards[trainer].ResetBoard();
            }
        }

        foreach (ChessBoard homeChessBoard in homeChessBoards)
        {
            homeChessBoard.EndBattle();
        }


        stageManager.EndStage();

        homeChessBoards = new List<ChessBoard>();
        winners = new List<Trainer>();
        losers = new List<Trainer>();

        timeManager.WaitBattle();
    }
}
