using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class CustomeHUd : MonoBehaviour
{
    NetworkManager manager;
    public InputField ip_InputField;
    public InputField playerName;
    public GameObject HostConnection_go;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void HostFunction()
    {

        manager.StartHost();

        HostConnection_go.SetActive(false);

        saveName = inputText.text;
        PlayerPrefs.SetString("name", saveName);

    }

    public void ConnectionFunction()
    {
        manager.networkAddress = ip_InputField.text;

        manager.StartClient();

        HostConnection_go.SetActive(false);
    }





    public string nameOfPlayer;
    public string saveName;

    public Text inputText;
    public Text loadName;

    public void Update()
    {
        nameOfPlayer = PlayerPrefs.GetString("name", "none");
        loadName.text = nameOfPlayer;
    }

    //public void SetName()
    //{



    //}

}
