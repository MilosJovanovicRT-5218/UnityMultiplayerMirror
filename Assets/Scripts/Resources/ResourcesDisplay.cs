using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (player != null)
            {
                ClientHandleResourcesUpdate(player.GetResources());

                player.ClientOnResourcesUpdated += ClientHandleResourcesUpdate;
            }  
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdate;
    }

    private void ClientHandleResourcesUpdate(int resources)
    {

        resourcesText.text = $"Resources: {resources}";

    }

}
