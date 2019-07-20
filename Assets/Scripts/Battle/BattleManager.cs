using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool isInBattle = false;
    private GameManager gameManager;
    private List<ChessBoard> homeChessBoards;
    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        homeChessBoards = new List<ChessBoard>();
    }
    public void ReadyBattle(Trainer home, Trainer away)
    {
        isInBattle = true;
        gameManager.chessBoards[home].ReadyBattle(away);
        homeChessBoards.Add(gameManager.chessBoards[home]);
    }

    public void StartBattle()
    {
        foreach (ChessBoard chessBoard in homeChessBoards)
        {
            chessBoard.StartBattle();
        }
    }
}
