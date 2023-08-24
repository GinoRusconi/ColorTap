using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode
{
    GameManagement gameManagement { set; }
    MixColor mixColor { set; }
    public void IGameMode(GameManagement gameManagement, MixColor mixColor);

    public void NewRound();
    public void CheckConditionWin(ButtonController buttonController);
    public IEnumerator PlayerWinRound(PlayerID playerID);
    public void PlayerWinGame(PlayerID playerID);
}
