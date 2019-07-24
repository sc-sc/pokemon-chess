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

    private int finishBattleCount;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        homeChessBoards = new List<ChessBoard>();
    }
    public void ReadyBattle(Trainer home, Trainer away)
    {
        if (gameManager.trainers.Count == 2)
        {
            gameManager.PlayBgm(finalBattleBgm);
        }

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
        Debug.Log("게임 끝남" + finishBattleCount);

        if (finishBattleCount == homeChessBoards.Count)
        {
            EndBattle();
        }
    }

    private void EndBattle()
    {   
        isInBattle = false;
        stageManager.Stage_Update();
        foreach (ChessBoard homeChessBoard in homeChessBoards)
        {
            homeChessBoard.EndBattle();
        }
        
        homeChessBoards = new List<ChessBoard>();
    }
}
