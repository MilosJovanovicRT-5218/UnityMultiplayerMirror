using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitProjectile : NetworkBehaviour
{

    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int demageToDeal = 1;//demage
    [SerializeField] private float destroyAfterSeconds = 5f;//unisti bullet posle 5 sekundi
    [SerializeField] private float launchForce = 10f;//sila izbacivanja metka

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * launchForce;   
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestrySelf), destroyAfterSeconds);
    }

    [ServerCallback]//jer se ne zove na klijentu ,Proveri
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient)
            {
                return;
            }
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
            //deal demage
            health.DealDemage(demageToDeal);
        }

        DestrySelf();//unisti bullet

    }


    [Server]
    private void DestrySelf()//zelimo ovo da pozovemo posle 5 sekundi
    {

        NetworkServer.Destroy(gameObject);//unisti objekat odnosno bullet

    }
}
