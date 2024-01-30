using UnityEngine;
using TMPro;
using System.Collections;
using System;
using GooglePlayGames;

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
        _audioSource = GetComponent<AudioSource>();
        _canvasGroupMenu = menuMod.GetComponent<CanvasGroup>();
    }
    #endregion Singleton
   
    #region GamesModes
    [Header("Mods")]
    public MemoryMode MemoryMod;
    public VelocityMode VelocityMod;
    public ChallengeMode ChallengeMod; 
    public IGameMode GameMode { get; private set; }
    #endregion GamesModes

    [Header("Referencias")]
    public Camera _mainCamera;
    public GameObject buttonNewLife;
    public GameObject menuMod;
    public TextMeshProUGUI textPlayerWin;
    public TextMeshProUGUI tmpTutorial;
    [HideInInspector] public ButtonsManager _ButtonsManager;
    [HideInInspector] public UiManagement _UiManagement;
    [HideInInspector] public MixColor _MixColor;
    public PlayGameService playGameService;
    private AudioSource _audioSource;
    private CanvasGroup _canvasGroupMenu;
    
    [Header("Reference Sprites")]
    public Sprite winSprite;
    public Sprite loseSprite;


    [Header("Particles")]
    public ParticleSystem _particleSystemMenu;
    public ParticleSystem _particleSystemWin;

    
    private int _currentColorIndex = 0;
    private int _targetIndexColor = 1;
    private float _targetPoint;
    private const float TimeDelayColor = 2;
    

    public Action OnStartGameOrFinishMode;
    public Action OnWinPlayerRound;
    public Action OnCloseUI;
    public Action OnResetRound;
   
    private int _countMatchPlaying = 0;
    private bool _IsShowTutorialPlayGameService;
    public GameObject recomendationPlayGame;
    private bool _enableMenu = true;
    private const int MatchsToShowAds = 1;

    //public InterstitialAdExample interstitialAdExample;

    
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update() {

        if(_enableMenu){

            _targetPoint += Time.deltaTime / TimeDelayColor;
            _mainCamera.backgroundColor = Color.Lerp(_MixColor.colors[_currentColorIndex],_MixColor.colors[_targetIndexColor],_targetPoint);

            if(_targetPoint >= 1f){
                _targetPoint = 0f;

                _currentColorIndex = _targetIndexColor;
                _targetIndexColor++;
                if(_targetIndexColor == _MixColor.colors.Length){
                    _targetIndexColor = 0;
                }
            }
        }
    }

    public void SetGameMode(int gameMode)
    {
        _audioSource.Stop();
        switch (gameMode)
        {
            case 1: PlayGame(VelocityMod); break;
            case 2: PlayGame(MemoryMod); break;
            case 3: PlayGame(ChallengeMod); break;
        }
        playGameService.HideUI();
    }



    private void PlayGame(IGameMode gameMode)
    {
        if (_countMatchPlaying >= MatchsToShowAds)
        {
            _countMatchPlaying = 0;
            //interstitialAdExample.ShowAd();

        } else
        {
            StartCoroutine(InitMod(gameMode));
        }
    }

    private IEnumerator InitMod(IGameMode gameMode)
    {
        _canvasGroupMenu.interactable = false;
        OnCloseUI?.Invoke();
        yield return new WaitForSeconds(1f);
        _enableMenu = false;
        _particleSystemMenu.Stop();
        //countMatchPlaying++;
        GameMode = gameMode;
        GameMode.IGameMode(this, _MixColor);
        GameMode.NewRound();
    }

    public IEnumerator FinishMatchTwoPlayer(PlayerID playerID)
    {
        _UiManagement.ResetDefault();
        OnStartGameOrFinishMode?.Invoke();
        FireWorks(playerID);
        yield return StartCoroutine(_UiManagement.TextPlayer(playerID, "win"));
        
        while (_particleSystemWin.gameObject.activeSelf)
        {
            yield return null;
        }

        _UiManagement.DesactivatedScore();
        _audioSource.Play();
        menuMod.SetActive(true);
        _canvasGroupMenu.interactable = true;
        _enableMenu = true;
        _particleSystemMenu.Play();
        playGameService.HideUI();
        _ButtonsManager.ResetDefaultButtons();

        

        //interstitialAdExample.LoadAd();
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator FinishMatchChallenge(float score)
    {
        OnStartGameOrFinishMode?.Invoke();
         //UpdateLeaderboard
        if(playGameService.UpgradeLeaderBoard(score)) yield return NewHighScore(score);
            
        
        OnCloseUI?.Invoke();
        //_UiManagement.EnabledUI(false);
        //ResetAll
        yield return new WaitForSeconds(1.5f);
        _audioSource.Play();
        menuMod.SetActive(true);
        _canvasGroupMenu.interactable = true;
        _enableMenu = true;
        _particleSystemMenu.Play();
        playGameService.HideUI();
        _ButtonsManager.ResetDefaultButtons();
        yield return null;
    }

    private IEnumerator NewHighScore(float newScore)
    {
        FireWorks(PlayerID.Player1);
        yield return _UiManagement.TextPlayer(PlayerID.Player1, "HIGHSCORE");
        while (_particleSystemWin.gameObject.activeSelf)
        {
            yield return null;
        }
    }

    public void NewLifeAds(bool condition)
    {
        buttonNewLife.SetActive(condition);
    }

    private void FireWorks(PlayerID playerID)
    {
        // Calcula las coordenadas en el mundo para los bordes Inferior y superior de la pantalla
        Vector3 leftEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1, _mainCamera.nearClipPlane));
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, _mainCamera.nearClipPlane));

        switch (playerID)
        {
            case PlayerID.Player2:
                _particleSystemWin.gameObject.transform.SetPositionAndRotation(leftEdge, Quaternion.Euler(0, 0, 90));
                break;
            case PlayerID.Player1:
                _particleSystemWin.gameObject.transform.SetPositionAndRotation(rightEdge, Quaternion.Euler(0, 0, 270));
                break;
        }

        _particleSystemWin.gameObject.SetActive(true);
    }

    public IEnumerator ShowTutorial(String Text){

        tmpTutorial.gameObject.SetActive(true);
        tmpTutorial.text = Text;
        yield return null; 
        while (Input.touchCount == 0)
        {
            yield return null;   
        }

        tmpTutorial.text = " ";
        tmpTutorial.gameObject.SetActive(false);
    }
}
