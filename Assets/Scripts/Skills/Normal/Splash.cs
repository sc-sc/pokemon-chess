using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : Skill
{
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        StartCoroutine(SplashAction(attacker));
    }
    
    private IEnumerator SplashAction(Pokemon attacker)
    {
        attacker.animator.speed *= 2f;
        attacker.currentState = PokemonState.Idle;
        attacker.Jump(1f);
        yield return new WaitForSeconds(1f);
        attacker.Jump(1f);
        yield return new WaitForSeconds(1f);
        attacker.animator.speed /= 2f;

        EndSkill(attacker);
    }
}
