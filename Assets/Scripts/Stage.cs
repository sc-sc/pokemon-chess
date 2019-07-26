using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Wild, StageTrainer, GymLeader
}
public class Stage : Trainer
{
    public GameObject[] pokemons;
    public Vector2Int[] positions;
    public StageType stageType;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < pokemons.Length; i++)
        {
            Pokemon pokemon = Instantiate(pokemons[i], transform).GetComponent<Pokemon>();
            pokemon.trainer = this;
            SetPlacedPokemon(positions[i], pokemon);
        }
    }

    public void DestroySelf()
    {
        List<Pokemon> placedPokemons = new List<Pokemon>(this.placedPokemons.Keys);

        foreach (Pokemon placedPokemon in placedPokemons)
        {
            placedPokemon.gameObject.SetActive(false);
            RemovePlacedPokemon(placedPokemon);
        }

        Destroy(gameObject);
    }
}
