using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleExecutor : MonoBehaviour
{
    private Pokemon[,] pokemonsInBattle;
    private ChessBoard chessBoard;
    private float moveTimer;
    public float moveTime = 1f;
    private Trainer challenger;

    private Dictionary<Pokemon, Vector2Int> liveOwnerPokemons;
    private Dictionary<Pokemon, Vector2Int> liveChallengerPokemons;

    private BattleCallbackHandler callbackHandler;
    private IEnumerator battleCoroutine;

    private Trainer winner;
    void Awake()
    {
        callbackHandler = GetComponent<BattleCallbackHandler>();
        chessBoard = GetComponent<ChessBoard>();
    }
    public void ReadyBattle(Trainer challenger)
    {
        pokemonsInBattle = new Pokemon[8, 8];

        liveOwnerPokemons = new Dictionary<Pokemon, Vector2Int>(chessBoard.owner.placedPokemons);
        this.challenger = challenger;
        liveChallengerPokemons = new Dictionary<Pokemon, Vector2Int>();
        ReadyPokemons(chessBoard.owner);
        ReadyPokemons(challenger);
    }

    private void ReadyPokemons(Trainer trainer)
    {
        winner = null;

        foreach (KeyValuePair<Pokemon, Vector2Int> pokemonAndIndex in trainer.placedPokemons)
        {
            Pokemon pokemon = pokemonAndIndex.Key;
            pokemon.currentState = PokemonState.Move;
            pokemon.battleCallbackHandler = callbackHandler;
            pokemon.isAlive = true;

            Vector2Int index = pokemonAndIndex.Value;
            if (trainer == challenger)
            {
                index = new Vector2Int(7, 7) - index;
                liveChallengerPokemons[pokemon] = index;
                pokemon.transform.position = chessBoard.IndexToWorldPosition(index);
            }
            pokemonsInBattle[index.x, index.y] = pokemon;
        }
    }

    public void StartBattle()
    {
        SetAttackTarget();
        battleCoroutine = BattleCoroutine();

        StartCoroutine(battleCoroutine);
    }

    private IEnumerator BattleCoroutine()
    {
        yield return new WaitForSeconds(1f);

        battleCoroutine = BattleCoroutine();
        StartCoroutine(battleCoroutine);
    }
    private void SetAttackTarget()
    {
        SetPokemonsAttackTarget(liveOwnerPokemons, liveChallengerPokemons);
        SetPokemonsAttackTarget(liveChallengerPokemons, liveOwnerPokemons);
    }

    private void SetPokemonsAttackTarget(Dictionary<Pokemon, Vector2Int> attackPokemons, Dictionary<Pokemon, Vector2Int> targetPokemons)
    {
        foreach (KeyValuePair<Pokemon, Vector2Int> attackPokemonAndIndex in attackPokemons)
        {
            SetAttackTargetTo(attackPokemonAndIndex, targetPokemons);
        }
    }

    public void SetAttackTargetTo(KeyValuePair<Pokemon, Vector2Int> attackPokemonAndIndex, Dictionary<Pokemon, Vector2Int> targetPokemons)
    {
        Pokemon attackPokemon = attackPokemonAndIndex.Key;
        if (attackPokemon.currentState == PokemonState.Attack) return;

        Vector2Int index = attackPokemonAndIndex.Value;

        List<Pokemon> targetPokemonList = new List<Pokemon>(targetPokemons.Keys);
        List<Vector2Int> targetIndexList = new List<Vector2Int>(targetPokemons.Values);

        Pokemon attackTarget = targetPokemonList[0];
        float minDistance = Vector2Int.Distance(index, targetIndexList[0]);

        for (int i = 1; i < targetPokemons.Count; i++)
        {
            float distance = Vector2Int.Distance(index, targetIndexList[i]);

            if (distance < minDistance)
            {
                minDistance = distance;
                attackTarget = targetPokemonList[i];
            }
        }

        attackPokemon.attackTarget = attackTarget;
    }
    public void PokemonDead(Pokemon pokemon)
    {
        if (liveChallengerPokemons.ContainsKey(pokemon))
        {
            liveChallengerPokemons.Remove(pokemon);
            if (liveChallengerPokemons.Count == 0)
            {
                Victory(chessBoard.owner);
            }
        } else
        {
            liveOwnerPokemons.Remove(pokemon);
            if (liveOwnerPokemons.Count == 0)
            {
                Victory(challenger);
            }
        }
    }

    private void Victory(Trainer trainer)
    {
        winner = trainer;

        foreach (Pokemon pokemon in trainer.placedPokemons.Keys)
        {
            pokemon.currentState = PokemonState.Idle;
        }
    }

    public void SetAttackTargetTo(Pokemon attackPokemon)
    {
        if (liveOwnerPokemons.ContainsKey(attackPokemon))
        {
            KeyValuePair<Pokemon, Vector2Int> attackPokemonAndIndex = new KeyValuePair<Pokemon, Vector2Int>(attackPokemon, liveOwnerPokemons[attackPokemon]);
            SetAttackTargetTo(attackPokemonAndIndex, liveChallengerPokemons);
        } else
        {
            KeyValuePair<Pokemon, Vector2Int> attackPokemonAndIndex = new KeyValuePair<Pokemon, Vector2Int>(attackPokemon, liveChallengerPokemons[attackPokemon]);
            SetAttackTargetTo(attackPokemonAndIndex, liveOwnerPokemons);
        }
    }
}
