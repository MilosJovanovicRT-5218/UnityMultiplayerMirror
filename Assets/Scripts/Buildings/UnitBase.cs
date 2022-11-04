using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class UnitBase : NetworkBehaviour
{

    [SerializeField] private Health health = null;

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDeSpawned;
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHanleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHanleDie;

        ServerOnBaseDeSpawned?.Invoke(this);
    }

    [Server]
    private void ServerHanleDie()
    {

        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject); //unisti objekat

    }

    #endregion

}
