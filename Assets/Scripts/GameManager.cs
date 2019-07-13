using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Trainer> trainers;
    public GameObject chessField;
    public PokemonStore pokemonStore;
    public Dictionary<Trainer, ChessBoard> chessBoards;
    public Dictionary<Trainer, WaitingBoard> waitingBoards;
    public void StartNewGame()
    {
        chessBoards = new Dictionary<Trainer, ChessBoard>();
        waitingBoards = new Dictionary<Trainer, WaitingBoard>();

        for (int i = 0; i < trainers.Count; i++)
        {
            Trainer trainer = trainers[i];

            GameObject chessFieldInstance = Instantiate(chessField);
            chessFieldInstance.transform.position = Quaternion.Euler(0f, 0f, 15f + 60f * i) * new Vector3(30f, 30f);
            ChessBoard chessBoard = chessFieldInstance.GetComponentInChildren<ChessBoard>();
            WaitingBoard waitingBoard = chessFieldInstance.GetComponentInChildren<WaitingBoard>();
            chessBoard.owner = trainer;
            waitingBoard.owner = trainer;

            chessBoards[trainer] = chessBoard;
            waitingBoards[trainer] = waitingBoard;

            if (trainer is Player)
            {
                Camera.main.transform.position = new Vector3(chessFieldInstance.transform.position.x, chessFieldInstance.transform.position.y, -10f);
            }
        }

        pokemonStore.Refresh();
    }

    void Start()
    {
        StartNewGame();
    }
}
