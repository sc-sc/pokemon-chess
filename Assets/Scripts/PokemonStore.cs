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

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        salePokemonButtons = new GameObject[SalePokemonsCount];
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