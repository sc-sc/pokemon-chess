using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool isInBattle = false;
    public AudioClip finalBattleBgm;
    private GameManager gameManager;
    private List<ChessBoard> homeChessBoards;
    public StageManager stageManager;

    private List<Trainer> winners;
    private List<Trainer> losers;

    private int finishBattleCount;

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
        if (gameManager.trainers.Count == 2)
        {
            gameManager.PlayBgm(finalBattleBgm);
        }

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

        if (finishBattleCount == homeChessBoards.Count)
        {
            EndBattle();
        }
    }

    public void EndBattle()
    {   
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
            if (!(trainer is Stage))
            {
                gameManager.chessBoards[trainer].ResetBoard();
            }
        }

        foreach (ChessBoard homeChessBoard in homeChessBoards)
        {
            homeChessBoard.EndBattle();
        }
        
        homeChessBoards = new List<ChessBoard>();
        winners = new List<Trainer>();
        losers = new List<Trainer>();
        stageManager.EndStage();
    }
}
