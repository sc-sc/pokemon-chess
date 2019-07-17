using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokemonUIManager : MonoBehaviour
{   
    public GameObject pokemonUIPrefab;
    private Dictionary<Pokemon, GameObject> pokemonUIDictionary;
    void Start()
    {
        pokemonUIDictionary = new Dictionary<Pokemon, GameObject>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach (KeyValuePair<Pokemon, GameObject> uiKeyValuePair in pokemonUIDictionary)
        {
            uiKeyValuePair.Value.transform.position = uiKeyValuePair.Key.uiTransform.position;
            uiKeyValuePair.Value.GetComponent<Canvas>().sortingOrder = uiKeyValuePair.Key.spriteRenderer.sortingOrder;
        }
    }

    public void AddPokemonUI(Pokemon pokemon)
    {
        if (!pokemonUIDictionary.ContainsKey(pokemon))
        {
            GameObject pokemonUI = Instantiate(pokemonUIPrefab, transform);
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
}
