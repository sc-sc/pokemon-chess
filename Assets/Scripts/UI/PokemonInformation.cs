using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonInformation : MonoBehaviour
{
    [System.Serializable]
    public struct PokemonTypeColor
    {
        [SerializeField] public PokemonType type;
        [SerializeField] public Color color;
    }

    public PokemonTypeColor[] pokemonTypeColors;
    public Dictionary<PokemonType, Color> pokemonTypeColorDictionary;
   
    public Sprite[] backgroundSprites;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Transform typesLayout;

    [SerializeField]
    private GameObject typePrefab;

    private Pokemon showingPokemon;

    void Start()
    {
        pokemonTypeColorDictionary = new Dictionary<PokemonType, Color>();
        foreach (PokemonTypeColor pokemonTypeColor in pokemonTypeColors)
        {
            pokemonTypeColorDictionary[pokemonTypeColor.type] = pokemonTypeColor.color;
        }
    }
    public void ShowPokemonInformation(Pokemon pokemon)
    {
        foreach (Transform type in typesLayout.transform)
        {
            Destroy(type.gameObject);
        }

        background.gameObject.SetActive(true);
        background.sprite = backgroundSprites[pokemon.cost - 1];
        costText.text = pokemon.cost.ToString();
        image.sprite = pokemon.GetComponentInChildren<SpriteRenderer>().sprite;
        image.SetNativeSize();

        showingPokemon = pokemon;
        
        for (int reverseIndex = pokemon.types.Length - 1; reverseIndex >= 0; reverseIndex--)
        {
            PokemonType type = pokemon.types[reverseIndex];
            GameObject typeObject = Instantiate(typePrefab, typesLayout);
            typeObject.GetComponent<Image>().color = pokemonTypeColorDictionary[type];
            typeObject.GetComponentInChildren<Text>().text = type.ToString();
        }
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
            transform.position = Camera.main.WorldToScreenPoint(showingPokemon.uiTransform.position);
        }
    }
}
