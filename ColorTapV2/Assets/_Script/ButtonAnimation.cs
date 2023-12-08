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
        float to1 = -rectTransformPlayer1.anchoredPosition.y;
        float to2 = -rectTransformPlayer2.anchoredPosition.y;
        rectTransformPlayer1.DOAnchorPos(new Vector2(0, to1), 1, true)
                                .SetEase(EaseMode);

        rectTransformPlayer2.DOAnchorPos(new Vector2(0,to2), 1, true)
                                .SetEase(EaseMode);
    }

    private void OnDisable() {
        GameManagement.Instance.OnStartGameOrFinishMode -= MoveButtons;
    }
}
