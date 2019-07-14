﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesDesk : MonoBehaviour, Touchable
{
    private Player player;
    public Pokemon selectedPokemon;
    internal PokemonPlaceableBoard previousBoard;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void Moved(Vector3 to)
    {
        if (selectedPokemon == null) return;

        selectedPokemon.transform.position = to;

        if (previousBoard.HasSquare(to))
        {
            previousBoard.selectedPokemon = selectedPokemon;
            FindObjectOfType<TouchManager>().Delegate(this, previousBoard);
        }
    }

    public void Released(Vector3 at)
    {
        if (selectedPokemon == null) return;

        int cost = selectedPokemon.cost;
        Debug.Log("앙 판매");
        // 판매
        previousBoard.RemovePokemon(selectedPokemon);
        previousBoard.PlaceEnd(selectedPokemon, true);
        previousBoard.linkedBoard.RemovePokemon(selectedPokemon);
        previousBoard.linkedBoard.PlaceEnd(selectedPokemon, true);
        Destroy(selectedPokemon.gameObject);
        player.money += cost;
    }

    public void Touched(Vector3 at)
    {

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