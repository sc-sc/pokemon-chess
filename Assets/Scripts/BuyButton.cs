using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public Sprite[] backgroundSprites;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image image;
    public void SetPokemonInformation(GameObject pokemonPrefab)
    {
        Pokemon pokemon = pokemonPrefab.GetComponent<Pokemon>();
        background.sprite = backgroundSprites[pokemon.cost - 1];
        costText.text = pokemon.cost.ToString();
        image.sprite = pokemonPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        image.SetNativeSize();
    }
}
