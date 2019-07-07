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

        Debug.Log(passingPosition);
        ChessSquare chessSquare = tilemap.GetTile(tilemap.WorldToCell(passingPosition)) as ChessSquare;

        if (!PlacePokemon(passingPosition, selectedPokemon))
        {
            selectedPokemon.transform.position = tilemap.GetCellCenterWorld(selectedPosition);
        }

        selectedPokemon = null;
    }

    public void Touched(Vector3 at)
    {
        selectedPosition = tilemap.WorldToCell(at);
        tilemap.SetColor(selectedPosition, Color.cyan);

        selectedPokemon = (tilemap.GetTile(selectedPosition) as ChessSquare)?.GetLocatedPokemon();
    }

    public bool PlacePokemon(Vector3Int position, Pokemon pokemon)
    {
        ChessSquare chessSquare = tilemap.GetTile(position) as ChessSquare;
        if (chessSquare == null)
        {
            return false;
        }

        chessSquare.LocatePokemon(pokemon);
        pokemon.transform.position = tilemap.GetCellCenterWorld(position);

        return true;
    }

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        PlacePokemon(new Vector3Int(-1, 0, 0), FindObjectOfType<Pokemon>());
    }
}
