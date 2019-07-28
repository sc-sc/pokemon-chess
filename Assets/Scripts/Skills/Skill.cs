using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public abstract void UseSkill(Pokemon attacker, Pokemon defensor);

    public virtual void EndSkill(Pokemon attacker)
    {
        attacker.StartAnimation();
        attacker.currentState = PokemonState.Move;
    }
}
