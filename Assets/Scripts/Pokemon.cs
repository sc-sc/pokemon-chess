using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
public enum PokemonType
{
    Normal, Fire, Water, Grass, Electric, Ice, Fight, Poison, Ground, Fly, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}

public enum PokemonState
{
    Idle, Attack, Move, Skill
}

public enum PokemonStat
{
    Hp, Attack, Defense, SpecialAttack, SpecialDefense, Speed
}

public class Pokemon : MonoBehaviour
{
    TransformAccessArray transforms;
    JobHandle moveJobHandle;

    public PokemonState currentState = PokemonState.Idle;
    private PokemonUIManager pokemonUIManager;

    public Trainer trainer;
    public int cost;
    public int baseHp = 100;
    public int actualHp
    {
        get
        {
            return DamageCalculator.GetActualHp(this);
        }
    }

    [SerializeField]
    private int _currentHp;
    public int currentHp {
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
    public int baseSpecialAttack = 100;
    public int baseSpecialDefense = 100;
    public int baseSpeed = 100;

    public Dictionary<PokemonStat, int> statRank = new Dictionary<PokemonStat, int>();

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
    private Animator animator;
    public Transform uiTransform;
    public PokemonType[] types = new PokemonType[2];

    public float range = 1;
    public Pokemon attackTarget;
    private bool isOnAttack = false;

    public bool isAlive = true;

    public BattleCallbackHandler battleCallbackHandler;

    private IEnumerator attackCoroutine;

    private Skill skill;

    private Vector3 originalPosition;

    private AudioSource audioSource;
    private AudioClip hitSound;
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        hitSound = FindObjectOfType<SoundFactory>().hitSound;
        pokemonUIManager = FindObjectOfType<PokemonUIManager>();
        transforms = new TransformAccessArray(2, -1);
        transforms.Add(transform);
        transforms.Add(spriteRenderer.transform);

        initRank();
        skill = GetComponent<Skill>();
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
                        attackCoroutine = AttackAction();
                        StartCoroutine(attackCoroutine);
                    }
                }

                break;

            case PokemonState.Move:
                StartAnimation();
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
        
        if ((currentState == PokemonState.Attack || currentState == PokemonState.Move) 
            && currentPp >= ppFull)
        {
            currentPp = 0;

            if (skill != null)
            {
                isOnAttack = false;
                StopAttack();
                currentState = PokemonState.Skill;
                Pokemon skillTarget = GetNearstEnemyPokemon();

                skill.UseSkill(this, skillTarget);
            }
        }
    }

    private void StopAttack()
    {
        StopCoroutine(attackCoroutine);
        StartCoroutine(BackToOriginalPosition(5));
    }
    
    private IEnumerator BackToOriginalPosition(int untilFrame)
    {
        Vector3 startPosition = transform.position;
        for (int frame = 0; frame < untilFrame; frame++)
        {
            transform.position = Vector3.Lerp(startPosition, originalPosition, (float)frame / untilFrame);
            yield return null;
        }

        transform.position = originalPosition;
    }

    void OnDestroy()
    {
        moveJobHandle.Complete();
        transforms.Dispose();
    }

    private IEnumerator AttackAction()
    {
        isOnAttack = true;
        originalPosition = battleCallbackHandler.GetPosition(this);
        if (attackTarget.transform.position.x > transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        int attackFrame = 10 + (int) ((100f / DamageCalculator.GetActualStat(baseSpeed, PokemonStat.Speed, this)) * 60f);
        for (int frame = 0; frame < attackFrame; frame++)
        {
            if (!attackTarget.isAlive || !IsAttackTargetInRange())
            {
                isOnAttack = false;
                yield break;
            }

            if (range == 1)
            {
                if (frame == attackFrame - 30)
                    StopAnimation();

                if (frame >= attackFrame - 10)
                {
                    if (frame < attackFrame - 5)
                    {
                        transform.position = Vector3.Lerp(originalPosition, attackTarget.transform.position, (float) (frame) / (attackFrame - 5));
                    } else if (frame == attackFrame - 5)
                    {
                        audioSource.PlayOneShot(hitSound);
                        StartCoroutine(BackToOriginalPosition(5));
                    }
                }
            }

            yield return null;
        }

        StartAnimation();

        int damage = DamageCalculator.CalculateBasicAttackDamage(this, attackTarget);
        currentPp += 5;
        attackTarget.Hit(damage, this);
        isOnAttack = false;
    }

    public void Hit(int damage, Pokemon by)
    {
        if (isAlive)
        {
            currentHp -= damage;
            currentPp += 3;


            if (currentHp <= 0)
            {
                StopAnimation();
                currentState = PokemonState.Idle;
                StopAllCoroutines();
                isAlive = false;
                battleCallbackHandler.PokemonDead(this);
                if (skill != null)
                {
                    skill.StopAllCoroutines();
                }
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
        } if (currentHp <= 0)
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
            MoveJob moveJob = new MoveJob();
            moveJob.startPosition = startPosition;
            moveJob.position = position;
            moveJob.time = time;
            moveJob.deltaTime = Time.deltaTime;

            moveJobHandle = moveJob.Schedule(transforms);
            yield return null;
        }

        transform.position = position;
        spriteRenderer.transform.localPosition = Vector3.zero;
    }

    private struct MoveJob : IJobParallelForTransform
    {
        public float time;
        public float deltaTime;
        public Vector3 startPosition;
        public Vector3 position;
        public void Execute(int index, TransformAccess transform)
        {
            if (time < 0.125f)
            {
                if (index == 1)
                    transform.position += new Vector3(0, deltaTime * 2);
            }
            else
            {
                if (index == 1)
                    transform.position -= new Vector3(0, deltaTime * 2);
            }

            if (index == 0)
                transform.position = Vector2.Lerp(startPosition, position, time * 4);
        }
    }
    public void Reset()
    {
        moveJobHandle.Complete();
        isOnAttack = false;
        currentState = PokemonState.Idle;
        StopAllCoroutines();
        pokemonUIManager.AddPokemonUI(this);
        currentHp = actualHp;
        currentPp = initialPp;
        isAlive = true;
        gameObject.SetActive(true);
        spriteRenderer.color = new Color(1, 1, 1);
        spriteRenderer.flipX = false;
        initRank();
        animator.speed = 1f;
    }

    private void initRank()
    {
        foreach (PokemonStat stat in System.Enum.GetValues(typeof(PokemonStat))) {
            statRank[stat] = 0;
        }
    }

    public void StopAnimation()
    {
        animator.enabled = false;
    }

    public void StartAnimation()
    {
        animator.enabled = true;
    }

    public void RankUp(PokemonStat statType, int amount)
    {
        statRank[statType] += amount;

        if (statType == PokemonStat.Speed)
        {
            animator.speed *= DamageCalculator.StatRank(PokemonStat.Speed, this);
        }
    }

    public Pokemon GetNearstEnemyPokemon()
    {
        return battleCallbackHandler.GetNearstEnemyPokemon(this);
    }
}
