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

    public PokemonPlaceableBoard linkedBoard;

    private TouchManager touchManager;
    private SalesDesk salesDesk;

    protected virtual void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        pokemonCache = new Dictionary<Pokemon, Vector2Int>();

        touchManager = FindObjectOfType<TouchManager>();
        salesDesk = FindObjectOfType<SalesDesk>();
    }

    public virtual bool PlacePokemon(Vector2Int index, Pokemon pokemon)
    {
        Vector3Int cellPosition = IndexToCell(index);
        if (!tilemap.HasTile(cellPosition)) return false;

        if (pokemon != null)
        {
            if (pokemon.trainer != owner)
            {
                return false;
            } else
            {
                Pokemon alreadyExistPokemon = placedPokemons[index.x, index.y];
                RemovePokemon(alreadyExistPokemon);
                if (pokemonCache.ContainsKey(pokemon))
                {
                    SetPokemon(pokemonCache[pokemon], alreadyExistPokemon);
                } else if (linkedBoard.pokemonCache.ContainsKey(pokemon))
                {
                    linkedBoard.SetPokemon(linkedBoard.pokemonCache[pokemon], alreadyExistPokemon);
                }

                RemovePokemon(pokemon);
                if (!AddPokemon(index, pokemon))
                {
                    return false;
                }
            }
        }

        linkedBoard.PlaceEnd(pokemon, true);
        return true;
    }

    protected virtual bool AddPokemon(Vector2Int index, Pokemon pokemon)
    {
        SetPokemon(index, pokemon);
        linkedBoard.RemovePokemon(pokemon);

        return true;
    }
    public void SetPokemon(Vector2Int index, Pokemon pokemon)
    {
        placedPokemons[index.x, index.y] = pokemon;
        if (pokemon != null)
        {
            pokemonCache[pokemon] = index;
            pokemon.transform.position = tilemap.GetCellCenterWorld(IndexToCell(index));
        }

        CompleteSetPokemon(index, pokemon);
    }

    protected abstract void CompleteSetPokemon(Vector2Int at, Pokemon pokemon);

    public void RemovePokemon(Pokemon pokemon)
    {
        if (pokemon == null || !pokemonCache.ContainsKey(pokemon)) return;
        Vector2Int index = pokemonCache[pokemon];
        if (index != null)
        {
            if (placedPokemons[index.x, index.y] == pokemon)
            {
                placedPokemons[index.x, index.y] = null;
                CompleteRemovePokemon(index, pokemon);
            }
            pokemonCache.Remove(pokemon);
        }
    }

    protected abstract void CompleteRemovePokemon(Vector2Int at, Pokemon pokemon);

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
        if (selectedPokemon != null && selectedPokemon.trainer is Player)
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
            touchManager.Delegate(this, linkedBoard);
        }

        if (IsInSalesDesk(to))
        {
            salesDesk.previousBoard = this;
            salesDesk.selectedPokemon = selectedPokemon;
            touchManager.Delegate(this, salesDesk);
        }
    }

    private bool IsInSalesDesk(Vector3 at)
    {
        RaycastHit2D hit = Physics2D.Raycast(at, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.tag == "SalesDesk";
        }
        return false;
    }

    protected abstract void ChangePassingSquareColor(Vector3Int passingCellPosition);

    public virtual void Released(Vector3 at)
    {
        tilemap.SetColor(selectedPosition, Color.white);
        tilemap.SetColor(passingPosition, Color.white);

        Vector2Int passingIndex = CellToIndex(passingPosition);

        if (selectedPokemon != null && !PlacePokemon(passingIndex, selectedPokemon))
        {
            if (pokemonCache.ContainsKey(selectedPokemon))
            {
                PlaceEnd(selectedPokemon, false);
            } else if (linkedBoard.pokemonCache.ContainsKey(selectedPokemon))
            {
                linkedBoard.PlaceEnd(selectedPokemon, false);
            } else
            {
                Destroy(selectedPokemon);
            }
        }
        selectedPokemon = null;
    }

    public void PlaceEnd(Pokemon pokemon, bool isSuccess)
    {
        if (!isSuccess)
        {
            SetPokemon(CellToIndex(selectedPosition), pokemon);
        }

        tilemap.SetColor(selectedPosition, Color.white);
        selectedPosition = selectedPosition = new Vector3Int(0, 0, -100);
    }

    public void SpecialTouched(Vector3 at)
    {
        throw new System.NotImplementedException();
    }

    public void SpecialReleased(Vector3 at)
    {
        throw new System.NotImplementedException();
    }
}
