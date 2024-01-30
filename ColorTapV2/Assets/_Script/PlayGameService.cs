using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SocialPlatforms;
using System.Globalization;

public class PlayGameService : MonoBehaviour
{
    public Button _buttonSingInPlayGameService;
    public Button _buttonLeaderBoard;
    public GameObject recomendation;
    private Button recomendationButton;
    private TMP_Text recomendationTxt;
    private bool _isHide;
    private long _highScore;

    //public TMP_Text prueba;

    private void Awake() {
        recomendationTxt = recomendation.GetComponentInChildren<TMP_Text>();
        recomendationButton = recomendation.GetComponent<Button>();
    }

    public void Start() {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        PlayGamesPlatform.Instance.SetDefaultLeaderboardForUI("CgkI-fHlps0YEAIQAQ");
    }

    internal void ProcessAuthentication(SignInStatus status) {
      if (status == SignInStatus.Success) {
        // Continue with Play Games Services
        _buttonLeaderBoard.interactable = true;
        _buttonSingInPlayGameService.interactable = false;
        //if(recomendation.activeSelf)recomendation.SetActive(false);
        LoadScoreLeaderboard();
        
      } else {
        recomendation.SetActive(true);
        // Disable your integration with Play Games Services or show a login button
        // to ask users to sign-in. Clicking it should call
        // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
      }
    }
    /*
    public void LoadScoreLeaderboard(){
        PlayGamesPlatform.Instance.LoadScores("CgkI-fHlps0YEAIQAQ",LeaderboardStart.PlayerCentered, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (data) =>
        {
            if(data.Valid && data.Scores.Length > 0){
                _highScore = data.Scores[0].value;
                testHighScore.text = _highScore.ToString();
                excepctions.text = "Load puntaje";
            }else{
                excepctions.text = "No load";
            }
        });
    }*/
    
    public void LoadScoreLeaderboard(){
         // Crea un objeto Leaderboard
        ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();

        // Establece el ID del Leaderboard que deseas obtener
        leaderboard.id = "CgkI-fHlps0YEAIQAQ"; // Reemplaza con el ID de tu Leaderboard

        // Carga los datos del Leaderboard
        leaderboard.LoadScores(success =>
        {
            if (success)
            {
                float showLoadScore;
                // Obtiene el score más alto del jugador actual
                IScore userScore = leaderboard.localUserScore;
                //prueba.text = $"{userScore.value}";
                _highScore = userScore.value;
                showLoadScore = (float)_highScore;
                showLoadScore /= 100;
                recomendationTxt.text = $"HIGHSCORE\n{showLoadScore.ToString("F2", CultureInfo.InvariantCulture)}";
            }
            else
            {
                recomendationTxt.text = ("Error al cargar el leaderboard.");
            }
        });

    }

    public void HideUI(){
        
        if (_isHide)
        {
            _buttonSingInPlayGameService.gameObject.SetActive(true);
            _buttonLeaderBoard.gameObject.SetActive(true);
            //if(!PlayGamesPlatform.Instance.IsAuthenticated())
            recomendation.SetActive(true);
        }else{
            _buttonSingInPlayGameService.gameObject.SetActive(false);
            _buttonLeaderBoard.gameObject.SetActive(false);
            recomendation.SetActive(false);
        }
        _isHide = !_isHide;
    }

    // Método para manejar el clic en el botón de inicio de sesión manual
    public void OnManualLoginButtonClick() {
        // Intenta la autenticación manualmente
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    // Método para manejar el clic en el botón de "No iniciar sesión"
    public void OnNoLoginButtonClick() {
        // Aquí puedes implementar la lógica correspondiente
        // cuando el usuario decide no iniciar sesión manualmente.
        
        // Por ejemplo, puedes deshabilitar la integración con Play Games Services.
        DisablePlayGamesIntegration();
    }
    
    public bool UpgradeLeaderBoard(float score) {
        bool isUpdate = false;
        long scoreRound = (long)(score * 100f);
       
        if(_highScore < scoreRound)
        {
            UpdateScore(score, scoreRound);
            _highScore = scoreRound;
            return isUpdate = true;
        }
        else
        {
            return isUpdate;
        };
    }

    private void UpdateScore(float score, long scoreRound)
    {
        if(WithOutInternet()){
            recomendationButton.onClick.RemoveAllListeners(); 
            recomendationTxt.text = $"HIGHSCORE\n{string.Format("{0:F2}", score)}";
            try
            {
                PlayGamesPlatform.Instance.ReportScore(scoreRound, "CgkI-fHlps0YEAIQAQ", (bool Success) =>
                {
                    
                });
            }
            catch (Exception e)
            {
                recomendationTxt.text = $"Error al actualizar la puntuación: {e.Message}";
            }
            
        }else{
            recomendationTxt.text = "Not Internet, Press To Update";
            recomendationButton.onClick.AddListener(() => UpdateScore(score,scoreRound)); 
        }
    }

    public bool WithOutInternet(){
        if(Application.internetReachability != NetworkReachability.NotReachable){
            return true;
        }else{
            return false;
        }
    }

    public void ShowLeaderBoard(){

        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    // Método para deshabilitar la integración con Play Games Services
    private void DisablePlayGamesIntegration() {
        // Implementa la lógica para deshabilitar la integración con Play Games Services
    }
    public bool UpgradeLeaderBoard(long score, Action<bool> callback)
    {
        bool isUpdate = false;

        if (_highScore < score)
        {
                try
                {
                    PlayGamesPlatform.Instance.ReportScore(score, "CgkI-fHlps0YEAIQAQ", success =>
                    {
                        if (success)
                        {
                            callback?.Invoke(success);
                        }

                        _highScore = score;
                        
                        callback(isUpdate);

                    });
                }
                catch (Exception e)
                {
                    // Manejar la excepción si ocurre alguna
                    recomendation.SetActive(true);
                    recomendation.GetComponent<TMP_Text>().text = $"Error al actualizar la puntuación: {e.Message}";
                    callback(false);
                }
                return isUpdate = true;
            }
            else
            {
                
                callback(false);
            }
            return isUpdate;
    }
}
