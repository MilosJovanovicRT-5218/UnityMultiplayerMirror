using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkRoomPlayerLoby : NetworkBehaviour
{

    //[Scene] [SerializeField] private string menuScene = string.Empty;

    //[Header("Room")]
    //[SerializeField]
    //private NetworkRoomPlayerLoby roomPayerPRefab = null;

    //public static event Action OnClientConnected;
    //public static event Action OnClientDisConnected;

    //public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    //public override void OnStopClient()
    //{

    //    var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

    //    foreach (var prefab in spawnablePrefabs)
    //    {
    //        .RegisterPrefab(prefab);
    //    }

    //}

    //public override void OnStartClient(NetworkConnection conn)
    //{
    //    base.OnStartClient(conn);
    //    OnStartClient?.Invoke();
    //}

}
