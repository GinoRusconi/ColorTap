using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryMode : MonoBehaviour, IGameMode
{
    public GameManagement gameManagement { get; set; }
    public MixColor mixColor { get; set; }

    public string textTutorial;

    private List<int> MemoryColorsID;
    private int countColorPress;
    private PlayerID playerTurn;


    public void IGameMode(GameManagement gameManagement, MixColor mixColor)
    {
        MemoryColorsID = new List<int>();
        this.gameManagement = gameManagement;
        this.mixColor = mixColor;
        playerTurn = PlayerID.Player1;
    }

    public void NewRound()
    {
        countColorPress = -1;
        MemoryColorsID.Clear();
        
        StartCoroutine(FirstTurn());
    }

    public void AddColorIDToList()
    {
        int ColorIDRandom = mixColor.GetRandomColor();
        MemoryColorsID.Add(ColorIDRandom);
        StartCoroutine(ShowButtonsOrder());
    }

    private void SwitchPlayer(PlayerID playerID)
    {
        switch (playerID)
        {
            case PlayerID.Player1:
                playerTurn = PlayerID.Player2;
                break;
            case PlayerID.Player2:
                playerTurn = PlayerID.Player1;
                break;
        }

    }

    public void CheckConditionWin(ButtonController buttonController)
    {
        countColorPress++;

        if (buttonController.info.colorID == MemoryColorsID[countColorPress])
        {
            if (MemoryColorsID.Count == (countColorPress + 1))
            {
                //switchPlayer
                StartCoroutine(PlayerWinRound(playerTurn));
            }

        }
        else
        {
            //loseCurrentPlayer
            switch (playerTurn)
            {
                case PlayerID.Player1:
                    PlayerWinGame(PlayerID.Player2);
                    break;
                case PlayerID.Player2:
                    PlayerWinGame(PlayerID.Player1);
                    break;
            }
        }
    }

    public IEnumerator PlayerWinRound(PlayerID playerID)
    {
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(playerID, false);
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(playerID, 20);
        countColorPress = -1;
        yield return null;
        SwitchPlayer(playerID);
        AddColorIDToList();
    }

    public void PlayerWinGame(PlayerID playerID)
    {
        gameManagement.WinPlayerRound?.Invoke();
        StartCoroutine(gameManagement.FinishMatchTwoPlayer(playerID));
    }

    //Animations

    public IEnumerator FirstTurn()
    {
        Debug.Log($"Start Game");
        yield return gameManagement.ShowTutorial(textTutorial);
        gameManagement.animatorPlayer1.SetTrigger("StartGame");
        gameManagement.animatorPlayer2.SetTrigger("StartGame");
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(false);
        yield return new WaitForSeconds(2f);
        AddColorIDToList();
    }

    public IEnumerator ShowButtonsOrder()
    {
        yield return StartCoroutine(gameManagement._UiManagement.TextPlayer(playerTurn, "Your Turn"));
        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(20);
        foreach (var button in MemoryColorsID)
        {
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(playerTurn, button, 255, true);
            mixColor.ChangeColorMainCamera(button); 
            yield return new WaitForSecondsRealtime(0.5f);
            gameManagement._ButtonsManager.ChangeTransparencyAButtons(playerTurn, button, 20, false);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        gameManagement._ButtonsManager.ChangeTransparencyAllButtons(playerTurn, 255);
        gameManagement._ButtonsManager.ActivateORDeactivateButtonsInteraction(playerTurn, true);

    }
}
