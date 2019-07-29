using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harden : Skill
{
    [SerializeField]
    private AudioClip hardenSound;
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
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


        for (float timer = 0f; timer < 2f; timer += Time.deltaTime)
        {
            if (timer < 0.5f)
            {
                effectSprite.color += new Color(0, 0, 0, Time.deltaTime);
            } else if (timer < 1f)
            {
                effectSprite.color -= new Color(0, 0, 0, Time.deltaTime);
            } else if (timer < 1.5f)
            {
                effectSprite.color += new Color(0, 0, 0, Time.deltaTime);
            } else
            {
                effectSprite.color -= new Color(0, 0, 0, Time.deltaTime);
            }

            yield return null;
        }

        attacker.RankUp(PokemonStat.Defense, 1);

        Destroy(hardenEffect);
        EndSkill(attacker);
    }
}
