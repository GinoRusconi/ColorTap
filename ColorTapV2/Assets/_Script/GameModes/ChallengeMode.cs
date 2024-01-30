using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMode : MonoBehaviour , IGameMode
{
    public GameManagement gameManagement { get; set; }
    public MixColor mixColor { get; set; }
    public List<int> MemoryColorsID;
    public string textTutorial;
    private float _score;
    private int _countColorPress;
    public int timeInitial;
    private float _timeLeft;
    private bool timeCountOff;
    private float _timeDelay = 1f;
    public AnimationCurve delayCurveShowingButton;
    private int _life;

    public void IGameMode(GameManagement gameManagement, MixColor mixColor)
    {
        MemoryColorsID = new List<int>();
        this.gameManagement = gameManagement;
        this.mixColor = mixColor;
        _life = 1;
    }

    public void NewRound()
    {
       //Start Game Mode
       _countColorPress = -1;
       timeCountOff= false;
       MemoryColorsID.Clear();
       
       StartCoroutine(FirstTurn());
    }

    public void CheckConditionWin(ButtonController buttonController)
    {
        // Chequear si gano o si perdio.
        _countColorPress++;

        if (buttonController.info.colorID == MemoryColorsID[_countColorPress])
        {
            _score += _timeLeft;
            float scoreRound = (float)Math.Round(_score,2);
            gameManagement._UiManagement.UpdateScore(scoreRound);
            Debug.Log(scoreRound);
            if (MemoryColorsID.Count == (_countColorPress + 1))
            {
                //StartNewRound
                //StopCoroutine(TimeCounter());                
                StopAllCoroutines();
                StartCoroutine(PlayerWinRound(PlayerID.Player1));
            }
            _timeLeft = timeInitial;
            _timeDelay = 1f;
            gameManagement._UiManagement.UpdateTimerUI(_timeLeft);
        }
        else
        {
            //Player Lose
            StopAllCoroutines();
            StartCoroutine(Defeat());
        }
    }

    public IEnumerator PlayerWinRound(PlayerID playerID)
    {
        //Gano la ronda

        ResetRound(playerID);
        yield return null;
        AddColorIDToList();

    }

    private void ResetRound(PlayerID playerID)
    {
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(playerID, false);
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(playerID, 20);
        _countColorPress = -1;
    }

    //Termina el juego
    //P1 reinicia la ronnda new life, P2 termino el juego
    public void PlayerWinGame(PlayerID playerID)
    {
        //Actualiza el LeaderBoard
        //NuevaVida
        if(PlayerID.Player1 == playerID)
        {
            ResetRound(playerID);
            StartCoroutine(ShowButtonsOrder());

        }else
        {
            StopAllCoroutines();
            StartCoroutine(gameManagement.FinishMatchChallenge(_score));
            _score = 0;
        }
    }

    public void AddColorIDToList()
    {
        int ColorIDRandom = mixColor.GetRandomColor();
        MemoryColorsID.Add(ColorIDRandom);
        StartCoroutine(ShowButtonsOrder());
    }

    private IEnumerator Defeat()
    {
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(PlayerID.Player1, false);
        yield return StartCoroutine(gameManagement._UiManagement.TextPlayer(PlayerID.Player1, "Defeat"));
            if(_life > 0)
            {
                gameManagement.NewLifeAds(true);
                _life--;
            }else
            {
                PlayerWinGame(PlayerID.Player2);
            }
    }

    private IEnumerator TimeCounter()
    {
        while (_timeLeft > 0)
        {
            gameManagement._UiManagement.UpdateTimerUI(_timeLeft);
            while(_timeDelay > 0)
            {
                _timeDelay -= Time.deltaTime;
                yield return null;
            }
            _timeLeft -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Defeat());
        _timeLeft = 0f;
        gameManagement._UiManagement.UpdateTimerUI(_timeLeft);
        timeCountOff = true;
        //gameManagement.NewLifeAds(true);
    }

    //Animation effect

    public IEnumerator FirstTurn()
    {
        Debug.Log($"Start Game");
        yield return gameManagement.ShowTutorial(textTutorial);
        gameManagement._UiManagement.EnabledUI(true);
        //gameManagement.animatorPlayer1.SetTrigger("StartGame");
        gameManagement.OnStartGameOrFinishMode?.Invoke();
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(PlayerID.Player1 ,false);
        yield return new WaitForSeconds(2f);
        AddColorIDToList();
    }

    public IEnumerator ShowButtonsOrder()
    {
        yield return StartCoroutine(gameManagement._UiManagement.TextPlayer(PlayerID.Player1, "Next Turn"));
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(PlayerID.Player1, 20);
        float timedelay = delayCurveShowingButton.Evaluate(MemoryColorsID.Count);
        Debug.Log(timedelay);
        foreach (var button in MemoryColorsID)
        {
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(PlayerID.Player1, button, 255, true);
            mixColor.ChangeColorMainCamera(button); 
            yield return new WaitForSecondsRealtime(timedelay);
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(PlayerID.Player1, button, 20, false);
            yield return new WaitForSecondsRealtime(0.05f);
        }
        yield return new WaitForSecondsRealtime(0.25f);
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(PlayerID.Player1, 255);
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(PlayerID.Player1, true);
       
        _timeLeft = timeInitial;
        StartCoroutine(TimeCounter());
    }
}
