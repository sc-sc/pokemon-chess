using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PokemonPlaceableBoard : MonoBehaviour, Touchable
{
    public Trainer owner;
    public Tilemap tilemap;
    protected Vector3Int selectedPosition;
    protected Vector3Int passingPosition;
    public Pokemon selectedPokemon;
    protected Pokemon[,] placedPokemons;
    protected Dictionary<Pokemon, Vector2Int> pokemonCache;

    protected PokemonPlaceableBoard linkedBoard;

    protected virtual void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        pokemonCache = new Dictionary<Pokemon, Vector2Int>();
    }

    public virtual bool PlacePokemon(Vector2Int index, Pokemon pokemon)
    {
        if (pokemon != null)
        {
            Vector3Int cellPosition = IndexToCell(index);
            if (!tilemap.HasTile(cellPosition))
                return false;
            else if (pokemon.trainer != owner)
                return false;

            Pokemon alreadyExistPokemon = placedPokemons[index.x, index.y];
            if (alreadyExistPokemon != null)
            {
                if (alreadyExistPokemon.trainer != owner)
                    return false;
            }
            if (pokemonCache.ContainsKey(pokemon))
            {
                SetPokemon(pokemonCache[pokemon], alreadyExistPokemon);
            } else if (linkedBoard.pokemonCache.ContainsKey(pokemon))
            {
                linkedBoard.SetPokemon(linkedBoard.pokemonCache[pokemon], alreadyExistPokemon);
            }

            linkedBoard.PlaceSuccessAtLinkedBoard(pokemon);

            SetPokemon(index, pokemon);
        }

        return true;
    }

    public void PlaceSuccessAtLinkedBoard(Pokemon pokemon)
    {
        selectedPosition = new Vector3Int(0, 0, -100);

        if (pokemonCache.ContainsKey(pokemon))
        {
            Vector2Int placedIndex = pokemonCache[pokemon];
            Vector3Int cellPosition = IndexToCell(placedIndex);
            tilemap.SetColor(cellPosition, Color.white);
            pokemonCache.Remove(pokemon);
        }
    }

    public void PlaceFailAtLinkedBoard(Pokemon pokemon)
    {
        if (pokemonCache.ContainsKey(pokemon))
        {
            Vector3Int cellPosition = IndexToCell(pokemonCache[pokemon]);
            tilemap.SetColor(cellPosition, Color.white);
            pokemon.transform.position = tilemap.GetCellCenterWorld(cellPosition);
        }
    }

    private void SetPokemon(Vector2Int index, Pokemon pokemon)
    {
        placedPokemons[index.x, index.y] = pokemon;
        if (pokemon != null)
        {
            pokemonCache[pokemon] = index;
            owner.placedPokemons[pokemon] = index;
            pokemon.transform.position = tilemap.GetCellCenterWorld(IndexToCell(index));
        }
    }

    protected abstract Vector2Int CellToIndex(Vector3Int cellPosition);

    protected abstract Vector3Int IndexToCell(Vector2Int index);

    public bool HasSquare(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        return tilemap.HasTile(cellPosition);
    }

    public virtual void Touched(Vector3 at)
    {
        selectedPosition = tilemap.WorldToCell(at);
        tilemap.SetColor(selectedPosition, Color.cyan);

        Vector2Int index = CellToIndex(selectedPosition);
        selectedPokemon = placedPokemons[index.x, index.y];
    }

    public virtual void Moved(Vector3 to)
    {
        if (selectedPokemon != null && selectedPokemon.trainer == owner)
        {
            selectedPokemon.transform.position = to;
        }


        if (passingPosition != null && passingPosition != selectedPosition)
        {
            tilemap.SetColor(passingPosition, Color.white);
        }

        passingPosition = tilemap.WorldToCell(to);
        if (passingPosition != selectedPosition)
        {
            ChangePassingSquareColor(passingPosition);
        }

        if (linkedBoard.HasSquare(to) && selectedPokemon != null)
        {
            linkedBoard.selectedPokemon = selectedPokemon;
            selectedPokemon = null;
            FindObjectOfType<TouchManager>().Delegate(this, linkedBoard);
        }
    }

    protected abstract void ChangePassingSquareColor(Vector3Int passingCellPosition);

    public virtual void Released(Vector3 at)
    {
        tilemap.SetColor(selectedPosition, Color.white);
        tilemap.SetColor(passingPosition, Color.white);

        Vector2Int passingIndex = CellToIndex(passingPosition);

        if (!PlacePokemon(passingIndex, selectedPokemon))
        {
            if (pokemonCache.ContainsKey(selectedPokemon))
            {
                selectedPokemon.transform.position = tilemap.GetCellCenterWorld(selectedPosition);
            } else
            {
                linkedBoard.PlaceFailAtLinkedBoard(selectedPokemon);
            }
        }

        selectedPosition = new Vector3Int(0, 0, -100);
        selectedPokemon = null;
    }
}
