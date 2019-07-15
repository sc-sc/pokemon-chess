﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSafariManager : MonoBehaviour
{
    public Transform[] spawnPoints;

    [System.Serializable]
    public struct SalePokemonPrefabs
    {
        [SerializeField] public GameObject[] prefabs;
    }

    public SalePokemonPrefabs[] salePokemonPrefabs;
    public GameObject pokemonInSafariPrefab;
    public PokemonInformation pokemonInformation;
    
    public const int SalePokemonsCount = 5;

    private GameManager gameManager;

    private Player player;

    private GameObject[] pokemonsInSafari;

    void Awake()
    {
        pokemonsInSafari = new GameObject[SalePokemonsCount];
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
    }
    public void Refresh()
    {
        foreach (GameObject pokemonInSafari in pokemonsInSafari)
        {
            Destroy(pokemonInSafari);
        }

        for (int i = 0; i < SalePokemonsCount; i++)
        {
            int cost = Random.Range(2, 4);
            int index = 0;

            GameObject pokemonInSafari = Instantiate(pokemonInSafariPrefab, transform.parent);
            GameObject salePokemonPrefab = salePokemonPrefabs[cost - 1].prefabs[index];

            Instantiate(salePokemonPrefab, pokemonInSafari.transform);
            PokemonInSafari pokemonInSafariScirpt = pokemonInSafari.GetComponent<PokemonInSafari>();
            pokemonInSafariScirpt.Init(pokemonInformation, () =>
            {
                if (TryBuy(salePokemonPrefab, player))
                {
                    Destroy(pokemonInSafari);
                }
            });

            pokemonsInSafari[i] = pokemonInSafari;
            Vector3 spawnPoint = spawnPoints[i].position;
            pokemonInSafari.transform.position = new Vector3(spawnPoint.x, spawnPoint.y, -20f);
        }
    }

    public void Refresh_Button()
    {
        if (player.money >= 2)
        {
            player.money -= 2;
            Refresh();
        }
    }
    public void Stage_end()
    {
        int plus_money= player.money;
        int temp = 0;
        while(plus_money >= 10)
        {
            temp += 1;
            plus_money -= 10;
        }
        player.money += temp;
        player.money += 5;
        player.exp_present += 2;
        if (player.exp_present >= player.exp_expect)
        {
            Level_Up();
        }

        Refresh();
    }
    public void Exp_Button()
    {
        if (player.money >= 4)
        {
            player.money -= 4;
            player.exp_present += 4;
            if(player.exp_present >= player.exp_expect)
            {
                Level_Up();
            }
        }
    }
    public void Level_Up()
    {
        player.exp_present -= player.exp_expect;
        player.level += 1;
        if (player.level < 4)
        {
            player.exp_expect += 4;
        }
        else if (player.level >= 4 && player.level < 6)
        {
            player.exp_expect += 6;
        }
        else if (player.level >= 6 && player.level < 8)
        {
            player.exp_expect += 10;
        }
        else
        {
            player.exp_expect += 16;
        }
    }

    public bool TryBuy(GameObject pokemonPrefab, Trainer trainer)
    {
        Pokemon pokemon = pokemonPrefab.GetComponent<Pokemon>();
        if (pokemon.cost <= trainer.money)
        {
            for (int i = 0; i < Trainer.CanWaitPokemonsNumber; i++)
            {
                if (trainer.waitingPokemons[i] == null)
                {
                    trainer.money -= pokemon.cost;
                    pokemon = Instantiate(pokemonPrefab).GetComponent<Pokemon>();
                    pokemon.trainer = trainer;
                    gameManager.waitingBoards[trainer].SetPokemon(new Vector2Int(0, i), pokemon);
                    int count = 0;
                    foreach(Pokemon placedpokemon in trainer.placedPokemons.Keys)
                    {
                        if(placedpokemon.name == pokemon.name)
                        {
                            count += 1;
                        }
                    }
                    foreach (Pokemon waitingpokemon in trainer.waitingPokemons)
                    {
                        if(waitingpokemon == null)
                        {
                            continue;
                        }
                        if (waitingpokemon.name == pokemon.name)
                        {
                            count += 1;
                        }
                    }
                    //Debug.Log(count);
                    if (count == 3)
                    {
                        List<Pokemon> placedPokemonList = new List<Pokemon>(trainer.placedPokemons.Keys);
                        foreach (Pokemon placedPokemons2 in placedPokemonList)
                        {
                            Debug.Log("테스트1");
                            if (placedPokemons2.name == pokemon.name)
                            {
                                gameManager.chessBoards[trainer].RemovePokemon(placedPokemons2);
                                Destroy(placedPokemons2.gameObject);
                            }
                        }
                        foreach (Pokemon waitingpokemon2 in trainer.waitingPokemons)
                        {                        
                            if (waitingpokemon2 == null)
                            {                              
                                continue;
                            }
                            if (waitingpokemon2.name == pokemon.name)
                            {                              
                                gameManager.waitingBoards[trainer].RemovePokemon(waitingpokemon2);
                                Destroy(waitingpokemon2.gameObject);
                            }
                        }
                        for (int j = 0; j < Trainer.CanWaitPokemonsNumber; j++)
                        {
                            if (trainer.waitingPokemons[j] == null)
                            {
                                Pokemon evolution = pokemonPrefab.GetComponent<Pokemon>();
                                evolution = Instantiate(pokemon.evolution).GetComponent<Pokemon>();
                                evolution.trainer = trainer;
                                gameManager.waitingBoards[trainer].SetPokemon(new Vector2Int(0, j), evolution);
                                return true;
                            }
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }
}