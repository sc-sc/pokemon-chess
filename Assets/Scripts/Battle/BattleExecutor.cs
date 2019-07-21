using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BattleExecutor : MonoBehaviour
{
    public bool isInBattle = false;

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
    
    private enum MoveDirection
    {
        Up, Down, Left, Right, None
    }

    private Dictionary<Pokemon, MoveDirection> pokemonPreviousMove;

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
        pokemonPreviousMove = new Dictionary<Pokemon, MoveDirection>();
        ReadyPokemons(chessBoard.owner);
        ReadyPokemons(challenger);

        SetAttackTarget();
    }

    private void ReadyPokemons(Trainer trainer)
    {
        isInBattle = true;
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
            pokemonPreviousMove[pokemon] = MoveDirection.None;
        }
    }
    public void StartBattle()
    {
        battleCoroutine = BattleCoroutine();

        StartCoroutine(battleCoroutine);
    }

    private IEnumerator BattleCoroutine()
    {
        yield return new WaitForSeconds(1f);
        MovePokemons(chessBoard.owner);
        MovePokemons(challenger);
        battleCoroutine = BattleCoroutine();
        StartCoroutine(battleCoroutine);
    }
    private void SetAttackTarget()
    {
        SetPokemonsAttackTarget(liveOwnerPokemons, liveChallengerPokemons);
        SetPokemonsAttackTarget(liveChallengerPokemons, liveOwnerPokemons);
    }

    private void MovePokemons(Trainer trainer)
    {
        var attackPokemonsAndIndexes = trainer == chessBoard.owner ?
            liveOwnerPokemons.OrderByDescending(pokemonAndIndex => pokemonAndIndex.Value.magnitude) :
            liveChallengerPokemons.OrderBy(pokemonAndIndex => pokemonAndIndex.Value.magnitude);

        var targetPokemonsAndIndex = trainer == chessBoard.owner ?
            liveChallengerPokemons :
            liveOwnerPokemons;


        foreach (KeyValuePair<Pokemon, Vector2Int> attackPokemonAndIndex in attackPokemonsAndIndexes)
        {
            Pokemon attackPokemon = attackPokemonAndIndex.Key;
            if (attackPokemon.currentState == PokemonState.Attack || !attackPokemon.attackTarget.isAlive)
            {
                pokemonPreviousMove[attackPokemon] = MoveDirection.None;
                continue;
            }

            Vector2Int index = attackPokemonAndIndex.Value;

            Vector2Int targetIndex = targetPokemonsAndIndex[attackPokemon.attackTarget];

            if (attackPokemon.transform.position.x > attackPokemon.attackTarget.transform.position.x)
            {
                attackPokemon.spriteRenderer.flipX = false;
            } else
            {
                attackPokemon.spriteRenderer.flipX = true;
            }

            Vector2Int distance = targetIndex - index;

            MoveDirection moveDirection;
            switch (pokemonPreviousMove[attackPokemon])
            {
                case MoveDirection.Up:
                    moveDirection = CalculateMoveDirection(index, distance, canGoDown: false);
                    break;
                case MoveDirection.Down:
                    moveDirection = CalculateMoveDirection(index, distance, canGoUp: false);
                    break;
                case MoveDirection.Left:
                    moveDirection = CalculateMoveDirection(index, distance, canGoRight: false);
                    break;
                case MoveDirection.Right:
                    moveDirection = CalculateMoveDirection(index, distance, canGoLeft: false);
                    break;
                default:
                    moveDirection = CalculateMoveDirection(index, distance);
                    break;
            }

            pokemonsInBattle[index.x, index.y] = null;

            Vector2Int moveTo = index;
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    moveTo += new Vector2Int(0, 1);
                    break;
                case MoveDirection.Down:
                    moveTo += new Vector2Int(0, -1);
                    break;
                case MoveDirection.Right:
                    moveTo += new Vector2Int(1, 0);
                    break;
                case MoveDirection.Left:
                    moveTo += new Vector2Int(-1, 0);
                    break;
                default:
                    break;
            }

            pokemonPreviousMove[attackPokemon] = moveDirection;
            if (trainer == chessBoard.owner)
            {
                liveOwnerPokemons[attackPokemon] = moveTo;
            } else
            {
                liveChallengerPokemons[attackPokemon] = moveTo;
            }
            pokemonsInBattle[moveTo.x, moveTo.y] = attackPokemon;
            attackPokemon.MoveTo(chessBoard.IndexToWorldPosition(moveTo));
        }
    }

    private MoveDirection CalculateMoveDirection(Vector2Int index, Vector2Int distance, bool canGoUp = true, bool canGoDown = true, bool canGoRight = true, bool canGoLeft = true)
    {
        MoveDirection moveDirection = MoveDirection.None;
        Vector2Int moveTo = index;

        Vector2Int absDistance = new Vector2Int(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

        Debug.Log(absDistance);

        if (absDistance.x > absDistance.y && (canGoRight || canGoLeft))
        {
            if (distance.x > 0)
            {
                moveTo = index + new Vector2Int(1, 0);
                moveDirection = MoveDirection.Right;
                if (moveTo.x > 7 || IsAnotherPokemonAlreadyExist(moveTo) || !canGoRight)
                    return CalculateMoveDirection(index, new Vector2Int(0, distance.y), canGoUp, canGoDown, false, canGoLeft);
            } else
            {
                moveTo = index + new Vector2Int(-1, 0);
                moveDirection = MoveDirection.Left;
                if (moveTo.x < 0 || IsAnotherPokemonAlreadyExist(moveTo) || !canGoLeft)
                    return CalculateMoveDirection(index, new Vector2Int(0, distance.y), canGoUp, canGoDown, canGoRight, false);
            }
        } else if (canGoUp || canGoDown)
        {
            if (distance.y > 0)
            {
                moveTo = index + new Vector2Int(0, 1);
                moveDirection = MoveDirection.Up;
                if (moveTo.y > 7 || IsAnotherPokemonAlreadyExist(moveTo) || !canGoUp)
                    return CalculateMoveDirection(index, new Vector2Int(distance.x, 0), false, canGoDown, canGoRight, canGoLeft);
            } else if (canGoDown)
            {
                moveTo = index + new Vector2Int(0, -1);
                moveDirection = MoveDirection.Down;
                if (moveTo.y < 0 || IsAnotherPokemonAlreadyExist(moveTo))
                    return CalculateMoveDirection(index, new Vector2Int(distance.x, 0), canGoUp, false, canGoRight, canGoLeft);
            }
        }

        return moveDirection;
    }

    private bool IsAnotherPokemonAlreadyExist(Vector2Int index)
    {
        return pokemonsInBattle[index.x, index.y] != null;
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
        Vector2Int index;
        if (liveChallengerPokemons.ContainsKey(pokemon))
        {
            index = liveChallengerPokemons[pokemon];

            liveChallengerPokemons.Remove(pokemon);

            if (liveChallengerPokemons.Count == 0)
            {
                Victory(chessBoard.owner);
            }
        } else
        {
            index = liveOwnerPokemons[pokemon];
            
            liveOwnerPokemons.Remove(pokemon);
            if (liveOwnerPokemons.Count == 0)
            {
                Victory(challenger);
            }
        }
        pokemonsInBattle[index.x, index.y] = null;
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

    public Pokemon GetPokemonInBattle(Vector2Int index)
    {
        return pokemonsInBattle[index.x, index.y];
    }
}
