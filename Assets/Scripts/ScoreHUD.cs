using UnityEngine;
using TMPro;

// 우측 상단 점수/직급/불만 HUD + 게임오버(권고사직) 패널
public class ScoreHUD : MonoBehaviour
{
    [Header("HUD Texts")]
    public TMP_Text txtRank;        // 직급
    public TMP_Text txtScore;       // 점수
    public TMP_Text txtComplaints;  // 불만 접수 X / 5
    public TMP_Text txtTimer;       // 응대 제한시간
    public TMP_Text txtQueue;       // 남은 대기 고객

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text   txtGameOver;

    void Awake()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void Refresh(string rank, int score, int complaints, int maxComplaints)
    {
        if (txtRank  != null) txtRank.text  = $"직급  {rank}";
        if (txtScore != null) txtScore.text = $"점수  {score}";

        if (txtComplaints != null)
        {
            string col = complaints >= maxComplaints - 1 ? "#FF4444"
                       : complaints > 0                    ? "#FFD700"
                       :                                     "#00FF88";
            txtComplaints.text = $"불만 접수  <color={col}>{complaints} / {maxComplaints}</color>";
        }
    }

    // 응대 제한시간 표시 (남은 초 / 전체)
    public void SetTimer(float remaining, float limit)
    {
        if (txtTimer == null) return;
        if (remaining < 0f) remaining = 0f;
        string col = remaining <= 5f ? "#FF4444"
                   : remaining <= 10f ? "#FFD700"
                   :                    "#00FF88";
        txtTimer.text = $"응대 시간  <color={col}>{remaining:F0}s</color>";
    }

    public void HideTimer()
    {
        if (txtTimer != null) txtTimer.text = "응대 시간  -";
    }

    // 남은 대기 고객 수
    public void SetQueue(int remaining)
    {
        if (txtQueue == null) return;
        txtQueue.text = $"대기 고객  {remaining}명";
    }

    public void ShowGameOver(string msg)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (txtGameOver != null)
        {
            txtGameOver.color = new Color(1f, 0.4f, 0.4f);
            txtGameOver.text  = msg;
        }
    }

    // 하루 업무 정상 종료 — 결산 화면 (성공 톤)
    public void ShowDayComplete(string msg)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (txtGameOver != null)
        {
            txtGameOver.color = new Color(0.5f, 1f, 0.7f);
            txtGameOver.text  = msg;
        }
    }
}
