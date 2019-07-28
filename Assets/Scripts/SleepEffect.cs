using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject zObject;

    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 0.5f)
        {
            timer = 0f;
            StartCoroutine(MakeZ());
        }
    }

    private IEnumerator MakeZ()
    {
        GameObject zInstance = Instantiate(zObject, transform);
        SpriteRenderer zSpriteRenderer = zInstance.GetComponent<SpriteRenderer>();
        Vector3 destionation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f));

        for (float time = 0f; time < 1f; time += 0.1f)
        {
            zInstance.transform.localPosition = Vector3.Lerp(Vector3.zero, destionation, time);
            zSpriteRenderer.color -= new Color(0f, 0f, 0f, 0.1f);
            zInstance.transform.localScale += new Vector3(0.1f, 0.1f);

            yield return new WaitForSeconds(0.1f);
        }

        Destroy(zInstance);
    }
}
