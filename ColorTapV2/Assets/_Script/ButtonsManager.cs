using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    private GameManagement gameManagement;
    public ParticleSystem particlesButtonPress;
    public ButtonController[] player1Buttons;
    public ButtonController[] player2Buttons;
    
    private bool isButtonPressedStopped;
    private Queue<ButtonController> ButtonsPresseds;

    public Sprite winSprite;
    public Sprite loseSprite;
    public float TimeBetweenCleanings = 0.5f;

    private void Awake()
    {
       
    }
    void Start()
    {
        gameManagement = GameManagement.Instance;
        gameManagement.WinPlayerRound += DesactivateAllButtons;


        ButtonsPresseds = new Queue<ButtonController>();

        winSprite = gameManagement.winSprite;
        loseSprite = gameManagement.loseSprite;

        for (int indexButton = 0; indexButton < player2Buttons.Length; indexButton++)
        {
            ButtonController buttonController1 = player1Buttons[indexButton];
            player1Buttons[indexButton].info.button.onClick.AddListener(() => PressButton(buttonController1, indexButton));
            player1Buttons[indexButton].particleSystemPress = this.particlesButtonPress;
            buttonController1._ButtonManager = this;

            ButtonController buttonController2 = player2Buttons[indexButton];
            player2Buttons[indexButton].info.button.onClick.AddListener(() => PressButton(buttonController2, indexButton));
            player2Buttons[indexButton].particleSystemPress = this.particlesButtonPress;
            buttonController2._ButtonManager = this;
        }

        ActivateORDeactivateButtonsInteraction(false);
        ChangeTransparencyAllButtons(0);
    }

    private void PressButton(ButtonController buttonController, int indexButton)
    {
        if (isButtonPressedStopped) return;
        gameManagement.gameMode.CheckConditionWin(buttonController);

        ButtonsPresseds.Enqueue(buttonController);
    }

    public void ResetDefaultButtons()
    {
        ButtonsPresseds.Clear();
        for (int indexButton = 0; indexButton < player1Buttons.Length; indexButton++)
        {
            player1Buttons[indexButton].ResetDefault();
            player2Buttons[indexButton].ResetDefault();
        }
    }
    #region TransparencyButtons
    public void ChangeTransparencyAllButtons(PlayerID playerID, float transparence)
    {
        switch (playerID)
        {
            case PlayerID.Player1:
                foreach (ButtonController button in player1Buttons)
                {
                    button.ChangeTransparency(transparence);
                }
                break;
            case PlayerID.Player2:
                foreach (ButtonController button in player2Buttons)
                {
                    button.ChangeTransparency(transparence);
                }
                break;
        }
    }

    public void ChangeTransparencyAllButtons(float transparence)
    {
        for (int indexButton = 0; indexButton < player1Buttons.Length; indexButton++)
        {
            player1Buttons[indexButton].ChangeTransparency(transparence);
            player2Buttons[indexButton].ChangeTransparency(transparence);
        }
    }

    public void ChangeTransparencyAButtons(PlayerID playerID, int IndexButton, float transparenci, bool isPlaySoundButton)
    {
        
        switch (playerID)
        {
            case PlayerID.Player1:
                player1Buttons[IndexButton].ChangeTransparency(transparenci);
                if (isPlaySoundButton) player1Buttons[IndexButton].PlaySoundPress();
                break;
            case PlayerID.Player2:
                player2Buttons[IndexButton].ChangeTransparency(transparenci);
                if (isPlaySoundButton) player2Buttons[IndexButton].PlaySoundPress();
                break;
        }
    }
    #endregion TransparencyButtons

    #region InteractionButton
    public void ActivateORDeactivateButtonInteraction(ButtonController buttonController, bool condition)
    {
        buttonController.info.button.interactable = condition;
        if (!condition) buttonController.ChangeTransparency(100);
    }

    public void ActivateORDeactivateButtonsInteraction(PlayerID playerID, bool condition)
    {
        switch (playerID)
        {
            case PlayerID.Player1:
                foreach (var button in player1Buttons)
                {
                    button.info.button.interactable = condition;
                    button.ChangeTransparency(100);
                }
                break;
            case PlayerID.Player2:
                foreach (var button in player2Buttons)
                {
                    button.info.button.interactable = condition;
                    button.ChangeTransparency(100);
                }
                break;
        }
    }
    public void ActivateORDeactivateButtonsInteraction(bool condition)
    {
        for (int indexButton = 0; indexButton < player1Buttons.Length; indexButton++)
        {
            
            player1Buttons[indexButton].info.button.interactable = condition;
            player2Buttons[indexButton].info.button.interactable = condition;

            if (!condition)
            {
                player1Buttons[indexButton].ChangeTransparency(100);
                player2Buttons[indexButton].ChangeTransparency(100);
            }
        }
    }


    public void ActivateORDeactivateButtons(bool condition)
    {
        for (int indexButton = 0; indexButton < player1Buttons.Length; indexButton++)
        {
            player1Buttons[indexButton].gameObject.SetActive(condition);
            player2Buttons[indexButton].gameObject.SetActive(condition);
        }
    }
# endregion InteractionButton
    public void ShowGuiButton(ButtonController buttonController, bool isCorrect)
    {
        if(isCorrect) buttonController.ShowResultToPressedButton(true, winSprite);
        else buttonController.ShowResultToPressedButton(false, loseSprite);
    }

    private void DesactivateAllButtons()
    {
        isButtonPressedStopped = false;
        ActivateORDeactivateButtonsInteraction(false);
    }

    public IEnumerator ClearButtons()
    {
        int countLastOne = 0;
        foreach (var button in ButtonsPresseds)
        {
            countLastOne++;
            if (countLastOne != ButtonsPresseds.Count)
            {
                StartCoroutine(button.ClearSlotWinLose());
                yield return new WaitForSeconds(TimeBetweenCleanings);
            }else
            {
                yield return StartCoroutine(button.ClearSlotWinLose());
                ChangeTransparencyAllButtons(0);
            }
        }
       
    }
}


