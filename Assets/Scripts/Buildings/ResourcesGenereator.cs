using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ResourcesGenereator :NetworkBehaviour
{

    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {

        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;

    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            //add mony for the Player 
            player.SetResouces(player.GetResources() + resourcesPerInterval);
            //Reset timer
            timer += interval;
        }

    }

    private void ServerHandleDie()
    {

        NetworkServer.Destroy(gameObject);

    }

    private void ServerHandleGameOver()
    {

        enabled = false;

    }

}
