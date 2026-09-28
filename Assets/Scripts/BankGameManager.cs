using UnityEngine;

public class BankGameManager : MonoBehaviour
{
    static BankGameManager _instance;
    public static BankGameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<BankGameManager>(FindObjectsInactive.Include);
            return _instance;
        }
        private set { _instance = value; }
    }

    [Header("UI")]
    public MonitorController monitorController;

    [Header("Character Prefabs")]
    public GameObject[] malePrefabs;    // Ch01, Ch33
    public GameObject[] femalePrefabs;  // Ch22

    [Header("Waypoints")]
    public Transform doorSpawnPoint;
    public Transform counterPoint;
    public Transform exitPoint;

    [Header("Score / Rank HUD")]
    public ScoreHUD scoreHUD;
    public const int MaxComplaints = 5;

    [Header("응대 제한시간")]
    public float responseTimeLimit = 25f;   // 손님 1명당 응대 제한시간(초)
    float timeRemaining = 0f;
    bool  timerActive   = false;

    int score      = 0;   // 누적 점수
    int complaints = 0;   // 불만 접수 횟수
    int processed  = 0;   // 처리한 고객 수

    int currentIndex = 0;
    BankCustomer activeCustomer;

    public enum GameState { WaitingForCustomer, CustomerAtCounter, Processing, Done }
    public GameState State { get; private set; } = GameState.WaitingForCustomer;

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
    }

    void Start()
    {
        if (monitorController == null)
            monitorController = FindFirstObjectByType<MonitorController>(FindObjectsInactive.Include);

        if (monitorController == null)
        {
            Debug.LogError("[Bank] MonitorController를 찾을 수 없습니다.");
            return;
        }

        monitorController.HideMonitor();

        if (scoreHUD == null)
            scoreHUD = FindFirstObjectByType<ScoreHUD>(FindObjectsInactive.Include);
        RefreshHUD();

        Invoke(nameof(SendNextCustomer), 1.5f);
    }

    void Update()
    {
        if (!timerActive || IsGameOver) return;

        timeRemaining -= Time.deltaTime;
        if (scoreHUD != null) scoreHUD.SetTimer(timeRemaining, responseTimeLimit);

        if (timeRemaining <= 0f)
        {
            timerActive = false;
            OnTimeout();
        }
    }

    void StartTimer()
    {
        timeRemaining = responseTimeLimit;
        timerActive   = true;
        if (scoreHUD != null) scoreHUD.SetTimer(timeRemaining, responseTimeLimit);
    }

    void StopTimer()
    {
        timerActive = false;
        if (scoreHUD != null) scoreHUD.HideTimer();
    }

    // 제한시간 초과 → 고객이 화내고 떠남 + 불만 접수
    void OnTimeout()
    {
        if (State == GameState.Processing || State == GameState.Done) return;
        State = GameState.Processing;

        if (activeCustomer != null) activeCustomer.PlayAngry();
        ReportResult(false);   // 오응대 처리 (불만 +1, 점수 -5)

        string msg =
            "<color=#FF4444>시간 초과</color>\n\n" +
            "응대가 지연되어 고객이 불만을 제기하고\n돌아갔습니다.\n\n" +
            "신속한 응대가 필요합니다.";
        monitorController.ShowTimeout(msg);
    }

    // ── 점수 / 직급 / 불만 ────────────────────────────────────
    public void ReportResult(bool correct)
    {
        processed++;
        if (correct)
        {
            score += 10;
        }
        else
        {
            complaints++;
            score = Mathf.Max(0, score - 5);
        }
        RefreshHUD();

        if (complaints >= MaxComplaints)
            GameOver();
    }

    public string GetRank()
    {
        if (score >= 330) return "지점장";
        if (score >= 250) return "부장";
        if (score >= 180) return "차장";
        if (score >= 120) return "과장";
        if (score >= 70)  return "대리";
        if (score >= 30)  return "주임";
        if (score >= 10)  return "사원";
        return "수습";
    }

    void RefreshHUD()
    {
        if (scoreHUD != null)
        {
            scoreHUD.Refresh(GetRank(), score, complaints, MaxComplaints);
            int waiting = Mathf.Max(0, CustomerData.SampleQueue.Length - currentIndex);
            scoreHUD.SetQueue(waiting);
        }
    }

    public bool IsGameOver => State == GameState.Done && complaints >= MaxComplaints;

    void ShowDayComplete()
    {
        int total = CustomerData.SampleQueue.Length;
        int correct = total - complaints;
        float rate = total > 0 ? (correct * 100f / total) : 0f;
        string msg =
            $"오늘 업무 종료\n\n" +
            $"처리 고객: {processed}명\n정확 응대: {correct} / {total} ({rate:F0}%)\n" +
            $"불만 접수: {complaints}회\n\n" +
            $"최종 점수: {score}점\n최종 직급: {GetRank()}\n\n" +
            "수고하셨습니다!";
        if (scoreHUD != null) scoreHUD.ShowDayComplete(msg);
    }

    void GameOver()
    {
        State = GameState.Done;
        string msg =
            $"권고사직\n\n불만 접수 {complaints}회 누적\n\n" +
            $"최종 직급: {GetRank()}\n처리 고객: {processed}명\n최종 점수: {score}점\n\n" +
            "고객 응대 미숙으로 권고사직 처리되었습니다.";
        if (scoreHUD != null) scoreHUD.ShowGameOver(msg);
        Debug.Log("[Bank] 게임 오버 — 권고사직");
    }

    public void CallNextCustomer()
    {
        if (IsGameOver) return;   // 권고사직 시 진행 중단
        monitorController.ShowNextCall(() => SendNextCustomer());
    }

    public void SendNextCustomer()
    {
        if (currentIndex >= CustomerData.SampleQueue.Length)
        {
            Debug.Log("[Bank] 오늘 업무 종료!");
            State = GameState.Done;
            ShowDayComplete();
            return;
        }

        State = GameState.WaitingForCustomer;
        var data = CustomerData.SampleQueue[currentIndex];

        // 성별에 맞는 프리팹 랜덤 선택
        var pool = data.gender == Gender.Female ? femalePrefabs : malePrefabs;
        if (pool == null || pool.Length == 0)
        {
            Debug.LogError($"[Bank] {data.gender} 프리팹이 없습니다!");
            return;
        }

        var prefab = pool[Random.Range(0, pool.Length)];

        // Instantiate로 캐릭터 스폰
        var go = Instantiate(prefab);
        activeCustomer = go.GetComponent<BankCustomer>();

        if (activeCustomer == null)
        {
            Debug.LogError("[Bank] BankCustomer 컴포넌트가 없습니다!");
            return;
        }

        // 웨이포인트 할당
        activeCustomer.doorSpawnPoint = doorSpawnPoint;
        activeCustomer.counterPoint   = counterPoint;
        activeCustomer.exitPoint      = exitPoint;

        activeCustomer.WalkToCounter(data);
    }

    public void OnCustomerArrived(CustomerData data)
    {
        State = GameState.CustomerAtCounter;
        monitorController.ShowCustomerInfo(data);
        StartTimer();   // 손님 응대 제한시간 시작
    }

    public void OnTellerAction(string action)
    {
        if (State != GameState.CustomerAtCounter) return;
        StopTimer();
        State = GameState.Processing;

        var data = CustomerData.SampleQueue[currentIndex];

        // 거절 시 화난 모션
        if (action == "거절" && activeCustomer != null)
            activeCustomer.PlayAngry();

        monitorController.ShowResult(action, data);
    }

    public void OnProcessComplete()
    {
        currentIndex++;
        monitorController.HideMonitor();

        if (activeCustomer != null)
            activeCustomer.WalkOut(CallNextCustomer);
        else
            CallNextCustomer();
    }

    // 본인확인 거절 → Angry 1회 재생 후 퇴장 → 다음 손님
    public void OnVerifyFailed()
    {
        StopTimer();
        State = GameState.Processing;

        // 문제 고객을 본인확인 단계에서 돌려보낸 것은 올바른 대응(정상 고객이면 오응대=불만)
        if (currentIndex < CustomerData.SampleQueue.Length)
        {
            var d = CustomerData.SampleQueue[currentIndex];
            ReportResult(!d.isNormalCase);
        }

        currentIndex++;
        monitorController.HideMonitor();

        if (activeCustomer != null)
        {
            activeCustomer.PlayAngry();
            // Angry 모션이 어느 정도 재생되면 퇴장 (1.5초 후)
            StartCoroutine(WalkOutAfterAngry());
        }
        else
        {
            SendNextCustomer();
        }
    }

    System.Collections.IEnumerator WalkOutAfterAngry()
    {
        yield return new WaitForSeconds(1.5f);
        if (activeCustomer != null)
        {
            activeCustomer.StopAngry();
            activeCustomer.WalkOut(CallNextCustomer);
        }
        else
        {
            CallNextCustomer();
        }
    }
}
