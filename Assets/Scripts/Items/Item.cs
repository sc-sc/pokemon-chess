using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item: MonoBehaviour
{
    public abstract float GetStatBonus(PokemonStat stat);
}
