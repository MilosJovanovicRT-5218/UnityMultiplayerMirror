using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;//max broj unit-a po kocki
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;//vreme potrebno da izadje tenk,kad istekne vreme pojavi se tenk

    [SyncVar(hook = nameof(ClientHandleQueueUntsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;//timer 

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }
        if (isClient)
        {
            UpdateTimerDislpay();
        }
    }

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleOnDie;
    }

    [Server]
    private void ProduceUnits()
    {

        if (queuedUnits == 0)
        {
            return;
        }

        unitTimer += Time.deltaTime;//tajmer pocinje

        if (unitTimer < unitSpawnDuration)
        {
            return;
        }
        GameObject unitInstance = Instantiate(
       unitPrefab.gameObject,
       unitSpawnPoint.position,
       unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        PlayerMovement unitMovement = unitInstance.GetComponent<PlayerMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;//reetuje timer na 0
    }

    [Server]
    private void ServerHandleOnDie()
    {

        NetworkServer.Destroy(gameObject);//unisti objekat 

    }

    [Command]
    private void CmdSpawnUnit()//sponovanje unit-a(tenka)
    {
        if (queuedUnits == maxUnitQueue)//ako je maksimalan broj unit = 5 onda return 
        {
            return;
        }

        //provera da li igrac ima pare 
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < unitPrefab.GetResourceCost())//ako je resurs manje od koliko kosta unit 
        {
            return;
        }

        queuedUnits++;

        //uzima novac od igraca
        player.SetResouces(player.GetResources() - unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDislpay()//abdjetuje vreme na ekranau ,donsono img
    {

        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)//popuni
        {
            unitProgressImage.fillAmount = newProgress; 
        }

        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
                );
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)//kada je pritisnut levi klik
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }

        CmdSpawnUnit();//sponuj tenk

    }

    private void ClientHandleQueueUntsUpdated(int oldUnits, int newUnits)
    {

        remainingUnitsText.text = newUnits.ToString();

    }

    #endregion
}
