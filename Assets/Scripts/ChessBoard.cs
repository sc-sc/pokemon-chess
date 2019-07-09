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
        } else if (selectedPosition != passingPosition)
        {
            PlacePokemon(CellToIndex(selectedPosition), null);
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

    public bool PlacePokemon(Vector2Int position, Pokemon pokemon)
    {
        Vector3Int cellPosition = IndexToCell(position);
        ChessSquare chessSquare = tilemap.GetTile(cellPosition) as ChessSquare;
        if (chessSquare == null)
        {
            return false;
        }

        placedPokemons[position.x, position.y] = pokemon;
        if (pokemon != null)
        {
            pokemon.transform.position = tilemap.GetCellCenterWorld(cellPosition);
        }

        return true;    
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        placedPokemons = new Pokemon[8, 8];

        Pokemon[] pokemons = FindObjectsOfType<Pokemon>();

        PlacePokemon(new Vector2Int(0, 0), pokemons[0]);
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
