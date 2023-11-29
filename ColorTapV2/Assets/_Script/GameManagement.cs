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
        _AudioSource = GetComponent<AudioSource>();
    }
    #endregion Singleton
    public MemoryMode memoryMod;
    public VelocityMode velocityMod;
    public ChallengeMod challengeMod;

    public float HighScore = 0;

    public IGameMode gameMode;
    public Camera _mainCamera;
    public ParticleSystem _particleSystemMenu;
    public ParticleSystem _particleSystemWin;
    private float topEdgeY;
    private float bottomEdgeY;

    public int currentColorIndex = 0;
    public int targetIndexColor = 1;
    public float targetPoint;
    private float timeDelayColor = 2;

    public Action OnStartGameOrFinishMode;
    public Action WinPlayerRound;
    public Action CloseUI;
    public Action ResetRound;
   
    public AudioClip mainMenuAudio;
    private AudioSource _AudioSource;

    [HideInInspector]public ButtonsManager _ButtonsManager;

    [HideInInspector] public UiManagement _UiManagement;
    public GameObject buttonNewLife;
    private int countMatchPlaying = 0;
    private readonly int matchsToShowAds = 1;
    public GameObject menuMod;
    private bool _EnableMenu = true;

    public Animator animatorPlayer1;
    public Animator animatorPlayer2;

    public TextMeshProUGUI textPlayerWin;

    public Sprite winSprite;
    public Sprite loseSprite;

    public TextMeshProUGUI tmpTutorial;


    [HideInInspector] public MixColor _MixColor;

    public InterstitialAdExample interstitialAdExample;
    
    private void Start() {
        // Obtén las dimensiones de la pantalla
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        menuMod.SetActive(true);
        // Calcula la posición del borde superior e inferior a la mitad de la pantalla
        topEdgeY = screenHeight * 0.5f;
        bottomEdgeY = -screenHeight * 0.5f;    
    }

    private void Update() {

        if(_EnableMenu){

            targetPoint += Time.deltaTime / timeDelayColor;
            _mainCamera.backgroundColor = Color.Lerp(_MixColor.colors[currentColorIndex],_MixColor.colors[targetIndexColor],targetPoint);

            if(targetPoint >= 1f){
                targetPoint = 0f;

                currentColorIndex = targetIndexColor;
                targetIndexColor++;
                if(targetIndexColor == _MixColor.colors.Length){
                    targetIndexColor = 0;
                }
            }
        }
    }

    public void SetGameMode(int gameMode)
    {
        _AudioSource.Stop();
        switch (gameMode)
        {
            case 1: PlayGame(velocityMod); break;
            case 2: PlayGame(memoryMod); break;
            case 3: PlayGame(challengeMod); break;
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
            StartCoroutine(InitMod(gameMode));
        }
    }

    private IEnumerator InitMod(IGameMode gameMode)
    {
        CloseUI?.Invoke();
        
        yield return new WaitForSeconds(1f);
        //menuMod.SetActive(false);
        _EnableMenu = false;
        _particleSystemMenu.Stop();
        //countMatchPlaying++;
        this.gameMode = gameMode;
        gameMode.IGameMode(this, _MixColor);
        gameMode.NewRound();
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
        _AudioSource.Play();
        menuMod.SetActive(true);
        _EnableMenu = true;
        _particleSystemMenu.Play();
        _ButtonsManager.ResetDefaultButtons();

        interstitialAdExample.LoadAd();
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator FinishMatchChallenge(float score)
    {
        CloseUI?.Invoke();
        OnStartGameOrFinishMode?.Invoke();
        if(score > HighScore)
        {
            yield return NewHighScore(score);
            //UpdateLeaderboard
        }
        //_UiManagement.EnabledUI(false);
        //ResetAll
        yield return new WaitForSeconds(1.5f);
        _AudioSource.Play();
        menuMod.SetActive(true);
        _EnableMenu = true;
        _particleSystemMenu.Play();
        _ButtonsManager.ResetDefaultButtons();
        yield return null;
    }

    public IEnumerator NewHighScore(float newScore)
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
        // Calcula las coordenadas en el mundo para los bordes izquierdo y derecho de la pantalla
        Vector3 leftEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, _mainCamera.nearClipPlane));
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, _mainCamera.nearClipPlane));

        switch (playerID)
        {
            case PlayerID.Player2:
                _particleSystemWin.gameObject.transform.position = leftEdge;
                _particleSystemWin.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case PlayerID.Player1:
                _particleSystemWin.gameObject.transform.position = rightEdge;
                _particleSystemWin.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
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
