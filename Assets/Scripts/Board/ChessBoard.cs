using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessBoard : PokemonPlaceableBoard
{
    public const int MaxRowCanPlace = 4;

    private PokemonUIManager pokemonUIManager;

    public override void Moved(Vector3 to)
    {
        base.Moved(to);
    }

    protected override void ChangePassingSquareColor(Vector3Int passingCellPosition)
    {
        if (CellToIndex(passingCellPosition).y < MaxRowCanPlace)
            tilemap.SetColor(passingCellPosition, Color.green);
        else
            tilemap.SetColor(passingCellPosition, Color.red);
    }

    public override void Released(Vector3 at)
    {
        base.Released(at);
    }
    protected override void Awake()
    {
        base.Awake();

        linkedBoard = GetComponentInChildren<WaitingBoard>();
        
        placedPokemons = new Pokemon[8, 8];

        pokemonUIManager = FindObjectOfType<PokemonUIManager>();
    }

    protected override Vector3Int IndexToCell(Vector2Int index)
    {
        return new Vector3Int((index.y - 4), -(index.x - 3), 0);
    }


    protected override Vector2Int CellToIndex(Vector3Int cellPosition)
    {
        return new Vector2Int(-(cellPosition.y - 3), cellPosition.x + 4);
    }

    public override bool PlacePokemon(Vector2Int index, Pokemon pokemon)
    {
        if (pokemon != null && index.y >= MaxRowCanPlace)
            return false;

        return base.PlacePokemon(index, pokemon);
    }

    protected override bool IsCanAddPokemon(Vector2Int index, Pokemon pokemon)
    {
        if (pokemonCache.Count >= owner.level)
        {
            return false;
        }

        return base.IsCanAddPokemon(index, pokemon);
    }
    protected override void CompleteSetPokemon(Vector2Int at, Pokemon pokemon)
    {
        if (pokemon != null)
        {
            owner.placedPokemons[pokemon] = at;
            pokemonUIManager.AddPokemonUI(pokemon);
            Check_type(pokemon);
        }
    }
    protected override void CompleteRemovePokemon(Vector2Int at, Pokemon pokemon)
    {
        owner.placedPokemons.Remove(pokemon);
        pokemonUIManager.RemovePokemonUI(pokemon);
    }
    public void Check_type(Pokemon pokemon)
    {
        for(int i=0; i<pokemon.types.Length; i++)
        {
            Debug.Log(pokemon.types[i]);
        }
    }
}
