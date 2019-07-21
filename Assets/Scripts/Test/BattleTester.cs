using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTester : MonoBehaviour
{
    public Pokemon[] playerPokemons;
    public Pokemon[] challengerPokemons;
    void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        Trainer player = gameManager.trainers[0];
        Trainer challenger = gameManager.trainers[1];

        player.level = playerPokemons.Length;
        challenger.level = challengerPokemons.Length;

        for (int i = 0; i < playerPokemons.Length; i++)
        {
            Pokemon playerPokemon = Instantiate(playerPokemons[i]);
            playerPokemon.trainer = player;
            gameManager.chessBoards[player].PlacePokemon(new Vector2Int(i, 0), playerPokemon);
        }

        for (int i = 0; i < challengerPokemons.Length; i++)
        {
            Pokemon challengerPokemon = Instantiate(challengerPokemons[i]);
            challengerPokemon.trainer = challenger;
            gameManager.chessBoards[challenger].PlacePokemon(new Vector2Int(i, 0), challengerPokemon);
        }

        FindObjectOfType<BattleManager>().ReadyBattle(player, challenger);
        FindObjectOfType<BattleManager>().StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
