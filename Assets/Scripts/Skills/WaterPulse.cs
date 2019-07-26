using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPulse : Skill
{
    [SerializeField]
    private AudioClip waterPulseSound;
    [SerializeField]
    private GameObject waterPulseEffect;
    [SerializeField]
    private AudioClip hitSound;
    
    public int baseDamage = 60;
    public override void UseSkill(Pokemon attacker, Pokemon defensor)
    {
        base.UseSkill(attacker, defensor);
        StartCoroutine(WaterPulseAction(attacker, defensor));
    }

    private IEnumerator WaterPulseAction(Pokemon attacker, Pokemon defensor)
    {
        Transform effectTransform = Instantiate(waterPulseEffect).transform;
        effectTransform.localScale = Vector2.zero;
        effectTransform.position = transform.position;
        Destroy(effectTransform.gameObject, 3f);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(waterPulseSound);

        Vector3 firePosition = transform.position + new Vector3(0f, 0.6f);

        attacker.StopAnimation();
        for (int frame = 0; frame < 120; frame++)
        {
            effectTransform.localScale += new Vector3(0.01f, 0.01f);

            if (frame < 30)
            {
                effectTransform.position += new Vector3(0, 0.02f);
            } else
            {
                effectTransform.position = Vector2.Lerp(firePosition, defensor.transform.position + new Vector3(0, 0.6f), (float)(frame - 30) / 90f);
            }

            yield return null;
        }

        Destroy(effectTransform.gameObject);
        audioSource.PlayOneShot(hitSound);
        defensor.Hit(DamageCalculator.CalculateSkillDamage(attacker, defensor, baseDamage, PokemonType.Water), attacker);
        EndSkill(attacker);
    }
}
