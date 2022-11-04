using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;
    //private Camera mainCamera;

    //Serverski deo
    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {

        Targetable target = targeter.GetTarget();

        if (target != null)
        {

            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                //chase
                agent.SetDestination(target.transform.position);//podesava destinaciju rute 
            }

            else if (agent.hasPath)
            {
                agent.ResetPath();
            }

            return;

        }

        if (!agent.hasPath)
        {
            return;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }

        agent.ResetPath();

    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
        //targeter.ClearTarget();

        //if (!NavMesh.SamplePosition(position, out NavMeshHit hit,1f,NavMesh.AllAreas)) { return; }

        //agent.SetDestination(hit.position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {


        targeter.ClearTarget();//brise target

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);//podesava destinaciju navMesha

    }

    [Server]
    private void ServerHandleGameOver()
    {

        agent.ResetPath();//resetuje putanju

    }

    #endregion

    //#region Client

    //public override void OnStartAuthority()
    //{
    //    mainCamera = Camera.main;
    //}

    //[ClientCallback]
    //private void Update()
    //{
    //    if (!hasAuthority)
    //    {
    //        return;
    //    }

    //    if (!Input.GetMouseButtonDown(1))
    //    {
    //        return;
    //    }

    //    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

    //    if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
    //    {
    //        return;
    //    }

    //    CmdMove(hit.point);
    //}

    //#endregion
}
