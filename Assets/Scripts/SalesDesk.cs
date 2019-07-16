using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalesDesk : MonoBehaviour, Touchable
{
    private Player player;
    public Pokemon selectedPokemon;
    public Transform soldActionStartPoint;
    public Transform soldActionTurningPoint;
    public Transform soldActionDestination;
    internal PokemonPlaceableBoard previousBoard;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    public void Moved(Vector3 to)
    {
        if (selectedPokemon == null) return;

        selectedPokemon.transform.position = to;

        if (previousBoard.HasSquare(to))
        {
            previousBoard.selectedPokemon = selectedPokemon;
            FindObjectOfType<TouchManager>().Delegate(this, previousBoard);
        }
    }

    public void Released(Vector3 at)
    {
        if (selectedPokemon == null) return;

        int cost = selectedPokemon.cost;
        Debug.Log("앙 판매");
        // 판매
        previousBoard.RemovePokemon(selectedPokemon);
        previousBoard.PlaceEnd(selectedPokemon, true);
        previousBoard.linkedBoard.RemovePokemon(selectedPokemon);
        previousBoard.linkedBoard.PlaceEnd(selectedPokemon, true);
        player.money += selectedPokemon.GetTotalCost();

        StartCoroutine(SoldAction(selectedPokemon));
        selectedPokemon = null;
    }

    private IEnumerator SoldAction(Pokemon soldPokemon)
    {
        SpriteRenderer spriteRenderer = soldPokemon.GetComponentInChildren<SpriteRenderer>();

        soldPokemon.transform.position = soldActionStartPoint.position;
        Vector3 startPosition = soldActionStartPoint.position;

        spriteRenderer.flipX = true;
        for (float time = 0f; time < 0.5; time += Time.deltaTime)
        {
            soldPokemon.transform.position = Vector2.Lerp(startPosition, soldActionTurningPoint.position, time / 0.5f);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(0.2f);
        spriteRenderer.flipX = false;
        startPosition = soldActionTurningPoint.position;
        for (float time = 0f; time < 1.5; time += Time.deltaTime)
        {
            soldPokemon.transform.position = Vector2.Lerp(startPosition, soldActionDestination.position, time / 1.5f);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(soldPokemon.gameObject);
    }

    public void Touched(Vector3 at)
    {

    }

    public void SpecialTouched(Vector3 at)
    {
        throw new System.NotImplementedException();
    }

    public void SpecialReleased(Vector3 at)
    {
        throw new System.NotImplementedException();
    }
}
