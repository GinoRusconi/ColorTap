using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    
    public RectTransform[] rectUI;
    private Vector2[] init;
    public Ease easemode;
    public float duration;
    public bool snappingSmooth;
    Tween mytween;
    
    private void Awake()
    {
        init = new Vector2[rectUI.Length]; 
        for (int i = 0; i < rectUI.Length; i++)
        {
            init[i] = rectUI[i].anchoredPosition;
        }
        
    }
    
    private void OnEnable() {

        //ReverseAnimationToCurrent();
        GameManagement.Instance.CloseUI += ReverseAnimationToCurrent;
        //currentState = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < rectUI.Length; i++)
        {
            rectUI[i].DOAnchorPos(new Vector2(init[i].x,0), duration, snappingSmooth)
                .SetEase(easemode);
        }
    }

    public void ReverseAnimationToCurrent()
    {
        //animator.SetTrigger(HashUIChallenge);
        StartCoroutine(AnimationUI());
       // animator.speed *= -1;
       // gameObject.SetActive(false);
    } 

    public IEnumerator AnimationUI()
    {
        
        for (int i = 0; i < rectUI.Length; i++)
        {
            if(i == rectUI.Length - 1)
            {
                mytween = rectUI[i].DOAnchorPos(new Vector2(init[i].x,init[i].y), duration, snappingSmooth)
                .SetEase(easemode);
                break;
            }
            rectUI[i].DOAnchorPos(new Vector2(init[i].x,init[i].y), duration, snappingSmooth)
                .SetEase(easemode);
        }
         
        yield return mytween.WaitForCompletion();

        gameObject.SetActive(false);
    }

    private void OnDisable() {
        GameManagement.Instance.CloseUI -= ReverseAnimationToCurrent;
    }
}
