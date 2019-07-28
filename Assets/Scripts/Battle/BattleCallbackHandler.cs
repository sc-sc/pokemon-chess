using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCallbackHandler : MonoBehaviour
{
    private BattleExecutor battleExecutor;
    void Awake()
    {
        battleExecutor = GetComponent<BattleExecutor>();
    }

    public void PokemonDead(Pokemon pokemon)
    {
        battleExecutor.PokemonDead(pokemon);
    }

    public void LostAttackTarget(Pokemon pokemon)
    {
        battleExecutor.SetAttackTargetTo(pokemon);
    }

    public bool IsAttackTargetInRange(Pokemon pokemon)
    {
        return battleExecutor.IsAttackTargetInRange(pokemon);
    }

    public Pokemon GetNearstEnemyPokemon(Pokemon requester)
    {
        return battleExecutor.GetNearstEnemyPokemon(requester);
    }

    public Vector3 GetPosition(Pokemon pokemon)
    {
        return battleExecutor.GetPosition(pokemon);
    }
}
