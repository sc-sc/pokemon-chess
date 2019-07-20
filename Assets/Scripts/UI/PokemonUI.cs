using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonUI : MonoBehaviour
{
    public Image hpBar;
    public Image ppBar;
    private IEnumerator hpActionCoroutine;
    public void ChangeHp(Pokemon pokemon)
    {
        hpActionCoroutine = ChangeHpAction(pokemon);
        StartCoroutine(hpActionCoroutine);
    }
    private IEnumerator ChangeHpAction(Pokemon pokemon)
    {
        float temp = (float)pokemon.hpCurrent / pokemon.hpFull;
        for (float time = 0f; time < 0.2f; time += Time.fixedDeltaTime)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, temp, time * 5);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void ChangePp(Pokemon pokemon)
    {
        ppBar.fillAmount = (float) pokemon.currentPp / pokemon.ppFull;
    }
}
