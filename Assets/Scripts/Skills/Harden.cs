using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harden : Skill
{
    [SerializeField]
    private AudioClip hardenSound;
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        base.UseSkill(attacker, defensor);

        StartCoroutine(HardenAction(attacker));
    }

    private IEnumerator HardenAction(Pokemon attacker)
    {
        GetComponent<AudioSource>().PlayOneShot(hardenSound);

        attacker.StopAnimation();
        GameObject hardenEffect = new GameObject("HardenEffect");
        hardenEffect.transform.SetParent(transform, false);
        hardenEffect.AddComponent<SpriteRenderer>();
        SpriteRenderer effectSprite = hardenEffect.GetComponent<SpriteRenderer>();
        effectSprite.sprite = attacker.spriteRenderer.sprite;
        effectSprite.material = Resources.Load("Materials/PaintWhite") as Material;
        effectSprite.sortingLayerName = "ForeShadow";
        effectSprite.flipX = attacker.spriteRenderer.flipX;
        effectSprite.color -= new Color(0, 0, 0, 1);

        for (int frame = 0; frame < 120; frame++)
        {
            if (frame < 30)
            {
                effectSprite.color += new Color(0, 0, 0, 0.01f);
            } else if (frame < 60)
            {
                effectSprite.color -= new Color(0, 0, 0, 0.01f);
            } else if (frame < 90)
            {
                effectSprite.color += new Color(0, 0, 0, 0.01f);
            } else
            {
                effectSprite.color -= new Color(0, 0, 0, 0.01f);
            }
            yield return null;
        }

        attacker.RankUp(PokemonStat.Defense, 1);

        Destroy(hardenEffect);
        EndSkill(attacker);
    }
}
