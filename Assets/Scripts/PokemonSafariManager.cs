using System.Collections;
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

    public GameObject evolutionEffect;

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
            int cost;
            if(player.level == 1)
            {
                cost = Random.Range(1, 3);
            }
            else if (player.level == 2 || player.level == 3)
            {
                cost = Random.Range(1, 4);
            }
            else if(player.level == 4 || player.level == 6)
            {
                cost = Random.Range(1, 5);
            }
            else
            {
                cost = Random.Range(1, 6);
            }
            int index = 0;
            if(cost == 1)
            {
                index = Random.Range(0, 2);
            }
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
                    BuyPokemon(pokemon, trainer, i);
                    return true;
                }
            }
        }
        return false;
    }

    private void BuyPokemon(Pokemon pokemon, Trainer trainer, int waitingBoardIndex)
    {
        trainer.money -= pokemon.cost;
        pokemon = Instantiate(pokemon.gameObject).GetComponent<Pokemon>();
        pokemon.trainer = trainer;
        gameManager.waitingBoards[trainer].SetPokemon(new Vector2Int(0, waitingBoardIndex), pokemon);

        StartCoroutine(StartEvolutionIfPokemonCan(trainer, pokemon));
    }

    public IEnumerator StartEvolutionIfPokemonCan(Trainer trainer, Pokemon pokemon)
    {
        int samePokemonCount = 0;
        List<Pokemon> placedSamePokemonList = new List<Pokemon>();

        foreach (Pokemon placedPokemon in trainer.placedPokemons.Keys)
        {
            if (placedPokemon.name == pokemon.name)
            {
                samePokemonCount += 1;
                placedSamePokemonList.Add(placedPokemon);
            }
        }

        List<int> waitingSamePokemonsIndex = new List<int>();
        for (int i = 0; i < Trainer.CanWaitPokemonsNumber; i++)
        {
            Pokemon waitingPokemon = trainer.waitingPokemons[i];
            if (waitingPokemon != null && waitingPokemon.name == pokemon.name)
            {
                samePokemonCount += 1;
                waitingSamePokemonsIndex.Add(i);
            }
        }

        if (samePokemonCount == 3)
        {
            Dictionary<PokemonPlaceableBoard, Vector2Int> placeEvolvedPokemonTo = new Dictionary<PokemonPlaceableBoard, Vector2Int>();

            ChessBoard chessBoard = gameManager.chessBoards[trainer];
            WaitingBoard waitingBoard = gameManager.waitingBoards[trainer];

            if (pokemon.evolution == null)
            {
                yield break;
            }

            Pokemon evolution = Instantiate(pokemon.evolution).GetComponent<Pokemon>();
            evolution.trainer = trainer;
            evolution.cost = pokemon.cost;
            evolution.evolutionPhase = pokemon.evolutionPhase + 1;

            Animator evolvedPokemonAnimator = evolution.GetComponentInChildren<Animator>();
            evolvedPokemonAnimator.enabled = false;

            SpriteRenderer evolvedPokemonSR = evolution.GetComponentInChildren<SpriteRenderer>();
            Sprite evolveFromSprite = evolvedPokemonSR.sprite;
            Sprite evolveToSprite = evolvedPokemonSR.sprite;
            Material defaultMaterial = evolvedPokemonSR.material;

            foreach (Pokemon placedPokemon in placedSamePokemonList)
            {
                if (placeEvolvedPokemonTo.Count == 0)
                {
                    placeEvolvedPokemonTo[chessBoard] = chessBoard.GetIndex(placedPokemon);
                    evolveFromSprite = placedPokemon.GetComponentInChildren<SpriteRenderer>().sprite;
                    Destroy(placedPokemon.gameObject);
                } else
                {
                    StartCoroutine(FlyToEvolvedPokemon(placedPokemon, evolution));
                }
                chessBoard.RemovePokemon(placedPokemon);
            }

            foreach (int waitingPokemonIndex in waitingSamePokemonsIndex)
            {
                Pokemon waitingPokemon = trainer.waitingPokemons[waitingPokemonIndex];
                if (placeEvolvedPokemonTo.Count == 0)
                {
                    placeEvolvedPokemonTo[waitingBoard] = waitingBoard.GetIndex(waitingPokemon);
                    evolveFromSprite = waitingPokemon.GetComponentInChildren<SpriteRenderer>().sprite;
                    Destroy(waitingPokemon.gameObject);
                } else
                {
                    StartCoroutine(FlyToEvolvedPokemon(waitingPokemon, evolution));
                }
                waitingBoard.RemovePokemon(waitingPokemon);
            }

            foreach (KeyValuePair<PokemonPlaceableBoard, Vector2Int> placePair in placeEvolvedPokemonTo)
            {
                placePair.Key.SetPokemon(placePair.Value, evolution);
            }

            evolvedPokemonSR.material = Resources.Load("Materials/PaintWhite") as Material;
            for (float time = 0f; time < 0.45f; time+=0.05f)
            {
                if (time < 0.15f)
                {
                    evolution.transform.localScale += new Vector3(0.05f, 0.05f);
                } else if (time < 0.3f)
                {
                    evolution.transform.localScale -= new Vector3(0.1f, 0.1f);
                    evolvedPokemonSR.sprite = evolveFromSprite;
                }
                else
                {
                    evolution.transform.localScale += new Vector3(0.05f, 0.05f);
                    evolvedPokemonSR.sprite = evolveToSprite;
                }
                yield return new WaitForSeconds(0.05f);
            }

            evolvedPokemonSR.material = defaultMaterial;
            Destroy(Instantiate(evolutionEffect, evolution.transform), 3f);
            evolvedPokemonAnimator.enabled = true;

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(StartEvolutionIfPokemonCan(trainer, evolution));
        }
    }

    private IEnumerator FlyToEvolvedPokemon(Pokemon pokemon, Pokemon evolvedPokemon)
    {
        for (float time = 0f; time < 0.2f; time += Time.fixedDeltaTime)
        {
            pokemon.transform.position = Vector3.Lerp(pokemon.transform.position, evolvedPokemon.transform.position, time * 5);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        Destroy(pokemon.gameObject);
    }
}