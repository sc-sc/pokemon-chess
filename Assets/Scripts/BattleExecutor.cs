using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleExecutor : MonoBehaviour
{
    private Pokemon[,] pokemonsInBattle;
    private ChessBoard chessBoard;

    void Awake()
    {
        chessBoard = GetComponent<ChessBoard>();
    }
    public void ReadyBattle(Trainer challenger)
    {
        pokemonsInBattle = new Pokemon[8, 8];

        foreach (KeyValuePair<Pokemon, Vector2Int> pokemonAndIndex in chessBoard.owner.placedPokemons)
        {
            Vector2Int index = pokemonAndIndex.Value;
            pokemonsInBattle[index.x, index.y] = pokemonAndIndex.Key;
        }

        foreach (KeyValuePair<Pokemon, Vector2Int> challengerPokemonAndIndex in challenger.placedPokemons)
        {
            Vector2Int index = new Vector2Int(7, 7) - challengerPokemonAndIndex.Value;
            Pokemon challengerPokemon = challengerPokemonAndIndex.Key;
            pokemonsInBattle[index.x, index.y] = challengerPokemon;
            challengerPokemon.transform.position = chessBoard.IndexToWorldPosition(index);
        }
    }
}
