using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    public static int CalculateDamage(Pokemon attacker, Pokemon defensor)
    {
        return (int) ((((((Level(attacker) * 2 / 5) + 2) * 30 * GetActualStat(attacker.baseAttack, PokemonStat.Attack, attacker) / 50)
            / GetActualStat(defensor.baseDefense, PokemonStat.Defense, defensor)) * Mod1(attacker, defensor) + 2) * Critical(attacker, defensor)
            * Mod2(attacker, defensor) * Stab(attacker) * TypeEffectiveness(attacker, defensor) * Mod3(attacker, defensor));
    }

    private static float Mod1(Pokemon attacker, Pokemon defensor) { return 1f; }
    private static float Mod2(Pokemon attacker, Pokemon defensor) { return 1f; }
    private static float Mod3(Pokemon attacker, Pokemon defensor) { return 1f; }

    private static float Stab(Pokemon attacker) { return 1f; }

    private static float Critical(Pokemon attacker, Pokemon defensor)
    {
        return 1f;
    }

    public static float TypeEffectiveness(Pokemon attacker, Pokemon defensor)
    {
        return 1f;
    }

    public static int GetActualHp(Pokemon pokemon)
    {
        return ActualStatCommonFormula(pokemon.baseHp, pokemon) + 10 + Level(pokemon);
    }
    private static int GetActualStat(int baseStat, Pokemon pokemon)
    {
        return ActualStatCommonFormula(baseStat, pokemon) + 5;
    }

    public static int GetActualStat(int baseStat, PokemonStat statType, Pokemon pokemon)
    {
        return (int) (StatRank(statType, pokemon) * GetActualStat(baseStat, pokemon));
    }

    public static float StatRank(PokemonStat statType, Pokemon pokemon)
    {
        int statRank = pokemon.statRank[statType];

        if (statRank == 0) return 1f;
        else if (statRank > 0)
        {
            if (statRank >= 6f) return 4f;
            else
                return (2 + statRank) / 2f;
        } else
        {
            if (statRank <= -6) return 1f / 4f;
            else
                return 2f / (2f - statRank);
        }
    }

    private static int ActualStatCommonFormula(int baseStat, Pokemon pokemon)
    {
        return ((baseStat * 2) + IV(pokemon) + (EV(pokemon) / 4)) * Level(pokemon) / 100;
    }

    private static int IV(Pokemon pokemon)
    {
        return (6 + pokemon.cost * 2) * (pokemon.evolutionPhase - 1);
    }

    private static int EV(Pokemon pokemon)
    {
        return (6 * (pokemon.cost) - 2) * pokemon.evolutionPhase;
    }

    private static int Level(Pokemon pokemon)
    {
        return 40 + pokemon.evolutionPhase * 5;
    }
}
