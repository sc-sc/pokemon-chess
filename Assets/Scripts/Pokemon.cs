using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
public enum PokemonType
{
    Normal, Fire, Water, Grass, Electric, Ice, Fight, Poison, Ground, Fly, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy
}

public enum PokemonStatus
{
    None, Poison, Freeze, Paralysis, Toxic, Sleep, Burn
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
    [SerializeField] public PokemonName pokemonName;

    TransformAccessArray transforms;
    JobHandle moveJobHandle;

    public PokemonState currentState = PokemonState.Idle;
    private PokemonUIManager pokemonUIManager;

    public Trainer trainer;
    public int cost;
    public int baseHp = 100;

    [SerializeField]
    private List<Item> items = new List<Item>();
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

    public bool isAlive = false;

    public BattleCallbackHandler battleCallbackHandler;

    private IEnumerator attackCoroutine;

    private Skill skill;

    private Vector3 originalPosition;

    private AudioSource audioSource;
    private AudioClip hitSound;

    [SerializeField]
    private PokemonStatus currentStatus = PokemonStatus.None;
    [SerializeField]
    private float statusDuraitionTime = 0;
    private float statusTime = 0;

    private GameObject sleepEffectPrefab;
    private GameObject sleepEffect;

    [SerializeField]
    private List<Ability> abilities;

    private AudioClip paralysisSound;

    private Material paintWhite, defulatMaterial;

    void Awake()
    {
        sleepEffectPrefab = Resources.Load("Prefabs/SleepEffect") as GameObject;
        isAlive = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        SoundFactory soundFactory = FindObjectOfType<SoundFactory>();
        hitSound = soundFactory.hitSound;
        paralysisSound = soundFactory.paralysisSound;

        pokemonUIManager = FindObjectOfType<PokemonUIManager>();
        transforms = new TransformAccessArray(2, -1);
        transforms.Add(transform);
        transforms.Add(spriteRenderer.transform);

        initRank();
        skill = GetComponent<Skill>();
        paintWhite = Resources.Load("Materials/PaintWhite") as Material;
        defulatMaterial = spriteRenderer.material;
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
                    if (!IsAttackTargetInRange())
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
                if (!CheckParalysis())
                {
                    currentState = PokemonState.Skill;
                    Pokemon skillTarget = GetNearstEnemyPokemon();

                    skill.UseSkill(this, skillTarget);
                }
            }
        }

        if (isAlive)
        {
            UpdateStatus();
        }
    }

    public bool CheckParalysis()
    {
        if (currentStatus == PokemonStatus.Paralysis && Random.Range(0f, 1f) < 0.25f)
        {
            StartCoroutine(ParalysisEffect());
            return true;
        }

        return false;
    }

    private void StopAttack()
    {
        StopCoroutine(attackCoroutine);
        StartCoroutine(BackToOriginalPosition(0.1f));
    }
    
    private IEnumerator BackToOriginalPosition(float time)
    {
        Vector3 startPosition = transform.position;
        for (float timer = 0; timer < time; timer += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPosition, originalPosition, timer / time);
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

        float attackTime = 0.4f + 100f / DamageCalculator.GetActualStat(baseSpeed, PokemonStat.Speed, this);


        if (CheckParalysis())
        {
            yield return new WaitForSeconds(attackTime);
            isOnAttack = false;
            yield break;
        }

        StartCoroutine(CheckCanAttack());

        yield return new WaitForSeconds(attackTime - 0.6f);

        StopAnimation();
        yield return new WaitForSeconds(0.2f);

        if (range == 1)
        {
            for (float timer = 0f; timer < 0.2f; timer += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(originalPosition, attackTarget.transform.position, timer / 0.2f);
                yield return null;
            }

            audioSource.PlayOneShot(hitSound);
            StartCoroutine(BackToOriginalPosition(0.2f));
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
        }

        int damage = DamageCalculator.CalculateBasicAttackDamage(this, attackTarget);
        currentPp += 5;
        attackTarget.Hit(damage, this, AttackType.Physical);
        isOnAttack = false;

        StartAnimation();
    }

    private IEnumerator CheckCanAttack()
    {
        while (isOnAttack)
        {
            if (!IsAttackTargetInRange())
            {
                isOnAttack = false;
                StopAttack();
                yield break;
            }

            yield return null;
        }
    }

    private List<Ability> GetAbilities()
    {
        return abilities;
    }

    public void Hit(int damage, Pokemon by, AttackType attackType = AttackType.Speical)
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
                if (skill != null)
                {
                    skill.StopAllCoroutines();
                }
                battleCallbackHandler.PokemonDead(this);
            }

            StartCoroutine(HitAction());

            CheckHitSideEffect(by, attackType);
        }
    }
    private void CheckHitSideEffect(Pokemon attackPokemon, AttackType attackType)
    {
        if (attackType == AttackType.Physical && abilities.Contains(Ability.정전기))
        {
            if (Random.Range(0f, 1f) < 0.3f)
            {
                attackPokemon.SetStatus(PokemonStatus.Paralysis, 6f);
            }
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

    public bool IsAttackTargetInRange()
    {
        return attackTarget.isAlive && battleCallbackHandler.IsAttackTargetInRange(this);
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
        UnsetStatus();
        StartAnimation();
        moveJobHandle.Complete();
        isOnAttack = false;
        currentState = PokemonState.Idle;
        StopAllCoroutines();
        if (skill != null)
            skill.StopAllCoroutines();
        pokemonUIManager.AddPokemonUI(this);
        currentHp = actualHp;
        currentPp = initialPp;
        isAlive = false;
        gameObject.SetActive(true);
        spriteRenderer.color = new Color(1, 1, 1);
        spriteRenderer.flipX = false;
        initRank();
        animator.speed = 1f;
        spriteRenderer.material = defulatMaterial;
    }
    public void SetStatus(PokemonStatus status, float durationTime)
    {
        Debug.Log(status + "에 걸림");

        if (status == PokemonStatus.Paralysis && HasType(PokemonType.Electric))
            return;

        UnsetStatus();
        currentStatus = status;
        statusDuraitionTime = durationTime;

        switch (status)
        {
            case PokemonStatus.Sleep:
                StopAnimation();
                currentState = PokemonState.Idle;
                sleepEffect = Instantiate(sleepEffectPrefab, uiTransform);
                break;

            case PokemonStatus.Paralysis:
                spriteRenderer.color = new Color(1f, 1f, 0f);
                animator.speed *= 0.5f;
                StartCoroutine(ParalysisEffect());
                break;
        }
    }

    public bool HasType(PokemonType type)
    {
        foreach (PokemonType pokemonType in types)
        {
            if (type == pokemonType)
                return true;
        }

        return false;
    }
    public void UnsetStatus()
    {
        switch (currentStatus)
        {
            case PokemonStatus.Sleep:
                StartAnimation();
                currentState = PokemonState.Move;
                Destroy(sleepEffect);
                break;

            case PokemonStatus.Paralysis:
                spriteRenderer.color = new Color(1f, 1f, 1f);
                animator.speed *= 2f;
                break;
        }

        currentStatus = PokemonStatus.None;
        statusTime = 0f;
    }

    public PokemonStatus GetCurrentStatus()
    {
        return currentStatus;
    }

    private void UpdateStatus()
    {
        switch (currentStatus)
        {
            case PokemonStatus.Paralysis:
                spriteRenderer.color *= new Color(1f, 1f, 0f);
                break;
        }

        statusTime += Time.deltaTime;
        if (statusTime >= statusDuraitionTime)
            UnsetStatus();
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
        if (statType == PokemonStat.Speed)
        {
            animator.speed /= DamageCalculator.StatRank(PokemonStat.Speed, this);
            statRank[statType] += amount;
            animator.speed *= DamageCalculator.StatRank(PokemonStat.Speed, this);
        }
        else
        {
            statRank[statType] += amount;
        }
    }

    public Pokemon GetNearstEnemyPokemon()
    {
        return battleCallbackHandler.GetNearstEnemyPokemon(this);
    }

    public List<Item> GetItems()
    {
        return items;
    }

    private IEnumerator ParalysisEffect()
    {
        audioSource.PlayOneShot(paralysisSound);

        for (int count = 0; count < 4; count++)
        {
            if (count % 2 == 0)
                spriteRenderer.material = paintWhite;
            else
                spriteRenderer.material = defulatMaterial;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
