using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoltTackle : Skill
{
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip chargeSound;

    private Material paintWhite;
    void Awake()
    {
        paintWhite = Resources.Load("Materials/PaintWhite") as Material;
    }
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        StartCoroutine(VoltTackleAction(attacker, defensor));
    }

    private IEnumerator VoltTackleAction(Pokemon attacker, Pokemon defensor)
    {
        attacker.StopAnimation();
        Material previousMetrial = attacker.spriteRenderer.material;
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(chargeSound);
        for (int frame = 1; frame <= 110; frame++)
        {
            attacker.spriteRenderer.color *= new Color(1f, 1f, 0f);

            if (frame == 30 || frame == 50 || frame == 90 || frame == 100)
            {
                attacker.spriteRenderer.material = paintWhite;
            } else if (frame == 35 || frame == 55 || frame == 95)
            {
                attacker.spriteRenderer.material = previousMetrial;
            }

            yield return null;
        }

        Vector3 startPosition = attacker.transform.position;
        for (int frame = 1; frame <= 5; frame ++)
        {
            attacker.transform.position = Vector3.Lerp(startPosition, defensor.transform.position, (float)frame / 5);

            yield return null;
        }

        int damage = DamageCalculator.CalculateSkillDamage(attacker, defensor, 120, PokemonType.Electric, AttackType.Physical);
        defensor.Hit(damage, attacker, AttackType.Physical);

        if (Random.Range(0, 1f) < 0.1f)
        {
            defensor.SetStatus(PokemonStatus.Paralysis, 360);
        }
        for (int frame = 1; frame <= 5; frame++)
        {
            attacker.transform.position = Vector3.Lerp(defensor.transform.position, startPosition, (float)frame / 5);

            yield return null;
        }

        audioSource.PlayOneShot(hitSound);
        attacker.spriteRenderer.color = new Color(1f, 1f, 1f);
        attacker.spriteRenderer.material = previousMetrial;
        EndSkill(attacker);
    }
}
