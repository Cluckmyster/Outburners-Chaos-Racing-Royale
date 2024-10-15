using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraDetachment : MonoBehaviour
{
    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "Lobby")
        {
            transform.parent = null;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {

    }
}
