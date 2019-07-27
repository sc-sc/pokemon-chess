using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBall : Item
{
    public override float GetStatBonus(PokemonStat stat, Pokemon pokemon)
    {
        if (stat == PokemonStat.Attack || stat == PokemonStat.SpecialAttack)
        {
            if (pokemon.pokemonName == PokemonName.피츄 || pokemon.pokemonName == PokemonName.피카츄)
            {
                return 2f;
            }
        }

        return 1f;
    }
}
