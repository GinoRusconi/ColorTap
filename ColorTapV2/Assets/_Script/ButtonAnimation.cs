using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Scripting.APIUpdating;

public class ButtonAnimation : MonoBehaviour
{

    public RectTransform rectTransformPlayer1;
    public RectTransform rectTransformPlayer2;
    public Ease EaseMode;

    private void Start() {
        GameManagement.Instance.OnStartGameOrFinishMode += MoveButtons;        
    }

    private void MoveButtons ()
    {
        float to1 = -rectTransformPlayer1.anchoredPosition.x;
        float to2 = -rectTransformPlayer2.anchoredPosition.x;
        rectTransformPlayer1.DOAnchorPos(new Vector2(to1, 0), 1, true)
                                .SetEase(EaseMode);

        rectTransformPlayer2.DOAnchorPos(new Vector2(to2, 0), 1, true)
                                .SetEase(EaseMode);
    }

    private void OnDisable() {
        GameManagement.Instance.OnStartGameOrFinishMode -= MoveButtons;
    }
}
