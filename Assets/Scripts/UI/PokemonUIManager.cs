using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokemonUIManager : MonoBehaviour
{   
    public GameObject pokemonUIPrefab;
    private Dictionary<Pokemon, PokemonUI> pokemonUIDictionary = new Dictionary<Pokemon, PokemonUI>();
    public Sprite enemyHpBarSprite;

    // Update is called once per frame
    void LateUpdate()
    {
        foreach (KeyValuePair<Pokemon, PokemonUI> uiKeyValuePair in pokemonUIDictionary)
        {
            uiKeyValuePair.Value.transform.position = uiKeyValuePair.Key.uiTransform.position;
            uiKeyValuePair.Value.GetComponent<Canvas>().sortingOrder = uiKeyValuePair.Key.spriteRenderer.sortingOrder;
        }
    }

    public void AddPokemonUI(Pokemon pokemon)
    {
        if (!pokemonUIDictionary.ContainsKey(pokemon))
        {
            PokemonUI pokemonUI = Instantiate(pokemonUIPrefab, transform).GetComponent<PokemonUI>();
            if (!(pokemon.trainer is Player))
            {
                pokemonUI.hpBar.sprite = enemyHpBarSprite;
            }

            pokemonUIDictionary[pokemon] = pokemonUI;
            pokemonUI.GetComponent<Canvas>().sortingLayerName = pokemon.spriteRenderer.sortingLayerName;
        }
    }

    public void RemovePokemonUI(Pokemon pokemon)
    {
        if (pokemon != null)
        {
            Destroy(pokemonUIDictionary[pokemon]);
            pokemonUIDictionary.Remove(pokemon);
        }
    }

    public void ChangeHp(Pokemon pokemon)
    {
        pokemonUIDictionary[pokemon].ChangeHp(pokemon);
    }
}
