using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMod : MonoBehaviour , IGameMode
{
    public GameManagement gameManagement { get; set; }
    public MixColor mixColor { get; set; }
    private List<int> MemoryColorsID;
    public string textTutorial;
    private float score;
    private int countColorPress;
    public int timeInitial;
    private float timeLeft;
    private bool timeCountOff;
    private float timeDelay = 1f;

    private int life;

    public void IGameMode(GameManagement gameManagement, MixColor mixColor)
    {
        MemoryColorsID = new List<int>();
        this.gameManagement = gameManagement;
        this.mixColor = mixColor;
        life = 1;
    }

    public void NewRound()
    {
       //Start Game Mode
       countColorPress = -1;
       timeCountOff= false;
       MemoryColorsID.Clear();
       
       StartCoroutine(FirstTurn());
    }

    public void CheckConditionWin(ButtonController buttonController)
    {
        // Chequear si gano o si perdio.
        countColorPress++;

        if (buttonController.info.colorID == MemoryColorsID[countColorPress])
        {
            score += timeLeft;
            float scoreRound = (float)Math.Round(score,2);
            gameManagement._UiManagement.UpdateScore(scoreRound);
            Debug.Log(scoreRound);
            if (MemoryColorsID.Count == (countColorPress + 1))
            {
                //StartNewRound
                //StopCoroutine(TimeCounter());                
                StopAllCoroutines();
                StartCoroutine(PlayerWinRound(PlayerID.Player1));
            }
            timeLeft = timeInitial;
            timeDelay = 1f;
            gameManagement._UiManagement.UpdateTimerUI(timeLeft);
        }
        else
        {
            //Player Lose
            if(life > 0)
            {
                StopAllCoroutines();
                gameManagement.NewLifeAds(true);
                life--;
            }else
            {
                PlayerWinGame(PlayerID.Player2);
            }
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
        countColorPress = -1;
    }

    //En este caso el se pierde
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
            score = 0;
            StopAllCoroutines();
            gameManagement.WinPlayerRound?.Invoke();
            gameManagement._UiManagement.EnabledUI(false);
            StartCoroutine(gameManagement.FinishMatchChallenge(score));
            
        }

        
    }

    public void AddColorIDToList()
    {
        int ColorIDRandom = mixColor.GetRandomColor();
        MemoryColorsID.Add(ColorIDRandom);
        StartCoroutine(ShowButtonsOrder());
    }

    private IEnumerator TimeCounter()
    {
        while (timeLeft > 0)
        {
            gameManagement._UiManagement.UpdateTimerUI(timeLeft);
            while(timeDelay > 0)
            {
                timeDelay -= Time.deltaTime;
                yield return null;
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        timeCountOff = true;
        gameManagement.NewLifeAds(true);
    }

    //Animation effect

    public IEnumerator FirstTurn()
    {
        Debug.Log($"Start Game");
        yield return gameManagement.ShowTutorial(textTutorial);
        gameManagement._UiManagement.EnabledUI(true);
        gameManagement.animatorPlayer1.SetTrigger("StartGame");
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(PlayerID.Player1 ,false);
        yield return new WaitForSeconds(2f);
        AddColorIDToList();
    }

    public IEnumerator ShowButtonsOrder()
    {
        yield return StartCoroutine(gameManagement._UiManagement.TextPlayer(PlayerID.Player1, "Next Turn"));
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(PlayerID.Player1, 20);
        foreach (var button in MemoryColorsID)
        {
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(PlayerID.Player1, button, 255, true);
            mixColor.ChangeColorMainCamera(button); 
            yield return new WaitForSecondsRealtime(0.5f);
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(PlayerID.Player1, button, 20, false);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(PlayerID.Player1, 255);
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(PlayerID.Player1, true);
       
        timeLeft = timeInitial;
        StartCoroutine(TimeCounter());
    }
}
