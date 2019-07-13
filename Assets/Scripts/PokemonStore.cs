using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonStore : MonoBehaviour
{
    [System.Serializable]
    public struct SalePokemonPrefabs
    {
        [SerializeField] public GameObject[] prefabs;
    }

    public SalePokemonPrefabs[] salePokemonPrefabs;
    public GameObject buyButtonPrefab;
    public GameObject buyButtonLayout;
    private GameObject[] salePokemonButtons;

    public const int SalePokemonsCount = 6;

    private GameManager gameManager;

    private Player player;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        salePokemonButtons = new GameObject[SalePokemonsCount];
        player = FindObjectOfType<Player>();
    }
    public void Refresh()
    {
        foreach (GameObject salePokemonButton in salePokemonButtons)
        {
            Destroy(salePokemonButton);
        }

        for (int i = 0; i < SalePokemonsCount; i++)
        {
            int cost = Random.Range(2, 4);
            int index = 0;

            GameObject salePokemonButton = Instantiate(buyButtonPrefab, buyButtonLayout.transform);
            GameObject salePokemonPrefab = salePokemonPrefabs[cost - 1].prefabs[index];
            salePokemonButton.GetComponent<BuyButton>().SetPokemonInformation(salePokemonPrefab);
            salePokemonButtons[i] = salePokemonButton;

            salePokemonButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (TryBuy(salePokemonPrefab, FindObjectOfType<Player>()))
                {
                    Destroy(salePokemonButton);
                }
            });
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

    private bool TryBuy(GameObject pokemonPrefab, Trainer trainer)
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
                    return true;
                }
            }
        }

        return false;
    }
}