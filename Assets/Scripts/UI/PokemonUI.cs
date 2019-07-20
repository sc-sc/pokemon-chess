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
        float temp = (float)pokemon.HP_current / pokemon.HP_full;
        for (float time = 0f; time < 0.2f; time += 0.1f)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, temp, time * 5);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
