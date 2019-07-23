using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : Trainer
{
    public GameObject[] pokemons;
    public Vector2Int[] positions;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < pokemons.Length; i++)
        {
            Pokemon pokemon = Instantiate(pokemons[i]).GetComponent<Pokemon>();
            pokemon.trainer = this;
            SetPlacedPokemon(positions[i], pokemon);
        }
    }
}
