using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Physical, Speical
}
public class DamageCalculator : MonoBehaviour
{
    public static float[,] TypeChart = new float[18, 18]
    {// 방어자 노말 불꽃 물 풀 전기 얼음 격투 독 땅 비행 에스퍼 벌레 바위 고스트 드래곤 악 강철 페어리
     // 공격자
     /*노말*/{ 1f,   1f, 1f,1f, 1f,  1f,  1f, 1f, 1f, 1f,  1f,   1f,  0.5f, 0f,    1f,  1f,0.5f, 1f},
     /*불꽃*/{ 1f, 0.5f,0.5f,2f, 1f, 2f,  1f, 1f, 1f, 1f,  1f,   2f,  0.5f, 1f,    0.5f, 1f,2f,  1f},
     /*물*/  { 1f,   2f,0.5f,0.5f,1f,1f,  1f, 1f, 2f, 1f,  1f,   1f,   2f,  1f,    0.5f,  1f,1f, 1f},
     /*풀*/  { 1f, 0.5f, 2f, 0.5f,1f, 1f, 1f,0.5f,2f, 0.5f, 1f, 0.5f,  2f, 1f,     0.5f, 1f,0.5f, 1f},
     /*전기*/{ 1f, 1f,  2f,0.5f,0.5f, 1f, 1f, 1f, 0f, 2f,  1f,  1f,   1f,   1f,   0.5f, 1f, 1f,  1f},
     /*얼음*/{ 1f, 0.5f,0.5f,2f, 1f, 0.5f,1f, 1f, 2f, 2f,  1f,  1f,  1f,  1f,      2f,  1f, 0.5f, 1f},
     /*격투*/{ 2f,   1f, 1f,1f, 1f,  2f,  1f,0.5f,1f,0.5f,0.5f, 0.5f, 2f,  0f,     1f,  2f, 2f, 0.5f },
     /*독*/  { 1f, 1f,  1f, 2f, 1f,  1f,  1f,0.5f,0.5f,1f, 1f,  1f,  0.5f, 0.5f,   1f,  1f, 0f, 2f },
     /*땅*/  { 1f, 2f,1f,0.5f,  2f,  1f,  1f, 2f, 1f, 0f,  1f,  0.5f, 2f,  1f,     1f,  1f, 2f, 1f },
     /*비행*/{ 1f, 1f,  1f, 2f,0.5f, 1f,  2f, 1f, 1f, 1f, 1f,   2f,  0.5f, 1f,    1f,   1f,0.5f, 1f },
   /*에스퍼*/{ 1f, 1f,  1f, 1f, 1f,  1f,  2f, 2f, 1f, 1f, 0.5f, 1f,  1f,  1f,    1f,    0f, 0.5f, 1f },
    /*벌레*/ { 1f,0.5f, 1f, 2f, 1f,  1f, 0.5f,0.5f,1f,0.5f,2f,  1f,  1f, 0.5f,   1f,    2f, 0.5f,0.5f },
    /*바위*/ { 1f, 2f,  1f, 1f, 1f,  2f, 0.5f, 1f,0.5f,2f, 1f,  2f,  1f,  1f,    1f,    1f, 0.5f, 1f },
   /*고스트*/{ 0f, 1f,  1f, 1f, 1f,  1f,  1f, 1f, 1f, 1f, 2f,   1f,  1f,  2f,    1f,   0.5f, 1f, 1f },
   /*드래곤*/{ 1f, 1f,  1f, 1f, 1f,  1f,  1f, 1f, 1f, 1f, 1f,   1f,  1f,  1f,    2f,   1f,  0.5f, 0f },
     /*악*/  { 1f, 1f,  1f, 1f, 1f,  1f, 0.5f, 1f, 1f, 1f, 2f,  1f,  1f,  2f,    1f,  0.5f, 1f,  0.5f },
     /*강철*/{ 1f,0.5f,0.5f,1f,0.5f, 1f,  2f, 1f, 1f,  1f, 1f,  1f,  2f,  1f,    1f,   1f,  0.5f, 2f },
   /*페어리*/{ 1f,0.5f, 1f, 1f, 1f,  1f,  2f,0.5f, 1f, 1f, 1f,  1f,  1f,  1f,    2f,   2f,  0.5f, 1f }
    };

    public static int CalculateBasicAttackDamage(Pokemon attacker, Pokemon defensor)
    {
        return (int) ((((((Level(attacker) * 2 / 5) + 2) * 30 * GetActualStat(attacker.baseAttack, PokemonStat.Attack, attacker) / 50)
            / GetActualStat(defensor.baseDefense, PokemonStat.Defense, defensor)) * Mod1(attacker, defensor, AttackType.Physical) + 2) * Critical(attacker, defensor)
            * Mod2(attacker, defensor) * 1f * 1f * Mod3(attacker, defensor));
    }

    public static int CalculateSkillDamage(Pokemon attacker, Pokemon defensor, int baseDamage, PokemonType skillType)
    {
        return (int)((((((Level(attacker) * 2 / 5) + 2) * baseDamage * GetActualStat(attacker.baseAttack, PokemonStat.SpecialAttack, attacker) / 50)
            / GetActualStat(defensor.baseDefense, PokemonStat.SpecialDefense, defensor)) * Mod1(attacker, defensor, AttackType.Speical) + 2) * Critical(attacker, defensor)
            * Mod2(attacker, defensor) * Stab(attacker, skillType) * TypeEffectiveness(defensor, skillType) * Mod3(attacker, defensor));
    }

    private static float Mod1(Pokemon attacker, Pokemon defensor, AttackType attackType) {
        float mod1 = 1f;

        if (attackType == AttackType.Physical)
        {
            if (attacker.GetCurrentStatus() == PokemonStatus.Burn)
                mod1 *= 0.5f;
        }

        return mod1;
    }
    private static float Mod2(Pokemon attacker, Pokemon defensor) { return 1f; }
    private static float Mod3(Pokemon attacker, Pokemon defensor) { return 1f; }

    private static float Stab(Pokemon attacker, PokemonType skillType) {
        foreach (PokemonType pokemonType in attacker.types)
        {
            if (pokemonType == skillType) return 1.5f;
        }

        return 1f;
    }

    private static float Critical(Pokemon attacker, Pokemon defensor)
    {
        return 1f;
    }

    public static float TypeEffectiveness(Pokemon defensor, PokemonType skillType)
    {
        float effectiveness = 1f;
        
        foreach (PokemonType pokemonType in defensor.types)
        {
            effectiveness *= TypeChart[(int) skillType, (int) pokemonType];
        }

        Debug.Log("효과: " + effectiveness);
        return effectiveness;
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
        return (int) (StatRank(statType, pokemon) * GetActualStat(baseStat, pokemon) * ItemBonus(statType, pokemon));
    }

    private static float ItemBonus(PokemonStat statType, Pokemon pokemon)
    {
        float bonus = 1f;

        foreach (Item item in pokemon.GetItems())
        {
            bonus *= item.GetStatBonus(statType, pokemon);
        }

        return bonus;
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
