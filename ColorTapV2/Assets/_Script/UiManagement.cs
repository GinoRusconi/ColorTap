using System.Collections;
using TMPro;
using UnityEngine;

public class UiManagement : MonoBehaviour
{
    public TextMeshProUGUI scorePlayer1;
    public TextMeshProUGUI scorePlayer2;

    public GameObject GOTextPlayerWin;
    private Animator animator;
    private TextMeshProUGUI PlayerWinGUI;
    private RectTransform RectTransformTextPlayerWin;

    public GameObject challengeUI;
    public TextMeshProUGUI challengeScore;
    public TextMeshProUGUI challengeTimeText;

    public int scoreP1;
    public int scoreP2;

    private readonly int HashShowPlayerWin = Animator.StringToHash("ShowWinPlayer");
    private void Awake()
    {
        animator = GOTextPlayerWin.GetComponent<Animator>();
        PlayerWinGUI = GOTextPlayerWin.GetComponent<TextMeshProUGUI>();
        RectTransformTextPlayerWin = GOTextPlayerWin.GetComponent<RectTransform>();


    }

    public void EnabledUI(bool enabled)
    {
        if(enabled)
        {
            challengeScore.text = "0";
            challengeTimeText.text = "10:00";
            challengeUI.SetActive(enabled);
        }
        else
        {
            challengeUI.SetActive(enabled);
        }
    }

    public void UpdateScore(PlayerID playerID)
    {
        //StopAllCoroutines();
        switch (playerID)
        {
            case PlayerID.Player1:
                scoreP1++;
                scorePlayer1.text = $"{scoreP1}";
                StartCoroutine(DeformScore(scorePlayer1));
                break;
            case PlayerID.Player2:
                scoreP2++;
                scorePlayer2.text = $"{scoreP2}";
                StartCoroutine(DeformScore(scorePlayer2));
                break;
        }
    }

    public void UpdateScore(float score)
    {
        challengeScore.text = string.Format("{0:F2}", score);
    }

    public void UpdateTimerUI(float timer)
    {
        int seconds = Mathf.FloorToInt(timer); // Obtiene los segundos como parte entera.
        int milliseconds = Mathf.FloorToInt((timer - seconds) * 1000); // Obtiene las milésimas.

        challengeTimeText.text = string.Format("{0:D2}.{1:D2}", seconds, milliseconds);
    }

    public IEnumerator DeformScore(TMP_Text tmpText)
    {
        float duration = 0.2f; // Duraci�n en segundos
        float elapsedTime = 0f;

        float startFontSize = tmpText.fontSize;
        float targetFontSize = 50f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Normalizar el tiempo entre 0 y 1

            tmpText.fontSize = Mathf.Lerp(startFontSize, targetFontSize, t);

            yield return null;
        }

        // Asegurarse de que la fuente tenga el tama�o final
        tmpText.fontSize = startFontSize;
    }

    private void RotateRectTransform(RectTransform rectTransform, float rotationZ)
    {
        Quaternion targetQuaternion = Quaternion.Euler(0f, 0f, rotationZ);
        rectTransform.rotation = targetQuaternion;
    }

    public IEnumerator TextPlayer(PlayerID playerWiner, string text)
    {
        Vector3 currentPosition = RectTransformTextPlayerWin.anchoredPosition;
        PlayerWinGUI.text = text;

        switch (playerWiner)
        {
            case PlayerID.Player2:
                if (PositiveOrNegative(currentPosition.x))
                {
                    currentPosition.x *= -1;
                    RotateRectTransform(RectTransformTextPlayerWin, 270f);
                    RectTransformTextPlayerWin.anchoredPosition = currentPosition;
                }
                break;
            case PlayerID.Player1:
                if (!PositiveOrNegative(currentPosition.x))
                {
                    currentPosition.x *= -1;
                    RotateRectTransform(RectTransformTextPlayerWin, 90f);
                    RectTransformTextPlayerWin.anchoredPosition = currentPosition;
                }
                break;
        }

        

        // Activar la animaci�n
        animator.SetTrigger(HashShowPlayerWin); // Inicia la animaci�n

        yield return null; // Espera hasta la siguiente actualizaci�n del frame

        bool isThatAnimation = true;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (animator.IsInTransition(0))
        {
            yield return null;
        }

        while (isThatAnimation && stateInfo.normalizedTime < 1f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isThatAnimation = stateInfo.IsName("MoveInfoPlayer");
            yield return null; // Espera hasta la siguiente actualizaci�n del frame
        }

    }

    private bool PositiveOrNegative(float num)
    {
        num = Mathf.Sign(num);
        if (num == 1) return true;
        else return false;
    }

    public void DesactivatedScore()
    {
        scorePlayer1.text = $"";
        scorePlayer2.text = $"";
    }

    public void ForcedUpdateUI(int player1, int player2)
    {
        scorePlayer1.text = $"{player1}";
        scorePlayer2.text = $"{player2}";
    }

    public void ResetDefault()
    {
        scoreP1 = 0;
        scoreP2 = 0;    
    }
}
