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

    //Player Customisation
    [SyncVar(hook = nameof(ChangeCarColour))] public int PlayerColour;
    public Material[] colours;

    public GameObject playerCamera;
    public GameObject playerGun;

    public bool dead = false;

    private bool carControlOn = true;
    
    public Material newColour;
    public GameObject playerModel;
    private Rigidbody rb;
    private MeshRenderer carMesh;

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

    private void ResetPosition()
    {
        Vector3 spawnPoint = GameObject.Find("SpawnPoint").transform.position;
        gameObject.transform.position = new Vector3(spawnPoint.x + Random.Range(-50.0f, 50.0f), spawnPoint.y, spawnPoint.z + Random.Range(-50.0f, 50.0f));
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        rb = this.gameObject.GetComponent<Rigidbody>();
        playerMovementScript = gameObject.GetComponent<CarController>();
        carMesh = playerModel.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (gameObject.transform.position.y < -200.0f)
        {
            ResetPosition();
        }

        //Position player car in lobby and move to spawn in level
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (gameObject.name == "LocalGamePlayer")
            {
                //Position car in front of camera
                gameObject.transform.position = GameObject.Find("carPos").transform.position;
                gameObject.transform.rotation = GameObject.Find("carPos").transform.rotation;
            }

            //Toggle cursor on in lobby
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            if (carControlOn)
            {
                gameObject.GetComponent<AudioSource>().enabled = false;
                gameObject.GetComponent<CarController>().enabled = false;
                carControlOn = false;
            }
        }
        else
        {
            //Toggle cursor off in level
            //Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (!carControlOn)
            {
                gameObject.GetComponent<AudioSource>().enabled = true;
                gameObject.GetComponent<CarController>().enabled = true;
                foreach (Transform child in gameObject.transform)
                {
                    if (child.name != "Main Camera" || gameObject.name == "LocalGamePlayer")
                    {
                        child.gameObject.SetActive(true);
                    }
                }

                carControlOn = true;

                //Position car near spawn point
                ResetPosition();
                UpdateCarCosmetics();
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

    //Customisation
    [Command]
    public void cmdUpdateCar(int newValue)
    {
        ChangeCarColour(PlayerColour, newValue);
    }

    //Update car colour
    public void ChangeCarColour(int oldValue, int newValue)
    {
        if (oldValue != newValue)
        {
            UpdateColour(newValue);
        }

        //Material[] mats = playerModel.GetComponent<MeshRenderer>().materials;
        //mats[0] = mat;
        //mats[1] = mat;
        //mats[2] = mat;
        //playerModel.GetComponent<MeshRenderer>().materials = mats;
    }

    void UpdateColour(int message)
    {
        PlayerColour = message;
        UpdateCarCosmetics();
    }

    public void UpdateCarCosmetics()
    {
        Material[] mats = carMesh.materials;
        mats[0] = colours[PlayerColour];
        mats[1] = colours[PlayerColour];
        mats[2] = colours[PlayerColour];
        carMesh.materials = mats;
    }
}
