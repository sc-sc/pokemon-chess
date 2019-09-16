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

        attacker.spriteRenderer.color *= new Color(1f, 1f, 0f);

        yield return new WaitForSeconds(0.5f);
        attacker.spriteRenderer.material = paintWhite;
        yield return new WaitForSeconds(0.1f);
        attacker.spriteRenderer.material = previousMetrial;
        yield return new WaitForSeconds(0.5f);
        attacker.spriteRenderer.material = paintWhite;
        yield return new WaitForSeconds(0.1f);
        attacker.spriteRenderer.material = previousMetrial;
        yield return new WaitForSeconds(0.3f);
        attacker.spriteRenderer.material = paintWhite;
        yield return new WaitForSeconds(0.1f);
        attacker.spriteRenderer.material = previousMetrial;
        yield return new WaitForSeconds(0.2f);
        attacker.spriteRenderer.material = paintWhite;

        Vector3 startPosition = attacker.transform.position;
        for (float timer = 0; timer < 0.1f; timer += Time.deltaTime)
        {
            attacker.transform.position = Vector3.Lerp(startPosition, defensor.transform.position, timer / 0.1f);

            yield return null;
        }

        audioSource.PlayOneShot(hitSound);
        int damage = DamageCalculator.CalculateSkillDamage(attacker, defensor, 120, PokemonType.Electric, AttackType.Physical);
        defensor.Hit(damage, attacker, AttackType.Physical);

        if (Random.Range(0, 1f) < 0.1f)
        {
            defensor.SetStatus(PokemonStatus.Paralysis, 6f);
        }
        for (float timer = 0f; timer < 0.1f; timer += Time.deltaTime)
        {
            attacker.transform.position = Vector3.Lerp(defensor.transform.position, startPosition, timer / 0.1f);

            yield return null;
        }

        attacker.spriteRenderer.color = new Color(1f, 1f, 1f);
        attacker.spriteRenderer.material = previousMetrial;

        if (defensor.isAlive)
        {
            attacker.currentHp -= damage / 3;
            if (attacker.currentHp <= 0) attacker.currentHp = 1;
        }
        EndSkill(attacker);
    }
}
