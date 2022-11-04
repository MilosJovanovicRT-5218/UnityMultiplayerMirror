using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{

    public static event Action ServerOnGameOver;

    public static event Action<string> ClientGameOver;

    private List<UnitBase> bases = new List<UnitBase>(); 

    #region Server

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawn;
        UnitBase.ServerOnBaseDeSpawned += ServerHandleBaseDeSpawn;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawn;
        UnitBase.ServerOnBaseDeSpawned -= ServerHandleBaseDeSpawn;
    }

    [Server]
    private void ServerHandleBaseSpawn(UnitBase unitBase)
    {

        bases.Add(unitBase);//dodaj bazu ondnosno kocku

    }

    [Server]
    private void ServerHandleBaseDeSpawn(UnitBase unitBase)
    {

        bases.Remove(unitBase);//ukloni bazu ondnosno kocku

        if (bases.Count != 1)//ako nije jedan return
        {
            return;
        }

        int playerId = bases[0].connectionToClient.connectionId;

        //Debug.Log("Game Over");//Ako je ostao jedan igrac ili manje
        RpcGameOver($"Player {playerId}");//game over + idIgraca 
        ServerOnGameOver?.Invoke();
    }
    #endregion 

    //Klijentski deo
    #region Client

    private void RpcGameOver(string winner)
    {

        ClientGameOver?.Invoke(winner);

    }
    #endregion 
}
