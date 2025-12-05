using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    // ... (Enum, Component Links 등은 동일) ...
    private enum BossState { Idle, BasicAttacks, MainPattern, Aftershock }
    private BossState currentState = BossState.Idle;

    [Header("Component Links")]
    public Animator bodyAnimator;
    public BossArm leftArm;
    public BossArm rightArm;
    public Transform playerTransform;

    [Header("Timers")]
    public float mainPatternCycleTime = 30.0f;
    public float basicAttackCooldownMin = 3.0f;
    public float basicAttackCooldownMax = 5.0f;

    private float mainPatternTimer;
    private float basicAttackTimer;

    [Header("Pattern Prefabs (Basic)")]
    public GameObject warningSignPrefab;
    public int rockfallAmount = 10;

    [Tooltip("Warning sign for Arm Slam")]
    public GameObject slamWarningPrefab;
    [Tooltip("Hitbox for Arm Slam")]
    public GameObject slamHitboxPrefab;

    [Header("Main Pattern Settings")]
    public LaserBeam mainLaserBeam;

    public GameObject coreItemPrefab;
    // ▼▼▼▼▼ (수정) 1개로 변경 ▼▼▼▼▼
    public int coreDropAmount = 1;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    [Header("Attack Areas")]
    public Collider2D arenaBounds;

    [Header("Pattern Timings")]
    public float slamWarningDuration = 1.5f;
    public float armSlamAnimationTime = 1.5f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        // ▼▼▼▼▼ (수정) 타이머 초기화 코드 추가 (바로 발사 방지) ▼▼▼▼▼
        mainPatternTimer = mainPatternCycleTime;
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        ChangeState(BossState.BasicAttacks);
    }

    // ... (Update, ExecuteRandomBasicAttack, 기초 공격 코루틴들은 동일 - 생략 가능하지만 전체 코드 유지) ...

    void Update()
    {
        switch (currentState)
        {
            case BossState.BasicAttacks:
                mainPatternTimer -= Time.deltaTime;
                if (mainPatternTimer <= 0)
                {
                    ChangeState(BossState.MainPattern);
                    break;
                }

                basicAttackTimer -= Time.deltaTime;
                if (basicAttackTimer <= 0)
                {
                    ExecuteRandomBasicAttack();
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }

    void ExecuteRandomBasicAttack()
    {
        int patternIndex = Random.Range(0, 3);
        switch (patternIndex)
        {
            case 0: StartCoroutine(Attack_Basic_1_BothArms()); break;
            case 1: StartCoroutine(Attack_Basic_2_RightArm()); break;
            case 2: StartCoroutine(Attack_Basic_3_LeftArmSlam()); break;
        }
    }

    IEnumerator Attack_Basic_1_BothArms()
    {
        Debug.Log("Basic 1: Both Arms");
        yield return new WaitForSeconds(1.0f);
        ChangeState(BossState.Aftershock);
    }

    IEnumerator Attack_Basic_2_RightArm()
    {
        // (기존 코드 유지)
        Debug.Log("Basic 2: Right Arm Slam");
        Vector3 targetPos = playerTransform.position;
        if (slamWarningPrefab != null)
        {
            GameObject warningSign = Instantiate(slamWarningPrefab, targetPos, Quaternion.identity);
            DestroyAfterTime script = warningSign.GetComponent<DestroyAfterTime>();
            if (script != null) script.Initialize(slamWarningDuration);
            else Destroy(warningSign, slamWarningDuration);
        }
        yield return new WaitForSeconds(slamWarningDuration);
        if (rightArm != null)
        {
            rightArm.Slam(targetPos);
            if (slamHitboxPrefab != null) Instantiate(slamHitboxPrefab, targetPos, Quaternion.identity);
        }
        yield return new WaitForSeconds(armSlamAnimationTime);
        ChangeState(BossState.Aftershock);
    }

    IEnumerator Attack_Basic_3_LeftArmSlam()
    {
        Debug.Log("Basic 3: Left Arm 3-Combo");
        yield return new WaitForSeconds(1.0f);
        ChangeState(BossState.Aftershock);
    }

    // ▼▼▼▼▼ (수정) 레이저 코루틴 ▼▼▼▼▼
    IEnumerator Attack_Main_1_Laser()
    {
        Debug.Log("Main Pattern 1: Laser Start");

        if (mainLaserBeam != null)
        {
            mainLaserBeam.gameObject.SetActive(true);
            mainLaserBeam.FireLaser(); // 원뿔형 레이저 발사

            float totalDuration = mainLaserBeam.warningTime + mainLaserBeam.fireDuration;
            yield return new WaitForSeconds(totalDuration);
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
        }

        Debug.Log($"Dropping {coreDropAmount} Core Items!");
        SpawnCoreItems(coreDropAmount);

        yield return new WaitForSeconds(1.0f);
        ChangeState(BossState.Aftershock);
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    // (SpawnCoreItems, Pattern_Global_Aftershock, ChangeState 함수는 동일)
    void SpawnCoreItems(int count)
    {
        if (coreItemPrefab == null || arenaBounds == null) return;
        Bounds bounds = arenaBounds.bounds;
        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            Vector2 spawnPos = new Vector2(randomX, randomY);
            Instantiate(coreItemPrefab, spawnPos, Quaternion.identity);
        }
    }

    IEnumerator Pattern_Global_Aftershock()
    {
        Debug.Log("Global: Aftershock");
        yield return new WaitForSeconds(1.0f);
        if (warningSignPrefab != null && arenaBounds != null)
        {
            Bounds bounds = arenaBounds.bounds;
            for (int i = 0; i < rockfallAmount; i++)
            {
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomY = Random.Range(bounds.min.y, bounds.max.y);
                Instantiate(warningSignPrefab, new Vector2(randomX, randomY), Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(2.5f);
        ChangeState(BossState.BasicAttacks);
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case BossState.Idle: break;
            case BossState.BasicAttacks:
                basicAttackTimer = Random.Range(basicAttackCooldownMin, basicAttackCooldownMax);
                break;
            case BossState.MainPattern:
                mainPatternTimer = mainPatternCycleTime;
                StartCoroutine(Attack_Main_1_Laser());
                break;
            case BossState.Aftershock:
                StartCoroutine(Pattern_Global_Aftershock());
                break;
        }
    }
}