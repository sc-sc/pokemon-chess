using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public virtual void UseSkill(Pokemon attacker, Pokemon defensor)
    {
    }

    public virtual void EndSkill(Pokemon attacker)
    {
        attacker.StartAnimation();
        attacker.currentState = PokemonState.Move;
    }
}
