using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : Skill
{
    [SerializeField]
    private AudioClip restSound;
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        StartCoroutine(RestAction(attacker));
    }

    private IEnumerator RestAction(Pokemon attacker)
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.speed *= 0.5f;
        GetComponent<AudioSource>().PlayOneShot(restSound);

        for (int frame = 0; frame < 120; frame++)
        {
            yield return null;
        }

        animator.speed *= 2f;
        attacker.SetStatus(PokemonStatus.Sleep, 360);
        attacker.currentHp = attacker.actualHp;
    }
}
