using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public bool isInBattle = false;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
    public void ReadyBattle(Trainer home, Trainer away)
    {
        isInBattle = true;
        gameManager.chessBoards[home].ReadyBattle(away);
    }
}
