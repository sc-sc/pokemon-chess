using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokemonUIManager : MonoBehaviour
{   
    public GameObject pokemonUIPrefab;
    private Dictionary<Pokemon, GameObject> pokemonUIDictionary = new Dictionary<Pokemon, GameObject>();

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

    public void ChangeHp(Pokemon pokemon)
    {
        StartCoroutine(Change_Hp_Action(pokemon));
    }

    private IEnumerator Change_Hp_Action(Pokemon pokemon)
    {
        float temp = (float)pokemon.HP_current / pokemon.HP_full;
        Image hpBar = pokemonUIDictionary[pokemon].transform.Find("Bar").Find("HpBar").GetComponent<Image>();
        for (float time = 0f; time < 0.2f; time += Time.fixedDeltaTime)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, temp, time * 5) ;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

}
