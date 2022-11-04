using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
public class GameOverDisplay : MonoBehaviour
{

    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;

    private void Start()
    {
        GameOverHandler.ClientGameOver += ClientHandleGameOver; 
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            //stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //stop client
            NetworkManager.singleton.StopClient();
        }

    }

    private void ClientHandleGameOver(string winner)
    {

        winnerNameText.text = $"{winner} Has Won!";

        gameOverDisplayParent.SetActive(true);

    }

}
