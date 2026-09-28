using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorController : MonoBehaviour
{
    // ── 패널 ──────────────────────────────────────────────────
    [Header("Panels")]
    public GameObject monitorCanvas;
    public GameObject verificationPanel;   // 본인확인 (VN 스타일)
    public GameObject infoPanel;           // 고객 정보
    public GameObject actionPanel;         // 처리완료 / 거절
    public GameObject resultPanel;         // 처리 결과

    // ── 본인확인 패널 ─────────────────────────────────────────
    [Header("Verification Panel")]
    public TMP_Text txtDialogue;           // 대화 문구
    public Button   btnChoice1;
    public Button   btnChoice2;
    public Button   btnChoice3;
    public TMP_Text txtChoice1;
    public TMP_Text txtChoice2;
    public TMP_Text txtChoice3;

    // ── 고객 정보 패널 ────────────────────────────────────────
    [Header("Info Panel")]
    public TMP_Text txtName;
    public TMP_Text txtAccount;
    public TMP_Text txtRequest;
    public TMP_Text txtAmount;
    public TMP_Text txtMemo;
    public TMP_Text txtCredit;     // 신용점수 / DSR / 총부채
    public TMP_Text txtRisk;       // FDS / 계좌제한 / 의심 여부
    public TMP_Text txtOverdue;    // 연체 이력
    public TMP_Text txtCdd;        // CDD 상태

    // ── 결과 패널 ─────────────────────────────────────────────
    [Header("Result Panel")]
    public TMP_Text txtResult;
    public Button   btnConfirm;    // 확인 버튼 (피드백 후 진행)

    System.Action pendingConfirm;

    // ── 액션 버튼 ─────────────────────────────────────────────
    [Header("Action Buttons")]
    public Button btnProcess;      // 처리완료
    public Button btnReject;       // 거절

    // ── 내부 상태 ─────────────────────────────────────────────
    CustomerData currentData;
    bool initialized = false;

    // 본인확인 단계 정의
    struct VerifyStep
    {
        public string question;
        public string choice1, choice2, choice3;
        // 0=선택없음, 1/2/3 중 올바른 응답 번호 (0이면 분기 없음)
        public int correctChoice;
        // 다음 단계 인덱스 (-1=성공완료, -2=실패)
        public int next1, next2, next3;
    }

    List<VerifyStep> steps = new List<VerifyStep>();
    int currentStep = 0;

    // ─────────────────────────────────────────────────────────
    void Awake() { Init(); }
    void Start()  { Init(); }

    void Init()
    {
        if (initialized) return;
        initialized = true;

        if (monitorCanvas == null) monitorCanvas = gameObject;
        TryFindReferences();
        BindButtons();
    }

    void BindButtons()
    {
        // 본인확인 선택지
        btnChoice1?.onClick.RemoveAllListeners();
        btnChoice2?.onClick.RemoveAllListeners();
        btnChoice3?.onClick.RemoveAllListeners();
        btnChoice1?.onClick.AddListener(() => OnChoice(1));
        btnChoice2?.onClick.AddListener(() => OnChoice(2));
        btnChoice3?.onClick.AddListener(() => OnChoice(3));

        // 처리 버튼 (상담요청 없음)
        btnProcess?.onClick.RemoveAllListeners();
        btnReject ?.onClick.RemoveAllListeners();
        btnProcess?.onClick.AddListener(() => OnAction("처리완료"));
        btnReject ?.onClick.AddListener(() => OnAction("거절"));

        // 확인 버튼 (피드백 화면 → 다음 진행)
        btnConfirm?.onClick.RemoveAllListeners();
        btnConfirm?.onClick.AddListener(OnConfirm);
    }

    void OnConfirm()
    {
        var cb = pendingConfirm;
        pendingConfirm = null;
        if (btnConfirm != null) btnConfirm.gameObject.SetActive(false);
        cb?.Invoke();
    }

    // ── 본인확인 단계 빌드 ────────────────────────────────────
    // 손님이 말로 하는 선택지는 판단에 영향을 주지 않으므로 제거.
    // 간단한 인사 + 연락처 확인 1회만 거친 뒤, 화면의 고객 정보로 판단한다.
    void BuildVerifySteps(CustomerData d)
    {
        steps.Clear();

        steps.Add(new VerifyStep
        {
            question      = $"안녕하세요, {d.customerName} 고객님.\n등록된 연락처가\n{d.phone}\n맞으신지 확인 부탁드립니다.",
            choice1       = "네, 맞습니다. (정보 확인)",
            choice2       = "",   // 미사용 (버튼 숨김)
            choice3       = "",   // 미사용 (버튼 숨김)
            correctChoice = 1,
            next1         = -1,   // 확인 → 고객 정보 화면
            next2         = -1,
            next3         = -1,
        });
    }

    // ── 선택지 클릭 처리 ──────────────────────────────────────
    void OnChoice(int choice)
    {
        if (currentStep >= steps.Count) return;
        var step = steps[currentStep];

        int next = choice switch { 1 => step.next1, 2 => step.next2, _ => step.next3 };

        if (next == -1)
        {
            // 본인확인 성공
            ShowInfoPanel();
        }
        else if (next == -2)
        {
            ShowVerifyFail("죄송하지만 본인 확인이 불가하면\n상담이 불가능합니다.\n신분증을 지참하신 후 다시 방문해 주세요.");
        }
        else if (next == -3)
        {
            // 보이스피싱 의심 → 지급유예
            ShowVerifyFail("이상 거래가 감지되었습니다.\n금융사기 예방을 위해 지급이 유예됩니다.\n담당자가 추가 안내드리겠습니다.");
        }
        else if (next == -4)
        {
            // 대포통장 → 계좌 개설 거절
            ShowVerifyFail("최근 단기간 내 다수 계좌 개설이 확인되어\n대포통장 방지법에 따라 거래가 제한됩니다.");
        }
        else
        {
            // 다음 단계로
            // step 10 / 20 / 30 → steps 리스트 실제 인덱스로 매핑
            int realIndex = next switch
            {
                10 => 2,
                20 => 3,
                30 => currentData.suspectedPhishing ? 3 : 4,
                _  => next
            };
            if (realIndex < steps.Count)
            {
                currentStep = realIndex;
                ShowStep(currentStep);
            }
            else
            {
                ShowInfoPanel();
            }
        }
    }

    void ShowStep(int index)
    {
        if (index >= steps.Count) { ShowInfoPanel(); return; }
        var s = steps[index];

        if (txtDialogue != null) txtDialogue.text = s.question;
        if (txtChoice1  != null) txtChoice1.text  = s.choice1;
        if (txtChoice2  != null) txtChoice2.text  = s.choice2;
        if (txtChoice3  != null) txtChoice3.text  = s.choice3;

        // 선택지 텍스트가 비어있으면 해당 버튼 숨김 (단일 선택지 지원)
        if (btnChoice1 != null) btnChoice1.gameObject.SetActive(!string.IsNullOrEmpty(s.choice1));
        if (btnChoice2 != null) btnChoice2.gameObject.SetActive(!string.IsNullOrEmpty(s.choice2));
        if (btnChoice3 != null) btnChoice3.gameObject.SetActive(!string.IsNullOrEmpty(s.choice3));
    }

    void ShowVerifyFail(string msg)
    {
        // 본인확인 단계 거절 → 자세한 설명 + 확인 버튼
        string full = $"<color=#FF4444>본인 확인 실패</color>\n\n{msg}";
        ShowFeedback(full, () => BankGameManager.Instance.OnVerifyFailed());
    }

    // ── 피드백 화면 (설명 + 확인 버튼으로만 진행) ─────────────
    void ShowFeedback(string msg, System.Action onConfirm)
    {
        SetPanel(verification: false, info: false, action: false, result: true);
        if (txtResult != null)
        {
            txtResult.text = msg;
            txtResult.alignment = TextAlignmentOptions.Center;
        }
        pendingConfirm = onConfirm;
        if (btnConfirm != null) btnConfirm.gameObject.SetActive(true);
    }

    // ── 패널 전환 ─────────────────────────────────────────────
    void SetPanel(bool verification, bool info, bool action, bool result)
    {
        if (verificationPanel != null) verificationPanel.SetActive(verification);
        if (infoPanel         != null) infoPanel.SetActive(info);
        if (actionPanel       != null) actionPanel.SetActive(action);
        if (resultPanel       != null) resultPanel.SetActive(result);
    }

    void ShowInfoPanel()
    {
        SetPanel(verification: false, info: true, action: true, result: false);
        FillInfo(currentData);
    }

    void FillInfo(CustomerData d)
    {
        if (txtName    != null) txtName.text    = $"성명: {d.customerName}";
        if (txtAccount != null) txtAccount.text = $"계좌: {d.accountNumber}  |  잔액: {d.accountBalance:N0}원";
        if (txtRequest != null) txtRequest.text = $"업무: {d.requestType}";
        if (txtAmount  != null)
        {
            if (d.amount > 0)
            {
                // 출금·이체는 잔액과 비교하여 부족 시 경고 표시
                bool needsBalance = d.requestType == RequestType.출금 || d.requestType == RequestType.이체;
                bool insufficient = needsBalance && d.amount > d.accountBalance;
                string warn = insufficient ? "  <color=#FF4444>[잔액 부족]</color>" : "";
                txtAmount.text = $"금액: {d.amount:N0}원{warn}";
            }
            else
            {
                txtAmount.text = "금액: 해당없음";
            }
        }
        if (txtMemo    != null) txtMemo.text    = $"비고: {d.memo}";

        // 신용 정보
        if (txtCredit != null)
            txtCredit.text = $"신용점수: {d.creditScore}점  |  DSR: {d.dsr:F1}%  |  부채: {d.totalDebt:N0}만원";

        // FDS / 계좌 제한
        if (txtRisk != null)
        {
            string riskColor = d.fdsRisk switch
            {
                FdsRiskLevel.정상 => "#00FF88",
                FdsRiskLevel.주의 => "#FFD700",
                FdsRiskLevel.위험 => "#FF4444",
                FdsRiskLevel.차단 => "#FF0000",
                _ => "#FFFFFF"
            };
            string extra = "";
            if (d.restriction != AccountRestriction.없음)
                extra += $"  <color=#FF4444>[{d.restriction}]</color>";
            if (d.suspectedPhishing)
                extra += "  <color=#FF4444>[보이스피싱 의심]</color>";
            if (d.suspectedBurnerAccount)
                extra += "  <color=#FF4444>[대포통장 의심]</color>";
            txtRisk.text = $"FDS: <color={riskColor}>{d.fdsRisk}</color>{extra}";
        }

        // 연체
        if (txtOverdue != null)
        {
            string od = d.hasOverdue
                ? $"<color=#FF4444>현재 연체 중 ({d.overdueMonths}개월)</color>"
                : (d.overdueMonths > 0 ? $"과거 연체 {d.overdueMonths}개월 (완납)" : "없음");
            txtOverdue.text = $"연체: {od}";
        }

        // CDD
        if (txtCdd != null)
        {
            string cddColor = d.cddStatus == CddStatus.완료 ? "#00FF88" : "#FF4444";
            txtCdd.text = $"고객확인(CDD): <color={cddColor}>{d.cddStatus}</color>";
        }
    }

    // ── 외부 진입점 ───────────────────────────────────────────
    public void ShowCustomerInfo(CustomerData data)
    {
        currentData  = data;
        currentStep  = 0;

        monitorCanvas.SetActive(true);

        // 패널 활성화 후 리스너 재등록 (비활성 상태에서 Awake가 안 탔을 수 있으므로)
        TryFindReferences();
        BindButtons();
        ApplyButtonTextAutoSize();

        BuildVerifySteps(data);
        SetPanel(verification: true, info: false, action: false, result: false);
        ShowStep(0);

        Debug.Log($"[Monitor] 본인확인 시작: {data.customerName}");
    }

    // 버튼 텍스트 자동 크기 조절 — 넘치지 않도록
    void ApplyButtonTextAutoSize()
    {
        foreach (var txt in new[] { txtChoice1, txtChoice2, txtChoice3 })
        {
            if (txt == null) continue;
            txt.enableAutoSizing  = true;
            txt.fontSizeMin       = 8f;
            txt.fontSizeMax       = 15f;
            txt.overflowMode      = TextOverflowModes.Ellipsis;
            txt.textWrappingMode = TextWrappingModes.Normal;
        }
    }

    public void ShowResult(string action, CustomerData data)
    {
        // 정답 판정: 정상 고객은 처리완료, 문제 고객은 거절이 정답
        string correctAction = data.isNormalCase ? "처리완료" : "거절";
        bool   isCorrect     = (action == correctAction);

        // 점수/직급/불만 반영
        BankGameManager.Instance.ReportResult(isCorrect);

        string verdict = isCorrect
            ? "<color=#00FF88>✔ 올바른 판단입니다</color>"
            : "<color=#FF4444>✘ 잘못된 판단입니다</color>";

        string head = $"{data.customerName} 고객 · {data.requestType}";

        string detail;
        if (data.isNormalCase)
        {
            // 정상 고객
            detail = isCorrect
                ? "문제가 없는 정상 거래였습니다.\n정확하게 처리하셨습니다."
                : "이 고객은 아무 문제가 없는 정상 거래였습니다.\n부당하게 거절하면 고객 민원이 발생합니다.\n→ 처리완료가 올바른 선택이었습니다.";
        }
        else
        {
            // 거절해야 하는 고객
            string reason = string.IsNullOrEmpty(data.rejectReason) ? "규정 위반" : data.rejectReason;
            detail = isCorrect
                ? $"거절 사유: {reason}\n규정에 따라 올바르게 거절했습니다."
                : $"이 거래는 승인하면 안 되는 건이었습니다.\n사유: {reason}\n→ 거절이 올바른 선택이었습니다.";
        }

        string msg = $"{verdict}\n\n{head}\n\n{detail}";

        ShowFeedback(msg, () => BankGameManager.Instance.OnProcessComplete());
    }

    // 제한시간 초과 → 설명 + 확인 버튼으로 다음 진행
    public void ShowTimeout(string msg)
    {
        monitorCanvas.SetActive(true);
        TryFindReferences();
        BindButtons();
        ShowFeedback(msg, () => BankGameManager.Instance.OnProcessComplete());
    }

    public void HideMonitor()
    {
        if (monitorCanvas != null) monitorCanvas.SetActive(false);
    }

    public void ShowNextCall(System.Action onDone)
    {
        monitorCanvas.SetActive(true);
        SetPanel(verification: false, info: false, action: false, result: true);
        if (txtResult != null) txtResult.text = "다음 분이요!";
        StartCoroutine(NextCallSequence(onDone));
    }

    IEnumerator NextCallSequence(System.Action onDone)
    {
        yield return new WaitForSeconds(1.2f);
        HideMonitor();
        onDone?.Invoke();
    }

    void OnAction(string action)
    {
        Debug.Log($"[Monitor] OnAction: {action}");
        BankGameManager.Instance.OnTellerAction(action);
    }

    // ── 참조 자동 탐색 ────────────────────────────────────────
    void TryFindReferences()
    {
        if (verificationPanel == null) verificationPanel = FindChild("VerificationPanel");
        if (infoPanel         == null) infoPanel         = FindChild("InfoPanel");
        if (actionPanel       == null) actionPanel       = FindChild("ActionPanel");
        if (resultPanel       == null) resultPanel       = FindChild("ResultPanel");

        if (verificationPanel != null)
        {
            if (txtDialogue == null) txtDialogue = verificationPanel.transform.Find("TxtDialogue")?.GetComponent<TMP_Text>();
            if (btnChoice1  == null) btnChoice1  = verificationPanel.transform.Find("BtnChoice1")?.GetComponent<Button>();
            if (btnChoice2  == null) btnChoice2  = verificationPanel.transform.Find("BtnChoice2")?.GetComponent<Button>();
            if (btnChoice3  == null) btnChoice3  = verificationPanel.transform.Find("BtnChoice3")?.GetComponent<Button>();
            if (txtChoice1  == null) txtChoice1  = btnChoice1?.GetComponentInChildren<TMP_Text>();
            if (txtChoice2  == null) txtChoice2  = btnChoice2?.GetComponentInChildren<TMP_Text>();
            if (txtChoice3  == null) txtChoice3  = btnChoice3?.GetComponentInChildren<TMP_Text>();
        }

        if (infoPanel != null)
        {
            if (txtName    == null) txtName    = infoPanel.transform.Find("TxtName")   ?.GetComponent<TMP_Text>();
            if (txtAccount == null) txtAccount = infoPanel.transform.Find("TxtAccount")?.GetComponent<TMP_Text>();
            if (txtRequest == null) txtRequest = infoPanel.transform.Find("TxtRequest")?.GetComponent<TMP_Text>();
            if (txtAmount  == null) txtAmount  = infoPanel.transform.Find("TxtAmount") ?.GetComponent<TMP_Text>();
            if (txtMemo    == null) txtMemo    = infoPanel.transform.Find("TxtMemo")   ?.GetComponent<TMP_Text>();
            if (txtCredit  == null) txtCredit  = infoPanel.transform.Find("TxtCredit") ?.GetComponent<TMP_Text>();
            if (txtRisk    == null) txtRisk    = infoPanel.transform.Find("TxtRisk")   ?.GetComponent<TMP_Text>();
            if (txtOverdue == null) txtOverdue = infoPanel.transform.Find("TxtOverdue")?.GetComponent<TMP_Text>();
            if (txtCdd     == null) txtCdd     = infoPanel.transform.Find("TxtCdd")    ?.GetComponent<TMP_Text>();
        }

        if (resultPanel != null)
        {
            if (txtResult  == null) txtResult  = resultPanel.transform.Find("TxtResult")?.GetComponent<TMP_Text>();
            if (btnConfirm == null) btnConfirm = resultPanel.transform.Find("BtnConfirm")?.GetComponent<Button>();
        }

        if (actionPanel != null)
        {
            if (btnProcess == null) btnProcess = actionPanel.transform.Find("BtnProcess")?.GetComponent<Button>();
            if (btnReject  == null) btnReject  = actionPanel.transform.Find("BtnReject") ?.GetComponent<Button>();
        }
    }

    GameObject FindChild(string name)
    {
        var t = transform.Find(name);
        if (t != null) return t.gameObject;
        foreach (Transform child in transform)
        {
            var deep = child.Find(name);
            if (deep != null) return deep.gameObject;
        }
        return null;
    }
}
