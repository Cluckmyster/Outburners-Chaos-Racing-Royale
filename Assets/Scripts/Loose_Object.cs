using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using TMPro;

public class Loose_Object : NetworkBehaviour
{
    private bool hit;

    private Collider col;
    private Rigidbody rb;
    private NetworkIdentity netIdent;
    private NetworkTransformReliable netTrans;
    private NetworkRigidbodyReliable netRB;

    private void Awake()
    {
        hit = false;
        col = gameObject.GetComponent<Collider>();
        col.isTrigger = true;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit && (other.gameObject.transform != gameObject.transform.parent) && SceneManager.GetActiveScene().name != "Lobby")
        {
            hit = true;
            rb.isKinematic = false;
            gameObject.transform.parent = null;
            //rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            netIdent = gameObject.AddComponent(typeof(NetworkIdentity)) as NetworkIdentity;
            //netTrans = gameObject.AddComponent(typeof(NetworkTransformReliable)) as NetworkTransformReliable;
            //netRB = gameObject.AddComponent(typeof(NetworkRigidbodyReliable)) as NetworkRigidbodyReliable;
            StartCoroutine(InterationPeriod());
        }
    }

    IEnumerator InterationPeriod()
    {
        col.isTrigger = false;
        rb.isKinematic = false;
        yield return new WaitForSeconds(8);
        col.isTrigger = true;
        rb.isKinematic = true;
    }
}
