using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item: MonoBehaviour
{
    [SerializeField]
    public ItemName itemName;

    public Sprite sprite;
    public abstract float GetStatBonus(PokemonStat stat, Pokemon pokemon);
}
