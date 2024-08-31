using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostButton : MonoBehaviour
{
    public GameObject NetworkManager;

    private void Start()
    {
        NetworkManager = GameObject.Find("NetworkManager");
        gameObject.GetComponent<Button>().onClick.AddListener(delegate { NetworkManager.GetComponent<SteamLobby>().HostLobby(); });
    }
}
