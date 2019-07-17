using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            uiKeyValuePair.Value.transform.position = Camera.main.WorldToScreenPoint(uiKeyValuePair.Key.uiTransform.position);
            uiKeyValuePair.Value.transform.position -= new Vector3(0, 0, uiKeyValuePair.Key.spriteRenderer.sortingOrder);
        }
    }

    public void AddPokemonUI(Pokemon pokemon)
    {
        if (!pokemonUIDictionary.ContainsKey(pokemon))
        {
            pokemonUIDictionary[pokemon] = Instantiate(pokemonUIPrefab, transform);
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
