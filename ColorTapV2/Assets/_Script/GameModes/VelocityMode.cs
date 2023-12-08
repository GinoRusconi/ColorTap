using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMode : MonoBehaviour, IGameMode
{
    public GameManagement gameManagement { get; set; }
    public MixColor mixColor { get; set; }
    public string textTutorial;
    public bool isShowingTutorial = false;
    private int RandomColorSelect;

    private int scoreP1;
    private int scoreP2;
    private readonly int scoreWin = 2;
    public void IGameMode(GameManagement gameManagement, MixColor mixColor)
    {
        this.gameManagement = gameManagement;
        this.mixColor = mixColor;
        gameManagement.OnStartGameOrFinishMode?.Invoke();
    }
    public void NewRound()
    {
        gameManagement._ButtonsManager.ResetDefaultButtons();
        StartCoroutine(PlayRound());
    }

    public IEnumerator PlayRound()
    {

        if(isShowingTutorial == false){
            yield return gameManagement.ShowTutorial(textTutorial);  
            isShowingTutorial = true;
        } 

        yield return StartCoroutine(mixColor.Mixed());
        RandomColorSelect = mixColor.randomColorID;

        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(true);

    }
    public void CheckConditionWin(ButtonController buttonController)
    {
        if (buttonController.info.colorID == RandomColorSelect)
        {
            gameManagement._ButtonsManager.ShowGuiButton(buttonController, true);
            gameManagement.WinPlayerRound?.Invoke();
            Debug.Log("Actualizacion de Gui");
            StartCoroutine(PlayerWinRound(buttonController.info.playerID));
        }else
        {
            gameManagement._ButtonsManager.ActivateORDeactivateButtonInteraction(buttonController, false);
            gameManagement._ButtonsManager.ShowGuiButton(buttonController, false);
        }
    }

    public IEnumerator PlayerWinRound(PlayerID playerID)
    {

        yield return new WaitForSeconds(1f);
        Debug.Log("lanzamiento de limpieza");
        yield return StartCoroutine(gameManagement._ButtonsManager.ClearButtons());

        switch (playerID)
        {
            case PlayerID.Player1:
                scoreP1++;
                break;
            case PlayerID.Player2:
                scoreP2++;
                break;
        }

        gameManagement._UiManagement.UpdateScore(playerID);
        yield return new WaitForSeconds(1f);
        CheckSomebodyWin();
    }

    private void CheckSomebodyWin()
    {
        bool p1ExceededGoal = scoreP1 >= scoreWin;
        bool p2ExceededGoal = scoreP2 >= scoreWin;

        if (p1ExceededGoal || p2ExceededGoal)
        {
            if (p1ExceededGoal && !p2ExceededGoal)
            {
                //gano p1
                PlayerWinGame(PlayerID.Player1);
            }
            else if (!p1ExceededGoal && p2ExceededGoal)
            {
                //gano p2
                PlayerWinGame(PlayerID.Player2);
            }
            else if (p1ExceededGoal && p2ExceededGoal)
            {
                if (scoreP1 == scoreP2)
                {
                    //Desempate
                    NewRound();
                }
                else if (scoreP1 > scoreP2)
                {
                    //gano p1
                    PlayerWinGame(PlayerID.Player1);
                }
                else
                {
                    //gano p2
                    PlayerWinGame(PlayerID.Player2);
                }
            }
        }
        else
        {
            NewRound();
        }
    }
    public void PlayerWinGame(PlayerID playerID)
    {
        isShowingTutorial = false;
        StartCoroutine(gameManagement.FinishMatchTwoPlayer(playerID));
        scoreP1 = 0;
        scoreP2 = 0;
    }
}
