using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harden : Skill
{

    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        base.UseSkill(attacker, defensor);

        StartCoroutine(HardenAction(attacker));
    }

    private IEnumerator HardenAction(Pokemon attacker)
    {
        Debug.Log("단단해지기");

        attacker.StopAnimation();

        for (int frame = 0; frame < 30; frame++)
        {
            yield return null;
        }

        attacker.statRank[PokemonStat.Defense] += 1;
        attacker.StartAnimation();
        attacker.currentState = PokemonState.Move;
    }
}
