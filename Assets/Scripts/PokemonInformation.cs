using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonInformation : MonoBehaviour
{
    public Sprite[] backgroundSprites;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image image;

    private Pokemon showingPokemon;
    public void ShowPokemonInformation(Pokemon pokemon)
    {
        background.gameObject.SetActive(true);
        background.sprite = backgroundSprites[pokemon.cost - 1];
        costText.text = pokemon.cost.ToString();
        image.sprite = pokemon.GetComponentInChildren<SpriteRenderer>().sprite;
        image.SetNativeSize();

        showingPokemon = pokemon;
    }

    public void UnshowPokemonInformation()
    {
        background.gameObject.SetActive(false);

        showingPokemon = null;
    }

    void Update()
    {
        if (showingPokemon != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(showingPokemon.transform.position + new Vector3(0f, 2.5f));
        }
    }
}
