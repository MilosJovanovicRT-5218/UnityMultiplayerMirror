//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Mirror.Examples.Basic
//{
//    public class Player : NetworkBehaviour
//    { }
//}
//{
//    // Events that the PlayerUI will subscribe to
//    public event System.Action<byte> OnPlayerNumberChanged;
//    public event System.Action<Color32> OnPlayerColorChanged;
//    public event System.Action<ushort> OnPlayerDataChanged;

//    // Players List to manage playerNumber
//    static readonly List<Player> playersList = new List<Player>();

//    [Header("Player UI")]
//    public GameObject playerUIPrefab;

//    GameObject playerUIObject;
//    PlayerUI playerUI = null;

//    #region SyncVars

//    [Header("SyncVars")]

//    /// <summary>
//    /// This is appended to the player name text, e.g. "Player 01"
//    /// </summary>
//    [SyncVar(hook = nameof(PlayerNumberChanged))]
//    public byte playerNumber = 0;

//    /// <summary>
//    /// Random color for the playerData text, assigned in OnStartServer
//    /// </summary>
//    [SyncVar(hook = nameof(PlayerColorChanged))]
//    public Color32 playerColor = Color.white;

//    /// <summary>
//    /// This is updated by UpdateData which is called from OnStartServer via InvokeRepeating
//    /// </summary>
//    [SyncVar(hook = nameof(PlayerDataChanged))]
//    public ushort playerData = 0;
//    public static object localPlayer;

//    // This is called by the hook of playerNumber SyncVar above
//    void PlayerNumberChanged(byte _, byte newPlayerNumber)
//    {
//        OnPlayerNumberChanged?.Invoke(newPlayerNumber);
//    }

//    // This is called by the hook of playerColor SyncVar above
//    void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
//    {
//        OnPlayerColorChanged?.Invoke(newPlayerColor);
//    }

//    // This is called by the hook of playerData SyncVar above
//    void PlayerDataChanged(ushort _, ushort newPlayerData)
//    {
//        OnPlayerDataChanged?.Invoke(newPlayerData);
//    }

//    #endregion

//    #region Server

//    /// <summary>
//    /// This is invoked for NetworkBehaviour objects when they become active on the server.
//    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
//    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
//    /// </summary>
//    public override void OnStartServer()
//    {
//        base.OnStartServer();

//        // Add this to the static Players List
//        playersList.Add(this);

//        // set the Player Color SyncVar
//        playerColor = System.Random.ColorHSV(0f, 1f, 0.9f, 0.9f, 1f, 1f);

//        // set the initial player data
//        playerData = (ushort)UnityEngine.Random.Range(100, 1000);

//        // Start generating updates
//        InvokeRepeating(nameof(UpdateData), 1, 1);
//    }

//    // This is called from BasicNetManager OnServerAddPlayer and OnServerDisconnect
//    // Player numbers are reset whenever a player joins / leaves
//    [ServerCallback]
//    internal static void ResetPlayerNumbers()
//    {
//        byte playerNumber = 0;
//        foreach (Player player in playersList)
//            player.playerNumber = playerNumber++;
//    }

//    // This only runs on the server, called from OnStartServer via InvokeRepeating
//    [ServerCallback]
//    void UpdateData()
//    {
//        playerData = (ushort)Random.Range(100, 1000);
//    }

//    /// <summary>
//    /// Invoked on the server when the object is unspawned
//    /// <para>Useful for saving object data in persistent storage</para>
//    /// </summary>
//    public override void OnStopServer()
//    {
//        CancelInvoke();
//        playersList.Remove(this);
//    }

//    #endregion

//    #region Client

//    /// <summary>
//    /// Called on every NetworkBehaviour when it is activated on a client.
//    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
//    /// </summary>
//    public override void OnStartClient()
//    {
//        Debug.Log("OnStartClient");

//        // Instantiate the player UI as child of the Players Panel
//        playerUIObject = Instantiate(playerUIPrefab, CanvasUI.instance.playersPanel);
//        playerUI = playerUIObject.GetComponent<PlayerUI>();

//        // wire up all events to handlers in PlayerUI
//        OnPlayerNumberChanged = playerUI.OnPlayerNumberChanged;
//        OnPlayerColorChanged = playerUI.OnPlayerColorChanged;
//        OnPlayerDataChanged = playerUI.OnPlayerDataChanged;

//        // Invoke all event handlers with the initial data from spawn payload
//        OnPlayerNumberChanged.Invoke(playerNumber);
//        OnPlayerColorChanged.Invoke(playerColor);
//        OnPlayerDataChanged.Invoke(playerData);
//    }

//    /// <summary>
//    /// Called when the local player object has been set up.
//    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
//    /// </summary>
//    public override void OnStartLocalPlayer()
//    {
//        Debug.Log("OnStartLocalPlayer");

//        // Set isLocalPlayer for this Player in UI for background shading
//        playerUI.SetLocalPlayer();

//        // Activate the main panel
//        CanvasUI.instance.mainPanel.gameObject.SetActive(true);
//    }

//    /// <summary>
//    /// Called when the local player object is being stopped.
//    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
//    /// </summary>
//    public override void OnStopLocalPlayer()
//    {
//        // Disable the main panel for local player
//        CanvasUI.instance.mainPanel.gameObject.SetActive(false);
//    }

//    /// <summary>
//    /// This is invoked on clients when the server has caused this object to be destroyed.
//    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
//    /// </summary>
//    public override void OnStopClient()
//    {
//        // disconnect event handlers
//        OnPlayerNumberChanged = null;
//        OnPlayerColorChanged = null;
//        OnPlayerDataChanged = null;

//        // Remove this player's UI object
//        Destroy(playerUIObject);
//    }

//    public void PlayerCountUpdated(int count)
//    {
//        throw new NotImplementedException();
//    }

//    public void StartGame()
//    {
//        throw new NotImplementedException();
//    }

//    #endregion
//}

///Player.txt
//Who has access

//System properties
//Type
//Text
//Size
//7 KB
//Storage used
//7 KB
//Location
//Скрипты
//Owner
//IDK ORG
//Modified
//Oct 28, 2022 by IDK ORG
//Opened
//7:42 PM by me
//Created
//Oct 28, 2022
//No description
//Viewers can download
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Mirror;
//using UnityEngine.SceneManagement;
//using System;
//using System.Text.RegularExpressions;

//public class Player : NetworkBehaviour
//{
//    bool facingRight = true;
//    public static Player localPlayer;
//    public TextMesh NameDisplayText;
//    [SyncVar(hook = "DisplayPlayerName")] public string PlayerDisplayName;
//    [SyncVar] public string matchID;

//    [SyncVar] public Match CurrentMatch;
//    public GameObject PlayerLobbyUI;
//    private Guid netIDGuid;

//    private NetworkMatch networkMatch;

//    private void Awake()
//    {
//        networkMatch = GetComponent<NetworkMatch>();
//    }

//    private void Start()
//    {
//        if (isLocalPlayer)
//        {
//            CmdSendName(MainMenu.instance.DisplayName);
//        }
//    }

//    public override void OnStartServer()
//    {
//        netIDGuid = netId.ToString().ToGuid();
//        networkMatch.matchId = netIDGuid;
//    }

//    public override void OnStartClient()
//    {
//        if (isLocalPlayer)
//        {
//            localPlayer = this;
//        }
//        else
//        {
//            PlayerLobbyUI = MainMenu.instance.SpawnPlayerUIPrefab(this);
//        }
//    }

//    public override void OnStopClient()
//    {
//        ClientDisconnect();
//    }

//    public override void OnStopServer()
//    {
//        ServerDisconnect();
//    }

//    [Command]
//    public void CmdSendName(string name)
//    {
//        PlayerDisplayName = name;
//    }

//    public void DisplayPlayerName(string name, string playerName)
//    {
//        name = PlayerDisplayName;
//        Debug.Log("Имя " + name + " : " + playerName);
//        NameDisplayText.text = playerName;
//    }

//    public void HostGame(bool publicMatch)
//    {
//        string ID = MainMenu.GetRandomID();
//        CmdHostGame(ID, publicMatch);
//    }

//    [Command]
//    public void CmdHostGame(string ID, bool publicMatch)
//    {
//        matchID = ID;
//        if (MainMenu.instance.HostGame(ID, gameObject, publicMatch))
//        {
//            Debug.Log("Лобби было создано успешно");
//            networkMatch.matchId = ID.ToGuid();
//            TargetHostGame(true, ID);
//        }
//        else
//        {
//            Debug.Log("Ошибка в создании лобби");
//            TargetHostGame(false, ID);
//        }
//    }

//    [TargetRpc]
//    void TargetHostGame(bool success, string ID)
//    {
//        matchID = ID;
//        Debug.Log($"ID {matchID} == {ID}");
//        MainMenu.instance.HostSuccess(success, ID);
//    }

//    public void JoinGame(string inputID)
//    {
//        CmdJoinGame(inputID);
//    }

//    [Command]
//    public void CmdJoinGame(string ID)
//    {
//        matchID = ID;
//        if (MainMenu.instance.JoinGame(ID, gameObject))
//        {
//            Debug.Log("Успешное подключение к лобби");
//            networkMatch.matchId = ID.ToGuid();
//            TargetJoinGame(true, ID);
//        }
//        else
//        {
//            Debug.Log("Не удалось подключиться");
//            TargetJoinGame(false, ID);
//        }
//    }

//    [TargetRpc]
//    void TargetJoinGame(bool success, string ID)
//    {
//        matchID = ID;
//        Debug.Log($"ID {matchID} == {ID}");
//        MainMenu.instance.JoinSuccess(success, ID);
//    }

//    public void DisconnectGame()
//    {
//        CmdDisconnectGame();
//    }

//    [Command]
//    void CmdDisconnectGame()
//    {
//        ServerDisconnect();
//    }

//    void ServerDisconnect()
//    {
//        MainMenu.instance.PlayerDisconnected(gameObject, matchID);
//        RpcDisconnectGame();
//        networkMatch.matchId = netIDGuid;
//    }

//    [ClientRpc]
//    void RpcDisconnectGame()
//    {
//        ClientDisconnect();
//    }

//    void ClientDisconnect()
//    {
//        if (PlayerLobbyUI != null)
//        {
//            if (!isServer)
//            {
//                Destroy(PlayerLobbyUI);
//            }
//            else
//            {
//                PlayerLobbyUI.SetActive(false);
//            }
//        }
//    }

//    public void SearchGame()
//    {
//        CmdSearchGame();
//    }

//    [Command]
//    void CmdSearchGame()
//    {
//        if (MainMenu.instance.SearchGame(gameObject, out matchID))
//        {
//            Debug.Log("Игра найдена успешно");
//            networkMatch.matchId = matchID.ToGuid();
//            TargetSearchGame(true, matchID);

//            if (isServer && PlayerLobbyUI != null)
//            {
//                PlayerLobbyUI.SetActive(true);
//            }
//        }
//        else
//        {
//            Debug.Log("Поиск игры не удался");
//            TargetSearchGame(false, matchID);
//        }
//    }

//    [TargetRpc]
//    void TargetSearchGame(bool success, string ID)
//    {
//        matchID = ID;
//        Debug.Log("ID: " + matchID + "==" + ID + " | " + success);
//        MainMenu.instance.SearchGameSuccess(success, ID);
//    }

//    [Server]
//    public void PlayerCountUpdated(int playerCount)
//    {
//        TargetPlayerCountUpdated(playerCount);
//    }

//    [TargetRpc]
//    void TargetPlayerCountUpdated(int playerCount)
//    {
//        if (playerCount > 1)
//        {
//            MainMenu.instance.SetBeginButtonActive(true);
//        }
//        else
//        {
//            MainMenu.instance.SetBeginButtonActive(false);
//        }
//    }

//    public void BeginGame()
//    {
//        CmdBeginGame();
//    }

//    [Command]
//    public void CmdBeginGame()
//    {
//        MainMenu.instance.BeginGame(matchID);
//        Debug.Log("Игра начилась");
//    }

//    public void StartGame()
//    {
//        TargetBeginGame();
//    }

//    [TargetRpc]
//    void TargetBeginGame()
//    {
//        Debug.Log($"ID {matchID} | начало");
//        DontDestroyOnLoad(gameObject);
//        MainMenu.instance.inGame = true;
//        transform.localScale = new Vector3(0.41664f, 0.41664f, 0.41664f); //Размер вашего игрока (x, y, z)
//        facingRight = true;
//        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
//    }

//    private void Update()
//    {
//        if (hasAuthority)
//        {
//            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
//            float speed = 6f * Time.deltaTime;
//            transform.Translate(new Vector2(input.x * speed, input.y * speed));
//            Animator animator = GetComponent<Animator>();
//            if (input.x == 0 && input.y == 0)
//            {
//                animator.SetBool("walk", false);
//            }
//            else
//            {
//                animator.SetBool("walk", true);
//            }
//            if (!facingRight && input.x > 0)
//            {
//                Flip();
//            }
//            else if (facingRight && input.x < 0)
//            {
//                Flip();
//            }
//        }
//    }

//    void Flip()
//    {
//        if (hasAuthority)
//        {
//            facingRight = !facingRight;
//            Vector3 Scale = transform.localScale;
//            Scale.x *= -1;
//            transform.localScale = Scale;

//            Vector3 TextScale = NameDisplayText.transform.localScale;
//            TextScale.x *= -1;
//            NameDisplayText.transform.localScale = TextScale;
//        }
//    }
//}