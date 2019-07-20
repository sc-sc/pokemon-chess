using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonType
{
    Normal, Fire, Water, Grass, Electric, Ice, Fight, Poison, Ground, Fly, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}

public enum PokemonState
{
    Idle, Attack, Move, Skill
}

public class Pokemon : MonoBehaviour
{
    public PokemonState currentState = PokemonState.Idle;
    private PokemonUIManager pokemonUIManager;

    public Trainer trainer;
    public int cost;
    public int hpFull = 100;
    public int hpCurrent;
    public int attack = 100;
    public int defense = 100;
    public int speed = 100;

    public int ppFull = 30;
    public int initialPp = 0;

    private int _currentPp;
    public int currentPp {
        get
        {
            return _currentPp;
        }

        set
        {
            _currentPp = value;
            pokemonUIManager.ChangePp(this);
        }
    }
    public GameObject evolution;
    public int evolutionPhase = 1;
    internal SpriteRenderer spriteRenderer;
    public Transform uiTransform;
    public PokemonType[] types = new PokemonType[2];

    public int range = 1;
    private float actualRange
    {
        get { return range * 2.5f; }
    }
    public Pokemon attackTarget;
    private bool isOnAttack = false;

    public bool isAlive = true;

    public BattleCallbackHandler battleCallbackHandler;
    void Awake()
    {
        hpCurrent = hpFull;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pokemonUIManager = FindObjectOfType<PokemonUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = -(int) (transform.position.y * 2);

        switch (currentState)
        {
            case PokemonState.Attack:
                if (!isOnAttack)
                {
                    if ((!attackTarget.isAlive) || DistanceBetweenAttackTarget() > actualRange)
                        currentState = PokemonState.Move;
                    else
                    {
                        StartCoroutine(AttackAction());
                    }
                }

                break;

            case PokemonState.Move:
                if (attackTarget != null && attackTarget.isAlive)
                {
                    if (DistanceBetweenAttackTarget() <= actualRange)
                        currentState = PokemonState.Attack;
                } else
                    battleCallbackHandler.LostAttackTarget(this);
                break;

            default:
                break;
        }
    }

    private IEnumerator AttackAction()
    {
        isOnAttack = true;
        if (attackTarget.transform.position.x > transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        float attackTime = 100f / speed;
        for (float time = 0.0f; time < attackTime; time += Time.deltaTime)
        {
            if (!attackTarget.isAlive || DistanceBetweenAttackTarget() > actualRange)
            {
                isOnAttack = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        int damage = CalculateDamage();
        currentPp += 5;
        attackTarget.Hit(damage, this);
        isOnAttack = false;
    }

    public void Hit(int damage, Pokemon by)
    {
        hpCurrent -= damage;
        currentPp += 5;
        pokemonUIManager.ChangeHp(this);
        if (hpCurrent <= 0)
        {
            StopAllCoroutines();
            isAlive = false;
            battleCallbackHandler.PokemonDead(this);
        }
        StartCoroutine(HitAction());
    }

    public int CalculateDamage()
    {
        return (int) ((((((22) * 30 * attack / 50) / attackTarget.defense) * Mod1()) + 2)
            * Critical() * Mod2() * TypeBonus() * Mod3());
    }

    private float Mod1()
    {
        return 1;
    }

    private float Mod2()
    {
        return 1;
    }

    private float Mod3()
    {
        return 1;
    }

    private float TypeBonus()
    {
        return 1;
    }

    private float Critical()
    {
        return 1;
    }
    private IEnumerator HitAction()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, 0, 0);

        for (float time = 0; time < 0.5f; time += 0.1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g + 0.2f, spriteRenderer.color.b + 0.2f);
            yield return new WaitForSeconds(0.1f);
        } if (hpCurrent <= 0)
        {
            pokemonUIManager.RemovePokemonUI(this);
            gameObject.SetActive(false);
        }
    }

    public int GetTotalCost()
    {
        return cost * (int ) Mathf.Pow(3, evolutionPhase - 1);
    }

    private float DistanceBetweenAttackTarget()
    {
        return Vector2.Distance(transform.position, attackTarget.transform.position);
    }
}
