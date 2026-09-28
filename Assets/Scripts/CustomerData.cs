using UnityEngine;

public enum Gender { Male, Female }

// 업무 유형
public enum RequestType
{
    출금,
    입금,
    이체,
    신규계좌개설,
    대출상담,
    카드발급,
    분실신고,
    적금가입,
    외화환전,
    공과금납부,
    증명서발급,
    정기예금,
    펀드가입,
    해외송금,
    통장재발급,
    대출상환,
    상속처리,
    대리인거래,
    보험가입,
    연금수령,
    자동이체,
    수표발행,
    카드한도상향,
    계좌해지,
    예금담보대출,
    비밀번호변경
}

// FDS 위험 등급
public enum FdsRiskLevel { 정상, 주의, 위험, 차단 }

// 계좌 제한 상태
public enum AccountRestriction { 없음, 압류, 가압류, 질권설정, 사고신고, 한도제한 }

// 고객확인(CDD) 상태
public enum CddStatus { 완료, 재인증필요, 거부, 미완료 }

[System.Serializable]
public class CustomerData
{
    // ── 기본 인적 사항 ─────────────────────────────────────────
    public string  customerName;
    public string  accountNumber;
    public Gender  gender;
    public string  phone;           // 연락처

    // 직장 정보
    public string  jobType;         // 직업군 (정규직/자영업/무직 등)
    public string  company;         // 직장명
    public int     workYears;       // 근속연수
    public int     annualIncome;    // 연 소득 (만원)

    // ── 요청 업무 ──────────────────────────────────────────────
    public RequestType requestType;
    public int         amount;      // 금액 (원)
    public string      memo;        // 비고 / 방문 목적

    // ── 고객확인(CDD/EDD) ──────────────────────────────────────
    public CddStatus   cddStatus;           // 고객확인 상태
    public bool        fundsSourceVerified; // 자금 원천 확인 여부
    public bool        beneficialOwnerConfirmed; // 실제 소유자 확인

    // ── 신용 및 부채 ───────────────────────────────────────────
    public int    creditScore;      // KCB/NICE 신용점수 (0~1000)
    public int    totalDebt;        // 총 대출 잔액 (만원)
    public float  dsr;              // DSR 총부채원리금상환비율 (%, 40% 초과 시 불가)
    public bool   hasOverdue;       // 현재 연체 중 여부
    public int    overdueMonths;    // 과거 최장 연체 개월수
    public string overdueDetail;    // 연체 상세 내용

    // ── 계좌 상태 ──────────────────────────────────────────────
    public long               accountBalance;     // 현재 계좌 잔액 (원)
    public int                accountAgeDays;     // 계좌 개설 후 경과일
    public bool               recentAccountOpened; // 최근 20영업일 내 타행 계좌 개설 여부
    public AccountRestriction restriction;         // 계좌 제한 사유

    // ── 리스크 / 보안 ──────────────────────────────────────────
    public FdsRiskLevel fdsRisk;          // FDS 위험 등급
    public bool         suspectedPhishing; // 보이스피싱 피해 의심
    public bool         suspectedBurnerAccount; // 대포통장 의심

    // ── 처리 결과 힌트 (게임 로직용) ─────────────────────────
    /// <summary>정상 처리 가능한 케이스이면 true</summary>
    public bool isNormalCase;
    /// <summary>거절 사유 코드 (MonitorController 표시용)</summary>
    public string rejectReason;

    // ─────────────────────────────────────────────────────────
    //  샘플 고객 큐 (10명)
    // ─────────────────────────────────────────────────────────
    public static CustomerData[] SampleQueue = new CustomerData[]
    {
        // ① 정상 출금 — 신분증 지참, 문제 없음
        new CustomerData
        {
            customerName   = "김민준",
            accountNumber  = "110-2847-3910",
            gender         = Gender.Male,
            phone          = "010-3821-4490",
            jobType        = "정규직",
            company        = "삼성전자(주)",
            workYears      = 6,
            annualIncome   = 5200,

            requestType    = RequestType.출금,
            amount         = 500000,
            memo           = "본인 신분증 지참, 생활비 출금",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 842,
            totalDebt      = 3500,
            dsr            = 18.4f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 12400000,
            accountAgeDays       = 1240,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.정상,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = true,
            rejectReason  = ""
        },

        // ② 정상 이체 — 월세 납부
        new CustomerData
        {
            customerName   = "이서연",
            accountNumber  = "220-1093-5672",
            gender         = Gender.Female,
            phone          = "010-5540-2231",
            jobType        = "정규직",
            company        = "카카오뱅크",
            workYears      = 3,
            annualIncome   = 4100,

            requestType    = RequestType.이체,
            amount         = 1200000,
            memo           = "월세 납부 (임대인 박○○ 계좌)",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 778,
            totalDebt      = 1800,
            dsr            = 11.2f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 8750000,
            accountAgeDays       = 890,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.정상,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = true,
            rejectReason  = ""
        },

        // ③ 거절 — DSR 초과 신용대출 신청
        new CustomerData
        {
            customerName   = "박지호",
            accountNumber  = "330-7761-2048",
            gender         = Gender.Male,
            phone          = "010-9921-6670",
            jobType        = "자영업",
            company        = "박지호 요식업(개인)",
            workYears      = 2,
            annualIncome   = 2800,

            requestType    = RequestType.대출상담,
            amount         = 50000000,
            memo           = "신용대출 5천만 원 신청",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 681,
            totalDebt      = 18700,
            dsr            = 67.3f,   // ← 40% 초과, 대출 불가
            hasOverdue     = false,
            overdueMonths  = 2,
            overdueDetail  = "3년 전 카드론 2개월 연체 (완납)",

            accountBalance       = 3200000,
            accountAgeDays       = 510,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.주의,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = false,
            rejectReason  = "DSR 67.3% — 규제 한도(40%) 초과로 추가 대출 불가"
        },

        // ④ 거절 — 신용점수 미달 체크카드 발급
        new CustomerData
        {
            customerName   = "최수아",
            accountNumber  = "440-5530-8821",
            gender         = Gender.Female,
            phone          = "010-2274-9903",
            jobType        = "무직",
            company        = "없음",
            workYears      = 0,
            annualIncome   = 0,

            requestType    = RequestType.카드발급,
            amount         = 0,
            memo           = "신용카드 신규 발급 요청",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = false,
            beneficialOwnerConfirmed = true,

            creditScore    = 432,    // ← 발급 최소 기준 600점 미달
            totalDebt      = 620,
            dsr            = 0f,
            hasOverdue     = true,   // 현재 연체 중
            overdueMonths  = 4,
            overdueDetail  = "현재 캐피탈 할부 4개월 연체 중",

            accountBalance       = 410000,
            accountAgeDays       = 230,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.주의,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = false,
            rejectReason  = "신용점수 432점 — 발급 기준(600점) 미달 / 현재 연체 이력 존재"
        },

        // ⑤ 거절 — 계좌 압류, 출금 불가
        new CustomerData
        {
            customerName   = "정도윤",
            accountNumber  = "550-3392-6677",
            gender         = Gender.Male,
            phone          = "010-8810-3345",
            jobType        = "정규직",
            company        = "현대건설(주)",
            workYears      = 9,
            annualIncome   = 6300,

            requestType    = RequestType.출금,
            amount         = 3000000,
            memo           = "급전 필요, 즉시 출금 요청",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 715,
            totalDebt      = 5200,
            dsr            = 24.1f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 25000000,
            accountAgeDays       = 2100,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.압류,  // ← 법원 압류

            fdsRisk              = FdsRiskLevel.정상,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = false,
            rejectReason  = "법원 압류 계좌 — 출금 불가 (담당 법원 확인 필요)"
        },

        // ⑥ 거절 — 보이스피싱 의심, 지급 유예
        new CustomerData
        {
            customerName   = "한지영",
            accountNumber  = "660-4421-0093",
            gender         = Gender.Female,
            phone          = "010-7732-8851",
            jobType        = "주부",
            company        = "없음",
            workYears      = 0,
            annualIncome   = 0,

            requestType    = RequestType.출금,
            amount         = 9800000,
            memo           = "전화 통화 중 내방, 빠른 처리 요청, 목적 진술 불일치",

            cddStatus      = CddStatus.재인증필요,
            fundsSourceVerified      = false,
            beneficialOwnerConfirmed = false,

            creditScore    = 601,
            totalDebt      = 0,
            dsr            = 0f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 11300000,
            accountAgeDays       = 3650,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.위험,  // ← FDS 위험 탐지
            suspectedPhishing    = true,               // ← 보이스피싱 의심
            suspectedBurnerAccount = false,

            isNormalCase  = false,
            rejectReason  = "보이스피싱 피해 의심 — 금융사기 예방 매뉴얼에 따라 지급 유예 조치"
        },

        // ⑦ 거절 — 단기 다수 계좌 개설 (대포통장 방지)
        new CustomerData
        {
            customerName   = "오승민",
            accountNumber  = "신규",
            gender         = Gender.Male,
            phone          = "010-4490-1127",
            jobType        = "아르바이트",
            company        = "편의점(단기)",
            workYears      = 0,
            annualIncome   = 800,

            requestType    = RequestType.신규계좌개설,
            amount         = 0,
            memo           = "급여 수령용 계좌 개설 요청, 증빙 서류 없음",

            cddStatus      = CddStatus.미완료,
            fundsSourceVerified      = false,
            beneficialOwnerConfirmed = false,

            creditScore    = 550,
            totalDebt      = 0,
            dsr            = 0f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 0,
            accountAgeDays       = 0,
            recentAccountOpened  = true,  // ← 최근 20영업일 내 타행 2개 개설
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.위험,
            suspectedPhishing    = false,
            suspectedBurnerAccount = true,  // ← 대포통장 의심

            isNormalCase  = false,
            rejectReason  = "최근 20영업일 내 타행 계좌 2건 개설 이력 — 대포통장 방지법 적용, 개설 거절"
        },

        // ⑧ 정상 — 분실 신고 (통장·도장)
        new CustomerData
        {
            customerName   = "윤채원",
            accountNumber  = "770-8812-4450",
            gender         = Gender.Female,
            phone          = "010-6630-7744",
            jobType        = "정규직",
            company        = "LG전자(주)",
            workYears      = 4,
            annualIncome   = 3800,

            requestType    = RequestType.분실신고,
            amount         = 0,
            memo           = "통장·도장 분실, 재발급 요청",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 803,
            totalDebt      = 2100,
            dsr            = 14.7f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 6900000,
            accountAgeDays       = 1560,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.정상,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = true,
            rejectReason  = ""
        },

        // ⑨ 거절 — CDD 거부, 거래 목적 진술 거부
        new CustomerData
        {
            customerName   = "임태양",
            accountNumber  = "880-2201-9934",
            gender         = Gender.Male,
            phone          = "010-1190-5521",
            jobType        = "무직",
            company        = "없음",
            workYears      = 0,
            annualIncome   = 0,

            requestType    = RequestType.신규계좌개설,
            amount         = 0,
            memo           = "계좌 개설 목적 진술 거부, 자금 원천 확인 거부",

            cddStatus      = CddStatus.거부,         // ← 고객이 거부
            fundsSourceVerified      = false,
            beneficialOwnerConfirmed = false,

            creditScore    = 498,
            totalDebt      = 0,
            dsr            = 0f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 0,
            accountAgeDays       = 0,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.차단,
            suspectedPhishing    = false,
            suspectedBurnerAccount = true,

            isNormalCase  = false,
            rejectReason  = "고객확인(CDD) 이행 거부 — 특정금융정보법 제5조에 따라 거래 거절"
        },

        // ⑩ 정상 — 입금 (타행 이체 수령 확인)
        new CustomerData
        {
            customerName   = "강다은",
            accountNumber  = "990-6673-1108",
            gender         = Gender.Female,
            phone          = "010-3310-8892",
            jobType        = "프리랜서",
            company        = "개인 디자인 스튜디오",
            workYears      = 5,
            annualIncome   = 3200,

            requestType    = RequestType.입금,
            amount         = 2500000,
            memo           = "프리랜서 용역비 수령 확인 요청",

            cddStatus      = CddStatus.완료,
            fundsSourceVerified      = true,
            beneficialOwnerConfirmed = true,

            creditScore    = 762,
            totalDebt      = 900,
            dsr            = 8.3f,
            hasOverdue     = false,
            overdueMonths  = 0,
            overdueDetail  = "없음",

            accountBalance       = 1850000,
            accountAgeDays       = 720,
            recentAccountOpened  = false,
            restriction          = AccountRestriction.없음,

            fdsRisk              = FdsRiskLevel.정상,
            suspectedPhishing    = false,
            suspectedBurnerAccount = false,

            isNormalCase  = true,
            rejectReason  = ""
        },

        // ⑪ 정상 — 공과금 납부
        new CustomerData
        {
            customerName = "송하준", accountNumber = "111-2093-4471",
            gender = Gender.Male, phone = "010-2231-7788",
            jobType = "정규직", company = "포스코(주)", workYears = 11, annualIncome = 7100,
            requestType = RequestType.공과금납부, amount = 184000,
            memo = "전기·수도 요금 자동이체 등록 및 당월 납부",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 821, totalDebt = 4100, dsr = 16.9f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 9200000, accountAgeDays = 3200, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ⑫ 정상 — 적금 가입
        new CustomerData
        {
            customerName = "배유나", accountNumber = "222-7741-9920",
            gender = Gender.Female, phone = "010-9982-3310",
            jobType = "공무원", company = "서울특별시청", workYears = 8, annualIncome = 4800,
            requestType = RequestType.적금가입, amount = 3000000,
            memo = "정기적금 36개월 가입, 월 30만원 납입",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 795, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 14600000, accountAgeDays = 2750, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ⑬ 거절 — 고액 외화환전, 자금 출처 미확인 (EDD 대상)
        new CustomerData
        {
            customerName = "조성훈", accountNumber = "333-1120-5567",
            gender = Gender.Male, phone = "010-4471-2289",
            jobType = "자영업", company = "무역업(개인)", workYears = 3, annualIncome = 5000,
            requestType = RequestType.외화환전, amount = 130000000,
            memo = "미화 10만 달러 환전 요청, 자금 출처 증빙 제출 거부",
            cddStatus = CddStatus.재인증필요, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 688, totalDebt = 3300, dsr = 22.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 47000000, accountAgeDays = 980, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.위험, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "고액 환전(1만 달러 초과) 자금 원천 미확인 — 강화된 고객확인(EDD) 미이행으로 거래 보류"
        },

        // ⑭ 거절 — 미성년자 신용카드 발급
        new CustomerData
        {
            customerName = "권시우", accountNumber = "444-9981-2230",
            gender = Gender.Male, phone = "010-3320-1145",
            jobType = "학생", company = "○○고등학교", workYears = 0, annualIncome = 0,
            requestType = RequestType.카드발급, amount = 0,
            memo = "신용카드 발급 요청 (만 17세)",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 0, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 320000, accountAgeDays = 120, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "미성년자(만 17세) — 신용카드 발급 불가 (체크카드만 가능)"
        },

        // ⑮ 정상 — 소액 여행 환전
        new CustomerData
        {
            customerName = "문아름", accountNumber = "555-2240-6678",
            gender = Gender.Female, phone = "010-6678-9921",
            jobType = "정규직", company = "네이버(주)", workYears = 2, annualIncome = 4500,
            requestType = RequestType.외화환전, amount = 1500000,
            memo = "일본 여행 경비 엔화 환전 (약 15만엔)",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 770, totalDebt = 1200, dsr = 9.5f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 5400000, accountAgeDays = 640, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ⑯ 거절 — 명의도용 의심 (타인 신분증)
        new CustomerData
        {
            customerName = "황준서", accountNumber = "666-3310-8845",
            gender = Gender.Male, phone = "010-1102-6634",
            jobType = "무직", company = "없음", workYears = 0, annualIncome = 0,
            requestType = RequestType.출금, amount = 8000000,
            memo = "제시한 신분증 사진과 실물 불일치, 서명 상이",
            cddStatus = CddStatus.재인증필요, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 540, totalDebt = 700, dsr = 5.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 9100000, accountAgeDays = 1500, recentAccountOpened = false, restriction = AccountRestriction.사고신고,
            fdsRisk = FdsRiskLevel.차단, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "신분증 위·변조 및 명의도용 의심 — 본인 확인 불가로 거래 거절, 사고 신고 접수"
        },

        // ⑰ 정상 — 잔액증명서 발급
        new CustomerData
        {
            customerName = "신예린", accountNumber = "777-4420-1193",
            gender = Gender.Female, phone = "010-8845-2207",
            jobType = "정규직", company = "삼성생명(주)", workYears = 7, annualIncome = 5600,
            requestType = RequestType.증명서발급, amount = 0,
            memo = "비자 발급용 잔액증명서 영문 발급 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 812, totalDebt = 2600, dsr = 13.1f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 33200000, accountAgeDays = 2980, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ⑱ 거절 — 한도제한계좌 초과 이체
        new CustomerData
        {
            customerName = "남기범", accountNumber = "888-5530-2274",
            gender = Gender.Male, phone = "010-2207-9938",
            jobType = "아르바이트", company = "물류센터(단기)", workYears = 0, annualIncome = 1100,
            requestType = RequestType.이체, amount = 5000000,
            memo = "한도제한계좌에서 500만원 이체 요청",
            cddStatus = CddStatus.미완료, fundsSourceVerified = false, beneficialOwnerConfirmed = true,
            creditScore = 612, totalDebt = 300, dsr = 3.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 6200000, accountAgeDays = 45, recentAccountOpened = true, restriction = AccountRestriction.한도제한,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "한도제한계좌 — 1일 이체 한도(30만원) 초과 / 증빙서류 제출 전 해제 불가"
        },

        // ⑲ 거절 — 잔액 부족 출금
        new CustomerData
        {
            customerName = "이준호", accountNumber = "112-3340-7781",
            gender = Gender.Male, phone = "010-3398-2241",
            jobType = "정규직", company = "한화시스템(주)", workYears = 4, annualIncome = 4300,
            requestType = RequestType.출금, amount = 2000000,
            memo = "200만원 현금 출금 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 768, totalDebt = 1500, dsr = 12.3f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 480000, accountAgeDays = 1100, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "계좌 잔액 부족 — 잔액 48만원으로 200만원 출금 불가"
        },

        // ⑳ 정상 — 신규계좌개설 (증빙 완비, 문제 없음)
        new CustomerData
        {
            customerName = "서지안", accountNumber = "신규",
            gender = Gender.Female, phone = "010-7741-3320",
            jobType = "정규직", company = "SK하이닉스(주)", workYears = 5, annualIncome = 5400,
            requestType = RequestType.신규계좌개설, amount = 0,
            memo = "급여이체용 입출금 계좌 개설, 재직증명서·신분증 지참",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 805, totalDebt = 1200, dsr = 9.8f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 0, accountAgeDays = 0, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉑ 거절 — 이체 잔액 부족
        new CustomerData
        {
            customerName = "홍성민", accountNumber = "113-9921-4470",
            gender = Gender.Male, phone = "010-2284-9970",
            jobType = "자영업", company = "성민상회(개인)", workYears = 6, annualIncome = 3600,
            requestType = RequestType.이체, amount = 3000000,
            memo = "거래처 대금 300만원 이체 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 712, totalDebt = 2400, dsr = 19.5f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 1200000, accountAgeDays = 1800, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "계좌 잔액 부족 — 잔액 120만원으로 300만원 이체 불가"
        },

        // ㉒ 정상 — 대출상담 (DSR 적정, 승인 가능)
        new CustomerData
        {
            customerName = "김도현", accountNumber = "114-3320-8891",
            gender = Gender.Male, phone = "010-5521-3308",
            jobType = "정규직", company = "현대모비스(주)", workYears = 8, annualIncome = 6800,
            requestType = RequestType.대출상담, amount = 30000000,
            memo = "전세자금 대출 3천만원 상담, 재직·소득 증빙 완비",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 798, totalDebt = 4200, dsr = 28.4f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 9300000, accountAgeDays = 2600, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉓ 거절 — 질권설정 계좌 출금
        new CustomerData
        {
            customerName = "박서준", accountNumber = "115-7740-2293",
            gender = Gender.Male, phone = "010-9930-4471",
            jobType = "정규직", company = "두산에너빌리티(주)", workYears = 7, annualIncome = 5900,
            requestType = RequestType.출금, amount = 2000000,
            memo = "200만원 출금 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 760, totalDebt = 3100, dsr = 21.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 15000000, accountAgeDays = 2200, recentAccountOpened = false, restriction = AccountRestriction.질권설정,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "질권설정 계좌 — 질권자 동의 없이는 출금 불가 (잔액과 무관)"
        },

        // ㉔ 정상 — 카드발급 (신용 우수)
        new CustomerData
        {
            customerName = "이하은", accountNumber = "116-2240-9914",
            gender = Gender.Female, phone = "010-3340-7782",
            jobType = "정규직", company = "신한은행", workYears = 4, annualIncome = 4700,
            requestType = RequestType.카드발급, amount = 0,
            memo = "신용카드 신규 발급 요청, 본인 신분증 지참",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 815, totalDebt = 900, dsr = 7.2f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 5600000, accountAgeDays = 1400, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉕ 거절 — 가압류 계좌 이체
        new CustomerData
        {
            customerName = "정민재", accountNumber = "117-9930-3348",
            gender = Gender.Male, phone = "010-4471-9920",
            jobType = "자영업", company = "민재푸드(개인)", workYears = 5, annualIncome = 4000,
            requestType = RequestType.이체, amount = 4000000,
            memo = "거래처 송금 400만원 이체 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 690, totalDebt = 5600, dsr = 33.2f,
            hasOverdue = false, overdueMonths = 1, overdueDetail = "1년 전 카드대금 1개월 연체 (완납)",
            accountBalance = 8200000, accountAgeDays = 1950, recentAccountOpened = false, restriction = AccountRestriction.가압류,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "가압류 계좌 — 법원 보전처분으로 이체·출금 제한 (잔액 있어도 처리 불가)"
        },

        // ㉖ 정상 — 입금 (급여 수령)
        new CustomerData
        {
            customerName = "최유진", accountNumber = "118-3340-1102",
            gender = Gender.Female, phone = "010-7782-3340",
            jobType = "정규직", company = "CJ제일제당(주)", workYears = 3, annualIncome = 3900,
            requestType = RequestType.입금, amount = 1800000,
            memo = "급여 현금 입금 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 782, totalDebt = 700, dsr = 6.5f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 2300000, accountAgeDays = 980, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉗ 정상 — 정기예금 가입
        new CustomerData
        {
            customerName = "윤서호", accountNumber = "119-4471-2280",
            gender = Gender.Male, phone = "010-3340-9921",
            jobType = "정규직", company = "LG화학(주)", workYears = 9, annualIncome = 6200,
            requestType = RequestType.정기예금, amount = 20000000,
            memo = "1년 만기 정기예금 2천만원 가입, 만기 자동해지",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 808, totalDebt = 1500, dsr = 8.9f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 28000000, accountAgeDays = 3100, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉘ 거절 — 고령 고객에게 고위험 펀드 권유 (적합성 원칙 위반)
        new CustomerData
        {
            customerName = "김영자", accountNumber = "120-7740-3318",
            gender = Gender.Female, phone = "010-9921-4470",
            jobType = "무직", company = "없음(연금생활)", workYears = 0, annualIncome = 1200,
            requestType = RequestType.펀드가입, amount = 30000000,
            memo = "만 78세, 투자성향 '안정형', 원금 전액으로 고위험 주식형 펀드 가입 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 730, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 31000000, accountAgeDays = 5200, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "적합성·적정성 원칙 위반 — 안정형 고령 투자자에게 고위험 상품 판매 불가 (금융소비자보호법)"
        },

        // ㉙ 거절 — 해외송금, 증빙 미비 + 자금세탁 의심
        new CustomerData
        {
            customerName = "장우혁", accountNumber = "121-3318-7742",
            gender = Gender.Male, phone = "010-4470-2289",
            jobType = "자영업", company = "무역업(개인)", workYears = 2, annualIncome = 3000,
            requestType = RequestType.해외송금, amount = 95000000,
            memo = "지인 사업자금 명목 9,500만원 해외송금, 송금 증빙·계약서 제출 거부",
            cddStatus = CddStatus.재인증필요, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 695, totalDebt = 2200, dsr = 18.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 110000000, accountAgeDays = 700, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.위험, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "외국환거래법상 증빙서류 미제출 + 자금세탁 의심 — 송금 거절 및 의심거래보고(STR) 대상"
        },

        // ㉚ 정상 — 통장 재발급 (분실, 본인확인 완료)
        new CustomerData
        {
            customerName = "오세진", accountNumber = "122-7742-9930",
            gender = Gender.Male, phone = "010-2289-3340",
            jobType = "정규직", company = "기아(주)", workYears = 6, annualIncome = 5100,
            requestType = RequestType.통장재발급, amount = 0,
            memo = "통장 분실, 신분증 지참 본인확인 완료, 재발급 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 791, totalDebt = 2000, dsr = 13.5f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 7400000, accountAgeDays = 2400, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉛ 정상 — 대출 중도상환
        new CustomerData
        {
            customerName = "배준영", accountNumber = "123-9930-4471",
            gender = Gender.Male, phone = "010-3340-7783",
            jobType = "정규직", company = "삼성SDI(주)", workYears = 10, annualIncome = 7300,
            requestType = RequestType.대출상환, amount = 10000000,
            memo = "신용대출 1천만원 중도 일부상환 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 820, totalDebt = 3000, dsr = 15.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 13500000, accountAgeDays = 3300, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉜ 거절 — 상속예금 인출, 상속 서류 미비
        new CustomerData
        {
            customerName = "신미경", accountNumber = "124-4471-3318",
            gender = Gender.Female, phone = "010-7783-9921",
            jobType = "주부", company = "없음", workYears = 0, annualIncome = 0,
            requestType = RequestType.상속처리, amount = 40000000,
            memo = "사망한 부친 예금 4천만원 인출 요청, 가족관계증명서·제적등본·상속인 전원 동의서 미제출",
            cddStatus = CddStatus.미완료, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 740, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 42000000, accountAgeDays = 4100, recentAccountOpened = false, restriction = AccountRestriction.사고신고,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "상속 관련 서류(제적등본·상속인 전원 동의 등) 미제출 — 상속예금 지급 불가"
        },

        // ㉝ 거절 — 대리인 거래, 위임장 없음
        new CustomerData
        {
            customerName = "권혁수", accountNumber = "125-3318-9930",
            gender = Gender.Male, phone = "010-9921-7784",
            jobType = "정규직", company = "포스코퓨처엠(주)", workYears = 4, annualIncome = 4600,
            requestType = RequestType.출금, amount = 3000000,
            memo = "배우자 계좌에서 300만원 대리 출금 요청, 위임장·인감증명서 없음",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = false,
            creditScore = 775, totalDebt = 1800, dsr = 12.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 9800000, accountAgeDays = 2700, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "대리 출금 — 위임장·본인 인감증명서 미제출로 대리인 거래 불가"
        },

        // ㉞ 정상 — 미성년자 계좌개설 (법정대리인 동반)
        new CustomerData
        {
            customerName = "이수민", accountNumber = "신규",
            gender = Gender.Female, phone = "010-7784-3318",
            jobType = "학생", company = "○○중학교", workYears = 0, annualIncome = 0,
            requestType = RequestType.신규계좌개설, amount = 0,
            memo = "만 14세, 법정대리인(모) 동반, 기본증명서·가족관계증명서·신분증 모두 지참",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 0, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 0, accountAgeDays = 0, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㉟ 정상 — 주택청약종합저축 가입
        new CustomerData
        {
            customerName = "한지민", accountNumber = "126-3318-4471",
            gender = Gender.Female, phone = "010-2280-3341",
            jobType = "정규직", company = "현대오토에버(주)", workYears = 3, annualIncome = 4000,
            requestType = RequestType.적금가입, amount = 100000,
            memo = "주택청약종합저축 신규 가입, 월 10만원 자동납입",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 780, totalDebt = 800, dsr = 7.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 4200000, accountAgeDays = 1100, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊱ 정상 — 압류계좌 입금 (입금은 압류와 무관하게 가능)
        new CustomerData
        {
            customerName = "백승호", accountNumber = "127-4471-9930",
            gender = Gender.Male, phone = "010-3341-7785",
            jobType = "정규직", company = "한국전력공사", workYears = 12, annualIncome = 6500,
            requestType = RequestType.입금, amount = 1000000,
            memo = "계좌 압류 상태이나 현금 100만원 입금 요청 (입금은 가능)",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 760, totalDebt = 4000, dsr = 20.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 300000, accountAgeDays = 2600, recentAccountOpened = false, restriction = AccountRestriction.압류,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊲ 정상 — 한도제한계좌 소액 이체 (1일 한도 30만원 이내)
        new CustomerData
        {
            customerName = "노유진", accountNumber = "128-9930-3318",
            gender = Gender.Female, phone = "010-7785-2281",
            jobType = "아르바이트", company = "카페(단기)", workYears = 0, annualIncome = 1000,
            requestType = RequestType.이체, amount = 250000,
            memo = "한도제한계좌에서 25만원 이체 (1일 한도 30만원 이내)",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 640, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 1800000, accountAgeDays = 60, recentAccountOpened = false, restriction = AccountRestriction.한도제한,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊳ 정상 — 연금 수령
        new CustomerData
        {
            customerName = "곽재현", accountNumber = "129-3318-7742",
            gender = Gender.Male, phone = "010-2281-3342",
            jobType = "무직", company = "없음(연금생활)", workYears = 0, annualIncome = 1800,
            requestType = RequestType.연금수령, amount = 1200000,
            memo = "국민연금 당월 수령 확인 및 인출, 본인 신분증 지참",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 720, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 5600000, accountAgeDays = 4800, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊴ 정상 — 자동이체 등록
        new CustomerData
        {
            customerName = "류시현", accountNumber = "130-7742-9930",
            gender = Gender.Female, phone = "010-3342-7786",
            jobType = "정규직", company = "쿠팡(주)", workYears = 4, annualIncome = 4300,
            requestType = RequestType.자동이체, amount = 450000,
            memo = "월세·관리비 자동이체 등록 요청, 출금계좌 본인 명의",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 785, totalDebt = 1500, dsr = 11.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 6300000, accountAgeDays = 1500, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊵ 정상 — 예금담보대출
        new CustomerData
        {
            customerName = "진동현", accountNumber = "131-9930-4471",
            gender = Gender.Male, phone = "010-7786-2282",
            jobType = "정규직", company = "한화에어로스페이스(주)", workYears = 8, annualIncome = 6800,
            requestType = RequestType.예금담보대출, amount = 15000000,
            memo = "본인 정기예금(3천만원) 담보로 1,500만원 대출 요청, 담보 이상 없음",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 812, totalDebt = 2000, dsr = 14.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 30000000, accountAgeDays = 3500, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊶ 정상 — 자기앞수표 발행
        new CustomerData
        {
            customerName = "차예슬", accountNumber = "132-4471-3318",
            gender = Gender.Female, phone = "010-2282-3343",
            jobType = "정규직", company = "롯데케미칼(주)", workYears = 6, annualIncome = 5200,
            requestType = RequestType.수표발행, amount = 5000000,
            memo = "전세 계약금 자기앞수표 500만원 발행 요청, 잔액 충분",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 795, totalDebt = 1800, dsr = 12.5f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 9200000, accountAgeDays = 2200, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊷ 정상 — 계좌 비밀번호 변경
        new CustomerData
        {
            customerName = "석민규", accountNumber = "133-3318-9930",
            gender = Gender.Male, phone = "010-3343-7787",
            jobType = "정규직", company = "KT(주)", workYears = 5, annualIncome = 4900,
            requestType = RequestType.비밀번호변경, amount = 0,
            memo = "계좌 비밀번호 변경 요청, 본인 신분증·통장 지참",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 800, totalDebt = 1000, dsr = 8.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 7700000, accountAgeDays = 1900, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = true, rejectReason = ""
        },

        // ㊸ 거절 — 고령자 변액보험 부적합 판매
        new CustomerData
        {
            customerName = "봉하영", accountNumber = "134-9930-3318",
            gender = Gender.Female, phone = "010-7787-2283",
            jobType = "무직", company = "없음(연금생활)", workYears = 0, annualIncome = 1100,
            requestType = RequestType.보험가입, amount = 25000000,
            memo = "만 75세, 투자성향 '안정형', 원금 손실 가능한 변액보험 일시납 가입 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 715, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 26000000, accountAgeDays = 5500, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "고령·안정형 고객에게 원금손실 위험 변액보험 판매 — 적합성 원칙 위반(불완전판매) 우려로 거절"
        },

        // ㊹ 거절 — 카드 한도 상향, 소득 대비 과다 + 연체
        new CustomerData
        {
            customerName = "모정훈", accountNumber = "135-3318-7742",
            gender = Gender.Male, phone = "010-2283-3344",
            jobType = "아르바이트", company = "배달대행(단기)", workYears = 0, annualIncome = 1500,
            requestType = RequestType.카드한도상향, amount = 10000000,
            memo = "신용카드 한도 1천만원으로 상향 요청 (현재 한도 300만)",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 520, totalDebt = 2800, dsr = 0f,
            hasOverdue = true, overdueMonths = 2, overdueDetail = "현재 카드대금 2개월 연체 중",
            accountBalance = 600000, accountAgeDays = 400, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "소득 대비 과다 한도 요청 + 현재 연체 중 — 카드 한도 상향 불가"
        },

        // ㊺ 거절 — 질권설정 계좌 해지
        new CustomerData
        {
            customerName = "도경수", accountNumber = "136-7742-9930",
            gender = Gender.Male, phone = "010-3344-7788",
            jobType = "정규직", company = "GS칼텍스(주)", workYears = 7, annualIncome = 5800,
            requestType = RequestType.계좌해지, amount = 0,
            memo = "예금 계좌 해지 요청, 단 해당 예금에 질권 설정되어 있음",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 770, totalDebt = 3200, dsr = 18.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 12000000, accountAgeDays = 2400, recentAccountOpened = false, restriction = AccountRestriction.질권설정,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "질권설정 계좌 — 질권자 동의 없이는 계좌 해지 불가"
        },

        // ㊻ 거절 — 자기앞수표 발행, 잔액 부족
        new CustomerData
        {
            customerName = "하지원", accountNumber = "137-9930-4471",
            gender = Gender.Female, phone = "010-7788-2284",
            jobType = "자영업", company = "지원공방(개인)", workYears = 3, annualIncome = 2600,
            requestType = RequestType.수표발행, amount = 8000000,
            memo = "800만원 자기앞수표 발행 요청",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 700, totalDebt = 1200, dsr = 14.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 1500000, accountAgeDays = 1300, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.정상, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "계좌 잔액 부족 — 잔액 150만원으로 800만원 수표 발행 불가"
        },

        // ㊼ 거절 — 고령자 보이스피싱 의심 이체
        new CustomerData
        {
            customerName = "천소율", accountNumber = "138-4471-3318",
            gender = Gender.Female, phone = "010-2284-3345",
            jobType = "무직", company = "없음(연금생활)", workYears = 0, annualIncome = 1000,
            requestType = RequestType.이체, amount = 20000000,
            memo = "만 72세, '아들이 사고나서 합의금 필요'하다며 모르는 계좌로 2천만원 이체 요청, 통화 중 내방",
            cddStatus = CddStatus.재인증필요, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 690, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 35000000, accountAgeDays = 6000, recentAccountOpened = false, restriction = AccountRestriction.없음,
            fdsRisk = FdsRiskLevel.위험, suspectedPhishing = true, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "보이스피싱(자녀 사칭) 피해 의심 — 금융사기 예방 매뉴얼에 따라 이체 보류 및 지급 유예"
        },

        // ㊽ 거절 — 명의도용 의심 카드발급
        new CustomerData
        {
            customerName = "구본혁", accountNumber = "139-3318-9930",
            gender = Gender.Male, phone = "010-3345-7789",
            jobType = "무직", company = "없음", workYears = 0, annualIncome = 0,
            requestType = RequestType.카드발급, amount = 0,
            memo = "신용카드 발급 요청, 제시한 신분증 사진·실물 불일치, 서명 상이",
            cddStatus = CddStatus.재인증필요, fundsSourceVerified = false, beneficialOwnerConfirmed = false,
            creditScore = 560, totalDebt = 500, dsr = 4.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 900000, accountAgeDays = 800, recentAccountOpened = false, restriction = AccountRestriction.사고신고,
            fdsRisk = FdsRiskLevel.차단, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "신분증 위·변조 및 명의도용 의심 — 본인 확인 불가로 발급 거절, 사고 신고 접수"
        },

        // ㊾ 거절 — 한도제한계좌 초과 자동이체 등록
        new CustomerData
        {
            customerName = "표은지", accountNumber = "140-9930-3318",
            gender = Gender.Female, phone = "010-7789-2285",
            jobType = "아르바이트", company = "편의점(단기)", workYears = 0, annualIncome = 900,
            requestType = RequestType.자동이체, amount = 1000000,
            memo = "한도제한계좌에서 매월 100만원 자동이체 등록 요청",
            cddStatus = CddStatus.미완료, fundsSourceVerified = false, beneficialOwnerConfirmed = true,
            creditScore = 600, totalDebt = 0, dsr = 0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 2000000, accountAgeDays = 50, recentAccountOpened = true, restriction = AccountRestriction.한도제한,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "한도제한계좌 — 1일 이체 한도(30만원) 초과 자동이체 등록 불가 (증빙 제출 후 해제 필요)"
        },

        // ㊿ 거절 — 예금담보대출, 담보 예금 압류 상태
        new CustomerData
        {
            customerName = "조은별", accountNumber = "141-3318-7742",
            gender = Gender.Female, phone = "010-2285-3346",
            jobType = "자영업", company = "은별꽃집(개인)", workYears = 4, annualIncome = 3200,
            requestType = RequestType.예금담보대출, amount = 10000000,
            memo = "정기예금 담보로 1천만원 대출 요청, 단 해당 예금이 압류된 상태",
            cddStatus = CddStatus.완료, fundsSourceVerified = true, beneficialOwnerConfirmed = true,
            creditScore = 705, totalDebt = 2400, dsr = 19.0f,
            hasOverdue = false, overdueMonths = 0, overdueDetail = "없음",
            accountBalance = 18000000, accountAgeDays = 2100, recentAccountOpened = false, restriction = AccountRestriction.압류,
            fdsRisk = FdsRiskLevel.주의, suspectedPhishing = false, suspectedBurnerAccount = false,
            isNormalCase = false,
            rejectReason = "담보 예금이 압류 상태 — 담보 효력 없어 예금담보대출 실행 불가"
        },
    };

    // ── 유틸 ──────────────────────────────────────────────────
    static readonly string[] FemaleNameEndings = { "연","아","희","영","지","수","은","혜","나","란","미","원","채","다","영" };

    public static Gender GuessGender(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length < 2) return Gender.Male;
        string last = name.Substring(name.Length - 1);
        foreach (var e in FemaleNameEndings)
            if (last == e) return Gender.Female;
        return Gender.Male;
    }
}
