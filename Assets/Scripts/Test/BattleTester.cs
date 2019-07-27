using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTester : MonoBehaviour
{
    public Trainer home;
    public Trainer away;

    [System.Serializable]
    public struct TestPokemon
    {
        [SerializeField] public Vector2Int position;
        [SerializeField] public Pokemon pokemon;
    }

    public ChessBoard homeChessBaord;
    public TestPokemon[] homePokemons;
    public TestPokemon[] awayPokemons;
    void Start()
    {
        home.level = homePokemons.Length;
        away.level = homePokemons.Length;

        homeChessBaord.owner = home;

        foreach (TestPokemon homePokemon in homePokemons)
        {
            Pokemon homePokemonInstance = Instantiate(homePokemon.pokemon);
            homePokemonInstance.trainer = home;
            homeChessBaord.PlacePokemon(homePokemon.position, homePokemonInstance);
        }

        foreach (TestPokemon awayPokemon in awayPokemons)
        {
            Pokemon awayPokemonInstance = Instantiate(awayPokemon.pokemon);
            awayPokemonInstance.trainer = away;
            away.SetPlacedPokemon(awayPokemon.position, awayPokemonInstance);
        }

        homeChessBaord.ReadyBattle(away);
        homeChessBaord.StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
