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

    private AudioSource audioSource;
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
            chessFieldInstance.transform.position = new Vector3(Mathf.Floor(position.x), Mathf.Floor(position.y));
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
        audioSource = GetComponent<AudioSource>();
        StartNewGame();
    }

    public void PlayBgm(AudioClip bgm)
    {
        StartCoroutine(FadeOutPreviousBgmAndPlay(bgm));
    }

    private IEnumerator FadeOutPreviousBgmAndPlay(AudioClip bgm)
    {
        float fadeTime = 0.5f;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
        audioSource.Stop();
        audioSource.PlayOneShot(bgm);
        audioSource.loop = true;
    }
}
