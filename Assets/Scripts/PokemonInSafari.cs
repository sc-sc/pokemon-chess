using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PokemonInSafari : MonoBehaviour, Touchable
{
    private PokemonInformation pokemonInformation;

    private Vector3 selectedAt;
    private Vector3 previousPosition;
    private UnityAction buyActionCallback;
    public void Moved(Vector3 to)
    {
        transform.position = to;
    }

    public void Released(Vector3 at)
    {
        transform.position = previousPosition;
        if (at == selectedAt)
        {
            buyActionCallback();
        }
    }

    public void SpecialReleased(Vector3 at)
    {
        pokemonInformation.gameObject.SetActive(false);
    }

    public void SpecialTouched(Vector3 at)
    {
        pokemonInformation.SetPokemonInformation(GetComponentInChildren<Pokemon>());
        pokemonInformation.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 2.5f));
        pokemonInformation.gameObject.SetActive(true);
    }

    public void Touched(Vector3 at)
    {
        selectedAt = at;
        previousPosition = transform.position;
    }

    public void Init(PokemonInformation pokemonInformation, UnityAction buyActionCallback)
    {
        this.pokemonInformation = pokemonInformation;
        this.buyActionCallback = buyActionCallback;

        PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
        Sprite sprite = GetComponentInChildren<SpriteRenderer>().sprite;

        polygonCollider2D.pathCount = sprite.GetPhysicsShapeCount();
        List<Vector2> path = new List<Vector2>();

        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            polygonCollider2D.SetPath(i, path);
        }
    }
}