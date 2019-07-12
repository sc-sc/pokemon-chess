using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaitingBoard : PokemonPlaceableBoard
{
    public override void Moved(Vector3 to)
    {
        base.Moved(to);
    }

    protected override void ChangePassingSquareColor(Vector3Int passingCellPosition)
    {
        tilemap.SetColor(passingCellPosition, Color.green);
    }

    public override void Released(Vector3 at)
    {
        base.Released(at);
    }

    public override void Touched(Vector3 at)
    {
        base.Touched(at);
    }

    protected override Vector2Int CellToIndex(Vector3Int cellPosition)
    {
        return new Vector2Int(0, -(cellPosition.y - 3));
    }

    protected override Vector3Int IndexToCell(Vector2Int index)
    {
        return new Vector3Int(-7, -(index.y - 3), 0);
    }

    protected override void Awake()
    {
        base.Awake();

        placedPokemons = new Pokemon[1, Trainer.CanWaitPokemonsNumber];
        linkedBoard = GetComponentInParent<ChessBoard>();

        for (int i = 0; i < Trainer.CanWaitPokemonsNumber; i++)
        {
            Vector3Int cellPosition = IndexToCell(new Vector2Int(0, i));
            tilemap.SetTileFlags(cellPosition, TileFlags.None);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
