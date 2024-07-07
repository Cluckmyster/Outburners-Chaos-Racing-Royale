using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Hitpoints hitpointsScript;

    // Start is called before the first frame update
    void Start()
    {
        hitpointsScript = gameObject.GetComponent<Hitpoints>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
