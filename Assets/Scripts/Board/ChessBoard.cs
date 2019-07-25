using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessBoard : PokemonPlaceableBoard
{
    public const int MaxRowCanPlace = 4;
    
    private BattleExecutor battleExecutor;
    public Transform challengerPosition;

    protected override void ChangePassingSquareColor(Vector3Int passingCellPosition)
    {
        if (CellToIndex(passingCellPosition).y < MaxRowCanPlace)
            tilemap.SetColor(passingCellPosition, Color.green);
        else
            tilemap.SetColor(passingCellPosition, Color.red);
    }

    public override void SpecialTouched(Vector3 at)
    {
        if (battleExecutor.isInBattle)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(at);
            Vector2Int index = CellToIndex(cellPosition);
            Pokemon pokemon = battleExecutor.GetPokemonInBattle(index);
            if (pokemon != null)
            {
                pokemonInformation.ShowPokemonInformation(pokemon);
            }

        } else
            base.SpecialTouched(at);
    }

    protected override void Awake()
    {
        base.Awake();

        linkedBoard = GetComponentInChildren<WaitingBoard>();
        
        placedPokemons = new Pokemon[8, 8];

        battleExecutor = GetComponent<BattleExecutor>();
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
            owner.SetPlacedPokemon(at, pokemon);
        }
    }
    protected override void CompleteRemovePokemon(Vector2Int at, Pokemon pokemon)
    {
        owner.RemovePlacedPokemon(pokemon);
    }

    public void ReadyBattle(Trainer challenger)
    {
        isTouchable = false;
        if (selectedPokemon != null)
        {
            Released(new Vector3(0, 0));
        }
        challenger.transform.position = challengerPosition.position;
        challenger.GetComponent<Animator>().SetTrigger("Appear");
        battleExecutor.ReadyBattle(challenger);
    }

    public void StartBattle()
    {
        battleExecutor.StartBattle();
    }

    public void EndBattle()
    {
        battleExecutor.EndBattle();
    }

    public void ResetBoard()
    {
        isTouchable = true;

        foreach (KeyValuePair<Pokemon, Vector2Int> pokemonAndIndex in owner.placedPokemons)
        {
            Pokemon pokemon = pokemonAndIndex.Key;
            pokemon.Reset();
            pokemon.transform.position = IndexToWorldPosition(pokemonAndIndex.Value);
        }
    }

    public void BattleTimeEnd()
    {
        battleExecutor.BattleTimeEnd();
    }
}
