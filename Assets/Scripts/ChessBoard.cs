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

        if (!PlacePokemon(passingPosition, selectedPokemon))
        {
            selectedPokemon.transform.position = tilemap.GetCellCenterWorld(selectedPosition);
        } else
        {
            PlacePokemon(selectedPosition, null);
        }

        selectedPokemon = null;
    }

    public void Touched(Vector3 at)
    {
        selectedPosition = tilemap.WorldToCell(at);
        tilemap.SetColor(selectedPosition, Color.cyan);

        selectedPokemon = placedPokemons[selectedPosition.x + 4, selectedPosition.y + 4];
        Debug.Log(selectedPokemon);
    }

    public bool PlacePokemon(Vector3Int position, Pokemon pokemon)
    {
        ChessSquare chessSquare = tilemap.GetTile(position) as ChessSquare;
        if (chessSquare == null)
        {
            return false;
        }

        placedPokemons[position.x + 4, position.y + 4] = pokemon;
        if (pokemon != null)
        {
            pokemon.transform.position = tilemap.GetCellCenterWorld(position);
        }

        return true;
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        placedPokemons = new Pokemon[8, 8];

        PlacePokemon(new Vector3Int(-1, 0, 0), FindObjectOfType<Pokemon>());
    }
}
