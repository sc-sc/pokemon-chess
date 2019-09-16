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

        attacker.StopAnimation();

        for (float timer = 0f; timer < 1.5f; timer += Time.deltaTime)
        {
            effectTransform.localScale += new Vector3(Time.deltaTime / 2f, Time.deltaTime / 2f);
            effectTransform.position = Vector2.Lerp(attacker.transform.position, attacker.uiTransform.position, timer / 1f);

            yield return null;
        }

        for (float timer = 0f; timer < 0.5f; timer += Time.deltaTime) {
            Vector2 destionation = (defensor.transform.position + defensor.uiTransform.position) / 2f;
            effectTransform.position = Vector2.Lerp(attacker.uiTransform.position, destionation, timer / 0.5f);
        
            yield return null;
        }

        Destroy(effectTransform.gameObject);
        audioSource.PlayOneShot(hitSound);
        defensor.Hit(DamageCalculator.CalculateSkillDamage(attacker, defensor, baseDamage, PokemonType.Water), attacker);
        EndSkill(attacker);
    }
}
