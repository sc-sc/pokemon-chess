using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    public Trainer trainer;
    public int cost;
    public int HP_full;
    public int HP_current;
    public int Attack;
    public GameObject evolution;
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = -(int) (transform.position.y * 2);
    }
}
