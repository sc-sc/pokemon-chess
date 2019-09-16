using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Unity.Jobs;
using Unity.Collections;
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

    private BattleManager battleManager;
    private enum MoveDirection
    {
        Up, Down, Left, Right, None
    }

    private Dictionary<Pokemon, MoveDirection> pokemonPreviousMove;
    public PokemonUIManager PokemonUIManager;

    void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        callbackHandler = GetComponent<BattleCallbackHandler>();
        chessBoard = GetComponent<ChessBoard>();
    }

    public void ReadyBattle(Trainer challenger)
    {
        isInBattle = true;
        winner = null;

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
        winner = null;
        //Debug.Log(trainer.placedPokemons);
        if(trainer.placedPokemons == null)
        {

        }
        foreach (KeyValuePair<Pokemon, Vector2Int> pokemonAndIndex in trainer.placedPokemons)
        {
            Pokemon pokemon = pokemonAndIndex.Key;
            pokemon.battleCallbackHandler = callbackHandler;
            pokemon.isAlive = true;

            Vector2Int index = pokemonAndIndex.Value;
            if (trainer == challenger)
            {
                index = new Vector2Int(7, 7) - index;
                liveChallengerPokemons[pokemon] = index;
            }

            pokemon.transform.position = chessBoard.IndexToWorldPosition(index);
            pokemonsInBattle[index.x, index.y] = pokemon;
            pokemonPreviousMove[pokemon] = MoveDirection.None;
        }
    }

    public void StartBattle()
    {
        foreach (Pokemon pokemon in chessBoard.owner.placedPokemons.Keys)
        {
            pokemon.currentState = PokemonState.Move;
        }

        foreach (Pokemon pokemon in challenger.placedPokemons.Keys)
        {
            pokemon.currentState = PokemonState.Move;
        }

        battleCoroutine = BattleCoroutine();
        StartCoroutine(battleCoroutine);
    }

    public void EndBattle()
    {
        isInBattle = false;
        StopCoroutine(battleCoroutine);
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

    private struct CalculateMoveDirectionJob : IJobParallelFor
    {
        public NativeArray<MoveDirection> resultMoveDirections;
        [ReadOnly]
        public NativeArray<Vector2Int> pokemonIndexes;
        [ReadOnly]
        public NativeArray<Vector2Int> distancesBetweenTarget;
        [ReadOnly]
        public NativeArray<MoveDirection> previousMoveDirections;

        public NativeArray<Vector2Int> pokemonsInBattleIndexes;

        public void Execute(int index)
        {
            switch (previousMoveDirections[index])
            {
                case MoveDirection.Up:
                    resultMoveDirections[index] = CalculateMoveDirection(pokemonIndexes[index], distancesBetweenTarget[index], canGoDown: false);
                    break;
                case MoveDirection.Down:
                    resultMoveDirections[index] = CalculateMoveDirection(pokemonIndexes[index], distancesBetweenTarget[index], canGoUp: false);
                    break;
                case MoveDirection.Right:
                    resultMoveDirections[index] = CalculateMoveDirection(pokemonIndexes[index], distancesBetweenTarget[index], canGoLeft: false);
                    break;
                case MoveDirection.Left:
                    resultMoveDirections[index] = CalculateMoveDirection(pokemonIndexes[index], distancesBetweenTarget[index], canGoRight: false);
                    break;
                default:
                    resultMoveDirections[index] = CalculateMoveDirection(pokemonIndexes[index], distancesBetweenTarget[index]);
                    break;
            }
        }
        private MoveDirection CalculateMoveDirection(Vector2Int index, Vector2Int distance, bool canGoUp = true, bool canGoDown = true, bool canGoRight = true, bool canGoLeft = true, bool horizontalFirst = false, bool verticalFirst = false, int step = 0)
        {
            MoveDirection moveDirection = MoveDirection.None;
            Vector2Int moveTo = index;

            Vector2Int absDistance = new Vector2Int(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

            bool isVerticalFirst = verticalFirst ? canGoUp || canGoDown : !horizontalFirst && absDistance.y > absDistance.x;

            if (!isVerticalFirst && (canGoRight || canGoLeft))
            {
                if (!canGoLeft || ((step == 0 || canGoRight) && distance.x >= 0))
                {
                    moveTo = index + new Vector2Int(1, 0);
                    moveDirection = MoveDirection.Right;
                    if (!canGoRight || moveTo.x > 7 || IsAnotherPokemonAlreadyExist(moveTo))
                        return CalculateMoveDirection(index, distance, canGoUp, canGoDown, false, canGoLeft, false, true, ++step);
                }
                else if (step == 0 || canGoLeft)
                {
                    moveTo = index + new Vector2Int(-1, 0);
                    moveDirection = MoveDirection.Left;
                    if (!canGoLeft || moveTo.x < 0 || IsAnotherPokemonAlreadyExist(moveTo))
                        return CalculateMoveDirection(index, distance, canGoUp, canGoDown, canGoRight, false, false, true, ++step);
                }
            }
            else if (canGoUp || canGoDown)
            {
                if (!canGoDown || ((step == 0 || canGoUp) && distance.y >= 0))
                {
                    moveTo = index + new Vector2Int(0, 1);
                    moveDirection = MoveDirection.Up;
                    if (!canGoUp || moveTo.y > 7 || IsAnotherPokemonAlreadyExist(moveTo))
                        return CalculateMoveDirection(index, distance, false, canGoDown, canGoRight, canGoLeft, true, false, ++step);

                }
                else if (step == 0 || canGoDown)
                {
                    moveDirection = MoveDirection.Down;
                    moveTo = index + new Vector2Int(0, -1);
                    if (!canGoDown || moveTo.y < 0 || IsAnotherPokemonAlreadyExist(moveTo))
                        return CalculateMoveDirection(index, distance, canGoUp, false, canGoRight, canGoLeft, true, false, ++step);
                }
            }

            if (moveDirection == MoveDirection.None)
            {
                Debug.Log((canGoUp, canGoDown, canGoRight, canGoLeft, isVerticalFirst));
            }

            return moveDirection;
        }

        private bool IsAnotherPokemonAlreadyExist(Vector2Int index)
        {
            return pokemonsInBattleIndexes.Contains(index);
        }
    }
    private void MovePokemons(Trainer trainer)
    {
        List<Vector2Int> pokemonIndexes = new List<Vector2Int>();
        List<Vector2Int> distnacesBetweenTargetPokemon = new List<Vector2Int>();
        List<MoveDirection> previousMoveDirections = new List<MoveDirection>();
        List<Pokemon> toMovePokemons = new List<Pokemon>();

        Dictionary<Pokemon, Vector2Int> toMovePokemonsAndIndexes = trainer == chessBoard.owner ? liveOwnerPokemons : liveChallengerPokemons;
        Dictionary<Pokemon, Vector2Int> targetPokemonsAndIndexes = trainer == chessBoard.owner ? liveChallengerPokemons : liveOwnerPokemons;
        
        foreach (KeyValuePair<Pokemon, Vector2Int> pokemonAndIndex in toMovePokemonsAndIndexes)
        {
            Pokemon pokemon = pokemonAndIndex.Key;
            if (pokemon.currentState != PokemonState.Move || !pokemon.attackTarget.isAlive || pokemon.CheckParalysis())
            {
                pokemonPreviousMove[pokemon] = MoveDirection.None;
                continue;
            }

            Vector2Int index = pokemonAndIndex.Value;
            Vector2Int distance = targetPokemonsAndIndexes[pokemon.attackTarget] - index;

            pokemonIndexes.Add(pokemonAndIndex.Value);
            distnacesBetweenTargetPokemon.Add(distance);
            previousMoveDirections.Add(pokemonPreviousMove[pokemon]);

            toMovePokemons.Add(pokemon);
        }

        NativeArray<MoveDirection> resultMoveDirections = new NativeArray<MoveDirection>(pokemonIndexes.Count, Allocator.TempJob);
        NativeArray<Vector2Int> nativePokemonIndexes = new NativeArray<Vector2Int>(pokemonIndexes.ToArray(), Allocator.TempJob);
        NativeArray<Vector2Int> nativeDistancesBetweenTargetPokemon = new NativeArray<Vector2Int>(distnacesBetweenTargetPokemon.ToArray(), Allocator.TempJob);
        NativeArray<MoveDirection> nativePreviousMoveDirections = new NativeArray<MoveDirection>(previousMoveDirections.ToArray(), Allocator.TempJob);
        NativeArray<Vector2Int> nativePokemonsInBattleIndexs = new NativeArray<Vector2Int>(liveOwnerPokemons.Values.Concat(liveChallengerPokemons.Values).ToArray(), Allocator.TempJob);

        CalculateMoveDirectionJob calculateMoveDirectionJob = new CalculateMoveDirectionJob();
        calculateMoveDirectionJob.resultMoveDirections = resultMoveDirections;
        calculateMoveDirectionJob.pokemonIndexes = nativePokemonIndexes;
        calculateMoveDirectionJob.distancesBetweenTarget = nativeDistancesBetweenTargetPokemon;
        calculateMoveDirectionJob.previousMoveDirections = nativePreviousMoveDirections;
        calculateMoveDirectionJob.pokemonsInBattleIndexes = nativePokemonsInBattleIndexs;

        JobHandle calculateMoveDirectionHandle = calculateMoveDirectionJob.Schedule(nativePokemonIndexes.Length, 1);
        calculateMoveDirectionHandle.Complete();
        
        for (int i = 0; i < toMovePokemons.Count; i++)
        {
            Vector2Int index = pokemonIndexes[i];
            MoveDirection moveDirection = resultMoveDirections[i];
            Pokemon toMovePokemon = toMovePokemons[i];

            if(toMovePokemon.transform.position.x > toMovePokemon.attackTarget.transform.position.x)
            {
                toMovePokemon.spriteRenderer.flipX = false;
            } else
            {
                toMovePokemon.spriteRenderer.flipX = true;
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

            if (toMovePokemon.trainer == chessBoard.owner ? 
                liveOwnerPokemons.Values.Contains(moveTo) : 
                liveChallengerPokemons.Values.Contains(moveTo)) {
                moveDirection = MoveDirection.None;
                moveTo = index;
            }

            pokemonPreviousMove[toMovePokemon] = moveDirection;
            if (trainer == chessBoard.owner)
            {
                liveOwnerPokemons[toMovePokemon] = moveTo;
            } else
            {
                liveChallengerPokemons[toMovePokemon] = moveTo;
            }
            pokemonsInBattle[moveTo.x, moveTo.y] = toMovePokemon;
            toMovePokemon.MoveTo(chessBoard.IndexToWorldPosition(moveTo));
        }

        nativePokemonsInBattleIndexs.Dispose();
        nativePokemonIndexes.Dispose();
        nativePreviousMoveDirections.Dispose();
        nativeDistancesBetweenTargetPokemon.Dispose();
        resultMoveDirections.Dispose();
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
        if (targetPokemons.Count == 0) return;

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
        StopAttack();

        winner = trainer;
        Trainer loser = trainer == chessBoard.owner ? challenger : chessBoard.owner;
        int damage = CalculateDamage(loser);

        Trainer_Hp_down(loser, damage);

        battleManager.FinishBattleIn(chessBoard, winner, loser);
    }

    public void BattleTimeEnd()
    {
        if (winner != null) return;

        StopAttack();

        Trainer_Hp_down(chessBoard.owner, CalculateDamage(chessBoard.owner));
        Trainer_Hp_down(challenger, CalculateDamage(challenger));

        battleManager.DrawBattleIn(chessBoard, chessBoard.owner, challenger);
    }

    private void StopAttack()
    {
        foreach (Pokemon liveOwnerPokemon in liveOwnerPokemons.Keys)
        {
            liveOwnerPokemon.currentState = PokemonState.Idle;
        }

        foreach (Pokemon liveChallengerPokemon in liveChallengerPokemons.Keys)
        {
            liveChallengerPokemon.currentState = PokemonState.Idle;
        }
    }

    private int CalculateDamage(Trainer loser)
    {
        int temp_damage = 2;

        List<Pokemon> livePokemons = loser == challenger ? new List<Pokemon>(liveOwnerPokemons.Keys) : new List<Pokemon>(liveChallengerPokemons.Keys);

        foreach (Pokemon livepokemon in livePokemons)
        {
            temp_damage += livepokemon.cost;
            if (livepokemon.evolutionPhase == 2)
            {
                temp_damage += 1;
            }
            else if (livepokemon.evolutionPhase == 3)
            {
                temp_damage += 3;
            }
        }

        return temp_damage;
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

    public bool IsAttackTargetInRange(Pokemon pokemon)
    {
        Vector2Int distance;
        if (pokemon.trainer == chessBoard.owner)
        {
             distance = liveOwnerPokemons[pokemon] - liveChallengerPokemons[pokemon.attackTarget];
        } else
        {
            distance = liveChallengerPokemons[pokemon] - liveOwnerPokemons[pokemon.attackTarget];
        }

        return Mathf.Abs(distance.x) <= pokemon.range && Mathf.Abs(distance.y) <= pokemon.range;
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    private void Trainer_Hp_down(Trainer trainer, int damage)
    {
        if(trainer is Stage)
        {
            Debug.Log("스테이지 트레이너이기에 피가 안깍인다");
        }
        else
        {
            trainer.currentHp -= damage;
        }
    }

    public Pokemon GetNearstEnemyPokemon(Pokemon requester)
    {
        Dictionary<Pokemon, Vector2Int> enemyPokemonsAndIndexes;
        Vector2Int index;

        if (requester.trainer == chessBoard.owner)
        {
            enemyPokemonsAndIndexes = liveChallengerPokemons;
            index = liveOwnerPokemons[requester];
        } else
        {
            enemyPokemonsAndIndexes = liveOwnerPokemons;
            index = liveChallengerPokemons[requester];
        }

        float minDistance = 1000;
        Pokemon nearstEnemy = null;

        foreach (KeyValuePair<Pokemon, Vector2Int> enemyPokemonAndIndex in enemyPokemonsAndIndexes)
        {
            float distance = Vector2Int.Distance(index, enemyPokemonAndIndex.Value);
            if (minDistance > distance)
            {
                minDistance = distance;
                nearstEnemy = enemyPokemonAndIndex.Key;
            }
        }

        return nearstEnemy;
    }

    public Vector3 GetPosition(Pokemon pokemon)
    {
        if (pokemon.trainer == chessBoard.owner)
        {
            return chessBoard.IndexToWorldPosition(liveOwnerPokemons[pokemon]);
        } else
        {
            return chessBoard.IndexToWorldPosition(liveChallengerPokemons[pokemon]);
        }
    }
}
