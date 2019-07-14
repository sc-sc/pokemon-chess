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
    public void SetPokemonInformation(Pokemon pokemon)
    {
        background.sprite = backgroundSprites[pokemon.cost - 1];
        costText.text = pokemon.cost.ToString();
        image.sprite = pokemon.GetComponentInChildren<SpriteRenderer>().sprite;
        image.SetNativeSize();
    }
}
