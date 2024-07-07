using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    public GameObject car;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
