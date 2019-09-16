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

        yield return new WaitForSeconds(2f);

        animator.speed *= 2f;
        attacker.SetStatus(PokemonStatus.Sleep, 6f);
        attacker.currentHp = attacker.actualHp;
    }
}
