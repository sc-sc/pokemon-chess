using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringShot : Skill
{
    [SerializeField]
    private AudioClip stringShopSound;
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        StartCoroutine(StringShotAction(attacker, defensor));
    }

    private IEnumerator StringShotAction(Pokemon attacker, Pokemon defensor)
    {
        Debug.Log("실 뿜기!");
        GetComponent<AudioSource>().PlayOneShot(stringShopSound);

        yield return new WaitForSeconds(2f);

        defensor.RankUp(PokemonStat.Speed, -1);
        EndSkill(attacker);
    }
}
