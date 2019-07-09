using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessBoard : MonoBehaviour, Touchable
{
    private Tilemap tilemap;
    private Vector3Int selectedPosition;
    private Vector3Int passingPosition;
    private Pokemon selectedPokemon;
    private Pokemon[, ] placedPokemons;
    private Dictionary<Pokemon, Vector2Int> pokemonCache;

    public void Moved(Vector3 to)
    {
        if (passingPosition != null && passingPosition != selectedPosition)
        {
            tilemap.SetColor(passingPosition, Color.white);
        }

        passingPosition = tilemap.WorldToCell(to);
        if (passingPosition != selectedPosition)
        {
            tilemap.SetColor(passingPosition, Color.green);
        }
            
        if (selectedPokemon != null)
        {
            selectedPokemon.transform.position = to;
        }
    }

    public void Released(Vector3 at)
    {
        tilemap.SetColor(selectedPosition, Color.white);
        tilemap.SetColor(passingPosition, Color.white);

        ChessSquare chessSquare = tilemap.GetTile(tilemap.WorldToCell(passingPosition)) as ChessSquare;

        if (!PlacePokemon(CellToIndex(passingPosition), selectedPokemon))
        {
            selectedPokemon.transform.position = tilemap.GetCellCenterWorld(selectedPosition);
        }

        selectedPokemon = null;
    }

    public void Touched(Vector3 at)
    {
        selectedPosition = tilemap.WorldToCell(at);
        tilemap.SetColor(selectedPosition, Color.cyan);

        Vector2Int index = CellToIndex(selectedPosition);
        selectedPokemon = placedPokemons[index.x, index.y];
        Debug.Log(selectedPokemon);
    }

    public bool PlacePokemon(Vector2Int index, Pokemon pokemon)
    {
        Vector3Int cellPosition = IndexToCell(index);
        ChessSquare chessSquare = tilemap.GetTile(cellPosition) as ChessSquare;
        if (chessSquare == null)
        {
            return false;
        }

        Pokemon alreadyExistPokemon = placedPokemons[index.x, index.y];
        if (alreadyExistPokemon != null)
        {
            SetPokemon(pokemonCache[pokemon], alreadyExistPokemon);
        }
        else if (pokemon != null)
        {
            if (pokemonCache.ContainsKey(pokemon))
            {
                Vector2Int previousIndex = pokemonCache[pokemon];
                placedPokemons[previousIndex.x, previousIndex.y] = null;
            }
        }

        SetPokemon(index, pokemon);
        return true;    
    }

    private void SetPokemon(Vector2Int index, Pokemon pokemon)
    {
        placedPokemons[index.x, index.y] = pokemon;
        if (pokemon != null)
        {
            pokemonCache[pokemon] = index;
            pokemon.transform.position = tilemap.GetCellCenterWorld(IndexToCell(index));
        }
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        placedPokemons = new Pokemon[8, 8];

        pokemonCache = new Dictionary<Pokemon, Vector2Int>();

        Pokemon[] pokemons = FindObjectsOfType<Pokemon>();

        for (int i = 0; i < pokemons.Length; i++)
        {
            PlacePokemon(new Vector2Int(i, i), pokemons[i]);
        }
    }

    private Vector2Int CellToIndex(Vector3Int cellPosition)
    {
        return new Vector2Int(-(cellPosition.y - 3), cellPosition.x + 4);
    }

    private Vector3Int IndexToCell(Vector2Int index)
    {
        return new Vector3Int((index.y - 4), -(index.x - 3), 0);
    }
}
