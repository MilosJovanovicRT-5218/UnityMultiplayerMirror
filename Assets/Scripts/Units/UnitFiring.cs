using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitFiring : NetworkBehaviour
{

    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectionPrefab = null;
    [SerializeField] private Transform projectilSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 53f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()//proveravamo svake sekunde range targeta ...
    {

        Targetable target = targeter.GetTarget();

        if (target == null)//ako nema targeta ne pucaj
        {
            return;
        }

        if (!CanFireAtTarget())
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);//koliko stepena moze maksimalno da se okrece 


        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            //we can now fire
             
            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimPoint().position - projectilSpawnPoint.position);//gledaj u rotaciju

            GameObject projectileInstance = Instantiate(projectionPrefab,projectilSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);//sponovanje bullet-a

            lastFireTime = Time.time;

        }

    }

    private bool CanFireAtTarget()//moze da puca u metu
    {

        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;//puca u metu kad je u dogovarajcuem range-u(opsegu)

    }

}
