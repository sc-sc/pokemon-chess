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
    public int baseHp = 100;
    public string ultimate_skill;
    public int actualHp
    {
        get
        {
            return DamageCalculator.GetActualHp(this);
        }
    }

    [SerializeField]
    private int _currentHp;
    public int currnetHp {
        get
        {
            return _currentHp;
        }

        set
        {
            _currentHp = value;
            pokemonUIManager.ChangeHp(this);
        }
    }
    public int baseAttack = 100;
    public int baseDefense = 100;
    public int baseSpeed = 100;

    public int ppFull = 30;
    public int initialPp = 0;

    [SerializeField]
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

    public float range = 1;
    public Pokemon attackTarget;
    private bool isOnAttack = false;

    public bool isAlive = true;

    public BattleCallbackHandler battleCallbackHandler;
    void Awake()
    {
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
                    if (!attackTarget.isAlive || !IsAttackTargetInRange())
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
                    if (IsAttackTargetInRange())
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

        float attackTime = 100f / baseSpeed;
        for (float time = 0.0f; time < attackTime; time += Time.deltaTime)
        {
            if (!attackTarget.isAlive || !IsAttackTargetInRange())
            {
                isOnAttack = false;
                yield break;
            }
            yield return null;
        }

        int damage = DamageCalculator.CalculateDamage(this, attackTarget);
        currentPp += 5;
        if(currentPp >= 20)
        {
            currentPp -= 20;
            Ultimate(this);
            damage *= 2;
        }
        Debug.Log(this.name + damage);
        attackTarget.Hit(damage, this);
        isOnAttack = false;
    }

    public void Hit(int damage, Pokemon by)
    {
        if (isAlive)
        {
            currnetHp -= damage;
            currentPp += 3;

            if (currnetHp <= 0)
            {
                currentState = PokemonState.Idle;
                StopAllCoroutines();
                isAlive = false;
                battleCallbackHandler.PokemonDead(this);
            }
            StartCoroutine(HitAction());
        }
    }
    private IEnumerator HitAction()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, 0, 0);

        for (float time = 0; time < 0.5f; time += 0.1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g + 0.2f, spriteRenderer.color.b + 0.2f);
            yield return new WaitForSeconds(0.1f);
        } if (currnetHp <= 0)
        {
            pokemonUIManager.RemovePokemonUI(this);
            gameObject.SetActive(false);
        }
    }
    public int GetTotalCost()
    {
        return cost * (int ) Mathf.Pow(3, evolutionPhase - 1);
    }

    private bool IsAttackTargetInRange()
    {
        Vector2 distance = attackTarget.transform.position - transform.position;

        return battleCallbackHandler.IsAttackTargetInRange(this);
    }

    public void MoveTo(Vector3 position)
    {
        StartCoroutine(MoveAction(position));
    }

    private IEnumerator MoveAction(Vector3 position)
    {
        Vector3 startPosition = transform.position;

        for (float time = 0f; time < 0.25f; time += Time.deltaTime)
        {
            if (time < 0.125f)
            {
                spriteRenderer.transform.position += new Vector3(0, Time.deltaTime * 2);
            } else
            {
                spriteRenderer.transform.position -= new Vector3(0, Time.deltaTime * 2);
            }

            transform.position = Vector2.Lerp(startPosition, position, time * 4);

            yield return new WaitForEndOfFrame();
        }

        transform.position = position;
        spriteRenderer.transform.localPosition = Vector3.zero;
    }
    public void Ultimate(Pokemon pokemon)
    {
        Debug.Log("필살기" + pokemon.ultimate_skill);
        StartCoroutine(Ultimate_Action());
    }
    private IEnumerator Ultimate_Action()
    {
        spriteRenderer.color = new Color(0, spriteRenderer.color.g , 0);

        for (float time = 0; time < 0.5f; time += 0.1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r + 0.2f, spriteRenderer.color.g, spriteRenderer.color.b + 0.2f);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
