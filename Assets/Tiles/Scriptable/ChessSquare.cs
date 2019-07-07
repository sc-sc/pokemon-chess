using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class ChessSquare : TileBase
{
    private Pokemon locatedPokemon;
    public Sprite whiteSprite;
    public Sprite blackSprite;
    
    [MenuItem("Assets/Tiles/ChessSquare")]
    public static void CreateChessSquare()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Chess Square", "New Chess Square", "Asset", "Save Chess Square", "Assets/Tiles");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ChessSquare>(), path);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if ((position.x + position.y) % 2 == 0)
        {
            tileData.sprite = whiteSprite;
        } else
        {
            tileData.sprite = blackSprite;
        }

        tileData.colliderType = Tile.ColliderType.Grid;
    }

    public void LocatePokemon(Pokemon pokemon)
    {
        locatedPokemon = pokemon;
    }

    public Pokemon GetLocatedPokemon()
    {
        return locatedPokemon;
    }
}
