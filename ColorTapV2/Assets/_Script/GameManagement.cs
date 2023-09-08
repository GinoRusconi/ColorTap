using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class GameManagement : MonoBehaviour
{
    #region Singleton
    public static GameManagement Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _MixColor = FindObjectOfType<MixColor>();
        _ButtonsManager = FindObjectOfType<ButtonsManager>();
        _UiManagement = FindObjectOfType<UiManagement>();
    }
    #endregion Singleton
    public MemoryMode memoryMod;
    public VelocityMode velocityMod;
    public IGameMode gameMode;
    
    public Action WinPlayerRound;
    public Action ResetRound;
   

    [HideInInspector]public ButtonsManager _ButtonsManager;

    [HideInInspector] public UiManagement _UiManagement;
    private int countMatchPlaying = 0;
    private readonly int matchsToShowAds = 1;
    public GameObject menuMod;

    

    public TextMeshProUGUI textPlayerWin;

    public Sprite winSprite;
    public Sprite loseSprite;


    [HideInInspector] public MixColor _MixColor;

    public InterstitialAdExample interstitialAdExample;
    
    public void SetGameMode(int gameMode)
    {
        switch (gameMode)
        {
            case 1: PlayGame(velocityMod); break;
            case 2: PlayGame(memoryMod); break;
        }
    }


    private void PlayGame(IGameMode gameMode)
    {
        if (countMatchPlaying >= matchsToShowAds)
        {
            countMatchPlaying = 0;
            interstitialAdExample.ShowAd();

        } else
        {
            menuMod.SetActive(false);
            countMatchPlaying++;
            this.gameMode = gameMode;
            gameMode.IGameMode(this, _MixColor);
            gameMode.NewRound();
        }
    }

    public IEnumerator FinishMatch(PlayerID playerID)
    {
        _UiManagement.ResetDefault();
        yield return StartCoroutine(_UiManagement.TextPlayer(playerID,"win"));
        _UiManagement.DesactivatedScore();
        menuMod.SetActive(true);
        _ButtonsManager.ResetDefaultButtons();
        
        interstitialAdExample.LoadAd();
        yield return new WaitForFixedUpdate();
    }
}
