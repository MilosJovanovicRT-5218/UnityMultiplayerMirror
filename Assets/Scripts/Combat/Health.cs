using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class Health : NetworkBehaviour
{

    [SerializeField] private int maxHealth = 100;//max health 

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action<int, int> CLientOnHealhtUpdate; 

    public event Action ServerOnDie;

    //Serverski deo
    #region Server

    public override void OnStartServer()
    {

        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;

    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {

        if (connectionToClient.connectionId != connectionId)
        {
            return;
        }

        DealDemage(currentHealth);//prima demage

    }

    [Server]
    public void DealDemage(int damageAmount)//prima demage
    {

        if (currentHealth == 0)//ako je health 0
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        //currentHealth -= damageAmount;


        //if (currentHealth < 0)
        //{
        //    currentHealth = 0;
        //}

        if (currentHealth != 0)
        {
            return;
        }
        ServerOnDie?.Invoke();

        Debug.Log("We Died");//Obavestenje da je mrtav
    }
    #endregion

    //Klijentski deo
    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)//update-uje health
    {

        CLientOnHealhtUpdate?.Invoke(newHealth ,  maxHealth);

    }

    #endregion

}
