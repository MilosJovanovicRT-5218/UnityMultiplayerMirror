using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;//pocetni resursi(novac)

    public event Action<int> ClientOnResourcesUpdated;

    private Color teamColor = new Color();//boja tima
    private List<Unit> myUnit = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    public Transform GetCameraTransform()
    {

        return cameraTransform;

    }

    public Color GetTeamColor()
    {

        return teamColor;

    }

    public int GetResources()
    {

        return resources;

    }

    public List<Unit> GetMyUnits()
    {

        return myUnit;

    }

    public List<Building> GetMyBuildings()
    {

        return myBuildings;

    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)//da li moze da stavi kocku
    {

        if (Physics.CheckBox(
           point + buildingCollider.center,
           buildingCollider.size / 2,
           Quaternion.identity,
           buildingBlockLayer))
        {
            return false;
        }

        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;

    }

    //Serverski deo
    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandlerSpawned;
        Unit.ServerOnUnitDeSpawned += ServerHandlerDeSpawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDeSpawned += ServerHandleBuildingDeSpawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandlerSpawned;
        Unit.ServerOnUnitDeSpawned -= ServerHandlerDeSpawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDeSpawned -= ServerHandleBuildingDeSpawned;
    }

    private void ServerHandlerSpawned(Unit unit)
    {

        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnit.Add(unit);

    }

    private void ServerHandlerDeSpawned(Unit unit)
    {

        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnit.Add(unit);

    }
    /// <summary>
    /// 
    /// </summary>
    private void ServerHandleBuildingSpawned(Building building)
    {

        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myBuildings.Add(building);

    }

    private void ServerHandleBuildingDeSpawned(Building building)
    {

        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myBuildings.Add(building);

    }
    #endregion

    //Klijentski deo
    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDeSpawned += AuthorityHandlerUnitDeSpawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandlerBuildingSpawned;
        Building.AuthorityOnBuildingDeSpawned += AuthorityHandlerBuildingDeSpawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {

        ClientOnResourcesUpdated?.Invoke(newResources);

    }

    public override void OnStopClient() 
    {
        if (!isClient || !hasAuthority)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDeSpawned -= AuthorityHandlerUnitDeSpawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandlerBuildingSpawned;
        Building.AuthorityOnBuildingDeSpawned -= AuthorityHandlerBuildingDeSpawned;
    }

    [Server]
    public void SetTeamColor(Color newTemaColor)
    {

        teamColor = newTemaColor;

    }

    [Server]
    public void SetResouces(int newResources)
    {

        resources = newResources;

    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId,Vector3 point)//dva parametra zato sto saljemo samo id ne ceo objekat
    {

        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

       

        if (!CanPlaceBuilding(buildingCollider, point))
        {
            return;
        }

        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResouces(resources - buildingToPlace.GetPrice());
    }

    private void AuthorityHandlerUnitSpawned(Unit unit)
    {

        myUnit.Add(unit);

    }

    private void AuthorityHandlerUnitDeSpawned(Unit unit)
    {

        myUnit.Add(unit);

    }

    private void AuthorityHandlerBuildingSpawned(Building building)
    {

        myBuildings.Add(building);

    }

    private void AuthorityHandlerBuildingDeSpawned(Building building)
    {

        myBuildings.Add(building);

    }

    #endregion
}
