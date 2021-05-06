using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInList : MonoBehaviour
{

    public Text _playerNameText;

    private string _playerName;
    private int _actorNumber;

    public void Initialize(string playerName, int actorNumber)
    {
        _playerName = playerName;
        _actorNumber = actorNumber;

        _playerNameText.text = playerName;
    }

    public void setPlayerName(string playerName)
    {
        _playerName = playerName;
    }

    public void setActorNumber(int actorNumber)
    {
        _actorNumber = actorNumber;
    }

    public string getPlayerName()
    {
        return _playerName;
    }

    public int getActorNumber()
    {
        return _actorNumber;
    }
}
