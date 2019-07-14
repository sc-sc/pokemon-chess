using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafariTester : MonoBehaviour
{
    public PokemonSafariManager pokemonSafari;
    void Start()
    {
        pokemonSafari.Refresh();
    }
}
