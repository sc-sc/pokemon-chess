using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Trainer> trainers;
    public GameObject chessField;
    public PokemonSafariManager pokemonSafari;
    public Dictionary<Trainer, ChessBoard> chessBoards;
    public Dictionary<Trainer, WaitingBoard> waitingBoards;
    public GameObject lapras;
    public BattleManager battleManager;
    public void StartNewGame()
    {
        chessBoards = new Dictionary<Trainer, ChessBoard>();
        waitingBoards = new Dictionary<Trainer, WaitingBoard>();

        float angle = 360 / trainers.Count;

        for (int i = 0; i < trainers.Count; i++)
        {
            Trainer trainer = trainers[i];

            GameObject chessFieldInstance = Instantiate(chessField);
            Vector3 position = Quaternion.Euler(0f, 0f, angle / 4f + angle * i) * new Vector3(30f, 30f);
            chessFieldInstance.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y));
            ChessBoard chessBoard = chessFieldInstance.GetComponentInChildren<ChessBoard>();
            WaitingBoard waitingBoard = chessFieldInstance.GetComponentInChildren<WaitingBoard>();
            chessBoard.owner = trainer;
            waitingBoard.owner = trainer;

            chessBoards[trainer] = chessBoard;
            waitingBoards[trainer] = waitingBoard;

            if (trainer is Player)
            {
                Camera.main.transform.position = new Vector3(chessFieldInstance.transform.position.x, chessFieldInstance.transform.position.y, -10f);
                lapras.transform.position = chessFieldInstance.transform.position + new Vector3(-12f, 0.5f);
            } else
            {
                // GameObject trainerObject = Instantiate(trainer.gameObject);
                // trainerObject.transform.position = chessFieldInstance.transform.position;
            }
        }

        pokemonSafari.Refresh();
    }

    void Awake()
    {
        StartNewGame();
    }
}
