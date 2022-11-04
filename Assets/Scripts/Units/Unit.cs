using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;
public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private PlayerMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDeSpawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDeSpawned;

    public int GetResourceCost()
    {

        return resourceCost;//vrati cenu 

    }

    public Targeter GetTargetter()
    {
        return targeter;//vrati target
    }

    public PlayerMovement GetPlayerMovement()
    {
        return unitMovement;//vrati movement
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);//? ako je nula ne radi nista

        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDeSpawned?.Invoke(this);//? ako je nula ne radi nista

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {

        NetworkServer.Destroy(gameObject);//unisti objekat

    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);//? ako je nula ne radi nista
    }

    public override void OnStopClient()
    {

        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        AuthorityOnUnitDeSpawned?.Invoke(this);//? ako je nula ne radi nista
    }

    [Client]
    public void Select()//selektuje unit(tenk)
    {

        if (!hasAuthority)
        {
            return;
        }

        onSelected?.Invoke();//? ako je nula ne radi nista

    }

    [Client]
    public void DeSelect()//de sekejtyhe unit(tenk)
    {

        if (!hasAuthority)
        {
            return;
        }

        onDeSelected?.Invoke();//? ako je nula ne radi nista

    }

    #endregion

}
