using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UiManagement : MonoBehaviour
{
    public TMP_Text scorePlayer1;
    public TMP_Text scorePlayer2;

    public GameObject GOTextPlayerWin;
    private Animator animator;
    private TMP_Text PlayerWinGUI;
    public RectTransform RectTransformTextPlayerWin;
    private AudioSource audioSource;

    public GameObject challengeUI;
    public TMP_Text challengeScore;
    public TMP_Text challengeTimeText;

    public int scoreP1;
    public int scoreP2;

    Sequence mySequence;
    public float timeAnimationDelay;
    private float to;

    private readonly int HashShowPlayerWin = Animator.StringToHash("ShowWinPlayer");
    private void Awake()
    {
        animator = GOTextPlayerWin.GetComponent<Animator>();
        PlayerWinGUI = GOTextPlayerWin.GetComponent<TMP_Text>();
        //RectTransformTextPlayerWin = GOTextPlayerWin.GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();

        
                    
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
            case PlayerID.Player1:
                if (PositiveOrNegative(currentPosition.y))
                {
                    currentPosition.y *= -1;
                    RotateRectTransform(RectTransformTextPlayerWin, 0f);
                    RectTransformTextPlayerWin.anchoredPosition = currentPosition;
                }
                break;
            case PlayerID.Player2:
                if (!PositiveOrNegative(currentPosition.y))
                {
                    currentPosition.y *= -1;
                    RotateRectTransform(RectTransformTextPlayerWin, 180f);
                    RectTransformTextPlayerWin.anchoredPosition = currentPosition;
                }
                break;
        }

       to = -RectTransformTextPlayerWin.anchoredPosition.x;
        
        mySequence = DOTween.Sequence();
        mySequence.Append(RectTransformTextPlayerWin.DOAnchorPosX(0,0.1f,true))
                    .AppendCallback(()=>audioSource.Play())
                    .AppendInterval(timeAnimationDelay)
                    .Append(RectTransformTextPlayerWin.DOAnchorPosX(to,0.1f,true)).SetEase(Ease.InOutQuint);
                    
        //audioSource.Play();
        yield return mySequence.WaitForCompletion();
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

    private void OnDestroy() {
        
    }
}
