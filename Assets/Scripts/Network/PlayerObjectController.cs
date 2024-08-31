using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerObjectController : NetworkBehaviour
{
    //PlayerData
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool PlayerReady;

    public GameObject playerCamera;
    public GameObject playerGun;

    public bool dead = false;

    private bool modelsLoaded = true;


    public GameObject playerModel;
    private Rigidbody rb;

    private CarController playerMovementScript;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        rb = this.gameObject.GetComponent<Rigidbody>();
        playerMovementScript = gameObject.GetComponent<CarController>();
    }

    private void Update()
    {
        //Deactivate player car in lobby and reactivate in level
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (modelsLoaded)
            {
                gameObject.GetComponent<AudioSource>().enabled = false;
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
                modelsLoaded = false;
            }
        }
        else
        {
            if (!modelsLoaded)
            {
                gameObject.GetComponent<AudioSource>().enabled = true;
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.SetActive(true);
                }
                
                modelsLoaded = true;

                //Position car near spawn point
                Vector3 spawnPoint = GameObject.Find("SpawnPoint").transform.position;
                gameObject.transform.position = new Vector3(spawnPoint.x + Random.Range(-50.0f, 50.0f), spawnPoint.y, spawnPoint.z + Random.Range(-50.0f, 50.0f));
            }
        }
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.PlayerReady = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.PlayerReady, !this.PlayerReady);
    }

    public void ChangeReady()
    {
        CmdSetPlayerReady();
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        if (isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    //Start Game
    public void CanStartGame(string scene)
    {
        if (authority)
        {
            CmdCanStartGame(scene);
        }
    }

    [Command]
    public void CmdCanStartGame(string scene)
    {
        manager.StartGame(scene);
    }
}
