using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public const int CanWaitPokemonsNumber = 9;

    public string id;
    public string nickname;
    public int level;
    public int hp;
    public int currentHp;
    public int money;

    public Dictionary<Pokemon, Vector2Int> placedPokemons;
    public Pokemon[] waitingPokemons;

    protected virtual void Awake()
    {
        placedPokemons = new Dictionary<Pokemon, Vector2Int>();
        waitingPokemons = new Pokemon[CanWaitPokemonsNumber];
    }
}
