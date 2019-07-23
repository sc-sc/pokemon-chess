using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public const int CanWaitPokemonsNumber = 9;

    public string id;
    public string nickname;
    public int level;
    public int hp;
    public int currentHp;
    public int money;
    public int exp_expect;
    public int exp_present;
    public Dictionary<PokemonType, List<Pokemon>> placedPokemonTypeDictionary = new Dictionary<PokemonType, List<Pokemon>>();

    public Dictionary<Pokemon, Vector2Int> placedPokemons = new Dictionary<Pokemon, Vector2Int>();
    public Pokemon[] waitingPokemons;
    private PokemonUIManager pokemonUIManager;

    protected virtual void Awake()
    {
        waitingPokemons = new Pokemon[CanWaitPokemonsNumber];
        pokemonUIManager = FindObjectOfType<PokemonUIManager>();
    }

    public void SetPlacedPokemon(Vector2Int at, Pokemon pokemon)
    {
        pokemonUIManager.AddPokemonUI(pokemon);
        pokemon.currnetHp = pokemon.actualHp;
        pokemon.currentPp = pokemon.initialPp;

        placedPokemons[pokemon] = at;
        foreach (PokemonType type in pokemon.types)
        {
            if (!placedPokemonTypeDictionary.ContainsKey(type))
            {
                placedPokemonTypeDictionary[type] = new List<Pokemon>();
            }

            List<Pokemon> pokemonList = placedPokemonTypeDictionary[type];

            foreach (Pokemon sameTypePokemon in pokemonList)
            {
                if (sameTypePokemon.name == pokemon.name)
                    return;
            }

            pokemonList.Add(pokemon);
            Debug.Log((type, pokemonList.Count));
        }
    }

    public void RemovePlacedPokemon(Pokemon pokemon)
    {
        pokemonUIManager.RemovePokemonUI(pokemon);
        placedPokemons.Remove(pokemon);
        foreach (PokemonType type in pokemon.types)
        {
            List<Pokemon> pokemonList = placedPokemonTypeDictionary[type];
            pokemonList.Remove(pokemon);
            Debug.Log((type, pokemonList.Count));
        }
    }
}
