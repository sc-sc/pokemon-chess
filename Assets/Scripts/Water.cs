using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 2f) {
            time = 0f;
            transform.position = new Vector3(2f + transform.position.x, 2f - transform.position.y);
        } else
        {
            time += Time.deltaTime / 10f;
            transform.position = Vector2.Lerp(new Vector2(0f, 0f), new Vector2(-2f, 2f), time);
        }
    }
}
