using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List 사용

public class BossController : MonoBehaviour
{
    private enum BossState { Idle, BasicAttacks, MainPattern, Aftershock }
    private BossState currentState = BossState.Idle;

    [Header("Component Links")]
    public Animator bodyAnimator;
    public BossArm leftArm; // (2단계에서 만들 스크립트)
    public BossArm rightArm; // (2단계에서 만들 스크립트)
    public Transform playerTransform; // 플레이어 위치 (추격용)

    [Header("Timers")]
    [Tooltip("메인 패턴(레이저)이 발동되기까지의 시간")]
    public float mainPatternCycleTime = 30.0f;
    [Tooltip("기초 공격 사이의 최소/최대 대기 시간")]
    public float basicAttackCooldownMin = 3.0f;
    public float basicAttackCooldownMax = 5.0f;

    private float mainPatternTimer;
    private float basicAttackTimer;

    [Header("Pattern Prefabs")] // (새 헤더 또는 기존 헤더에 추가)
    [Tooltip("낙석 패턴(Aftershock)이 사용할 워닝사인 프리팹")]
    public GameObject warningSignPrefab; // (WarningSign_Prefab 연결)
    public int rockfallAmount = 10; // 한 번에 떨어질 낙석 개수

    [Tooltip("기초공격 1, 2, 3번에 사용할 내려찍기 경고 프리팹")]
    public GameObject slamWarningPrefab;

    [Header("Attack Areas")] // (추가) 공격 범위를 정의
    public Collider2D arenaBounds; // (예: 씬에 배치된 BoxCollider2D)

    [Header("Pattern Timings")]
    [Tooltip("내려찍기 경고 표시 시간 (기초공격 1, 2, 3)")]
    public float slamWarningDuration = 1.5f;
    [Tooltip("BossArm.cs의 Slam() 애니메이션 총 시간")]
    public float armSlamAnimationTime = 1.5f; // (BossArm.cs의 0.2+0.3+0.5+0.5 = 1.5초)

    void Start()
    {
        // 플레이어 태그로 찾기 (안전장치)
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // 보스전 시작
        ChangeState(BossState.BasicAttacks);
    }

    void Update()
    {
        // 현재 상태에 따라 타이머 관리
        switch (currentState)
        {
            case BossState.BasicAttacks:
                // 메인 패턴 타이머
                mainPatternTimer -= Time.deltaTime;
                if (mainPatternTimer <= 0)
                {
                    ChangeState(BossState.MainPattern);
                    break;
                }

                // 기초 공격 타이머
                basicAttackTimer -= Time.deltaTime;
                if (basicAttackTimer <= 0)
                {
                    // 기초 공격 실행
                    ExecuteRandomBasicAttack();
                    // 이 상태를 잠시 'Idle'로 바꿔서 중복 실행 방지
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Idle:
            case BossState.MainPattern:
            case BossState.Aftershock:
                // 코루틴이 스스로 상태를 변경하므로 Update에서 할 일 없음
                break;
        }
    }

    void ExecuteRandomBasicAttack()
    {
        // 1, 2, 3번 중 랜덤 선택
        int patternIndex = Random.Range(0, 3);
        switch (patternIndex)
        {
            case 0:
                StartCoroutine(Attack_Basic_1_BothArms());
                break;
            case 1:
                StartCoroutine(Attack_Basic_2_RightArm());
                break;
            case 2:
                StartCoroutine(Attack_Basic_3_LeftArmSlam());
                break;
        }
    }

    // --- 공격 패턴 코루틴 (뼈대) ---

    // 기초공격 1 (양팔)
    IEnumerator Attack_Basic_1_BothArms()
    {
        Debug.Log("기초 1: 양팔 공격 시작");
        // (TODO: 1. 3분의 2 범위 경고 표시)
        // (TODO: 2. N초 대기)
        // (TODO: 3. leftArm.Slam() / rightArm.Slam() 호출)

        yield return new WaitForSeconds(1.0f); // (임시 대기)
        ChangeState(BossState.Aftershock); // (중요) 끝나면 낙석 패턴으로
    }

    // 기초공격 2 (오른팔 추격)
    IEnumerator Attack_Basic_2_RightArm()
    {
        Debug.Log("기초 2: 오른팔 내려찍기 시작");

        // 1. 플레이어의 현재 위치 저장
        Vector3 targetPos = playerTransform.position;

        // 2. '내려찍기 경고' 프리팹 스폰
        if (slamWarningPrefab != null)
        {
            GameObject warningSign = Instantiate(slamWarningPrefab, targetPos, Quaternion.identity);

            // 3. 경고 프리팹에게 파괴 시간 설정
            DestroyAfterTime script = warningSign.GetComponent<DestroyAfterTime>();
            if (script != null)
            {
                script.Initialize(slamWarningDuration);
            }
            else
            {
                // 스크립트가 없을 경우를 대비한 안전장치
                Destroy(warningSign, slamWarningDuration);
            }
        }

        // 4. 경고 시간(1.5초)만큼 대기
        yield return new WaitForSeconds(slamWarningDuration);

        // 5. 오른팔 스크립트에게 'Slam' 명령
        if (rightArm != null)
        {
            rightArm.Slam(targetPos);
            // (TODO: 이 시점에 targetPos에 공격 판정(Hitbox) 스폰)
        }

        // 6. 팔 애니메이션이 끝날 때까지(1.5초) 대기
        yield return new WaitForSeconds(armSlamAnimationTime);

        // 7. (중요) 끝나면 낙석 패턴으로
        ChangeState(BossState.Aftershock);
    }

    // 기초공격 3 (왼팔 3연속 추격)
    IEnumerator Attack_Basic_3_LeftArmSlam()
    {
        Debug.Log("기초 3: 왼팔 3연타 시작");
        // (TODO: 1. leftArm.Raise() (팔 들어올리기))
        // (TODO: 2. yield return new WaitForSeconds(1.0f);)
        // (TODO: 3. for (int i = 0; i < 3; i++))

        yield return new WaitForSeconds(1.0f); // (임시 대기)
        ChangeState(BossState.Aftershock);
    }

    // 주요패턴 1 (레이저)
    IEnumerator Attack_Main_1_Laser()
    {
        Debug.Log("메인 패턴 1: 레이저 시작");
        // (TODO: 1. bodyAnimator.SetTrigger("ChargeLaser");)
        // (TODO: 2. 안전 지대(좌우 상단)를 제외한 전체 범위 경고 표시)
        // (TODO: 3. 3초간 레이저 공격 판정)

        Debug.Log("코어 아이템 5개 드랍!");
        // (TODO: SpawnCoreItems(5);)

        yield return new WaitForSeconds(1.0f); // (임시 대기)
        ChangeState(BossState.Aftershock);
    }

    IEnumerator Pattern_Global_Aftershock()
    {
        Debug.Log("글로벌: 낙석 패턴 시작");

        // 1. 1초간 방 흔들기 (Camera Shake)
        // (TODO: CameraShake.Instance.Shake(1.0f);)
        yield return new WaitForSeconds(1.0f);

        // 2. 아레나 범위 내에 랜덤한 위치 N개 찾기
        if (warningSignPrefab != null && arenaBounds != null)
        {
            Bounds bounds = arenaBounds.bounds;
            for (int i = 0; i < rockfallAmount; i++)
            {
                // 아레나 바운더리 내 랜덤 좌표 생성
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomY = Random.Range(bounds.min.y, bounds.max.y);
                Vector2 spawnPos = new Vector2(randomX, randomY);

                // 3. '워닝사인 프리팹' 스폰
                Instantiate(warningSignPrefab, spawnPos, Quaternion.identity);
            }
        }

        // 4. (중요) 워닝사인 지속시간(2초) + 추가 대기시간(0.5초) 만큼 기다림
        // (WarningSignAndRockfall.cs의 warningDuration보다 길어야 함)
        yield return new WaitForSeconds(2.5f);

        // 5. 다음 행동(기초 공격)으로 복귀
        ChangeState(BossState.BasicAttacks);
    }


    // --- 상태 변경 관리 ---
    void ChangeState(BossState newState)
    {
        currentState = newState;

        // 새 상태에 진입할 때 초기화
        switch (newState)
        {
            case BossState.Idle:
                // 아무것도 안 함 (코루틴이 끝났다는 의미)
                break;
            case BossState.BasicAttacks:
                // 기초 공격 타이머 리셋
                basicAttackTimer = Random.Range(basicAttackCooldownMin, basicAttackCooldownMax);
                break;
            case BossState.MainPattern:
                // 메인 패턴 타이머 리셋
                mainPatternTimer = mainPatternCycleTime;
                StartCoroutine(Attack_Main_1_Laser());
                break;
            case BossState.Aftershock:
                StartCoroutine(Pattern_Global_Aftershock());
                break;
        }
    }
}