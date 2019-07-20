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
    public int exp_expect;
    public int exp_present;
    public int Bug;
    public int Water;
    public int Fire;

    public Dictionary<Pokemon, Vector2Int> placedPokemons = new Dictionary<Pokemon, Vector2Int>();
    public Pokemon[] waitingPokemons;

    protected virtual void Awake()
    {
        waitingPokemons = new Pokemon[CanWaitPokemonsNumber];
    }
}
