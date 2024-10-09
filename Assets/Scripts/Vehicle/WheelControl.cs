using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider WheelCollider;

    [Tooltip("Does the wheel belong to car's forward axle?")]
    [SerializeField] private bool forwardAxle;

    private bool steerable;
    private bool motorized;

    // Properties for "steerable" and "motorized" bools
    // This makes the code safer
    public bool Steerable { get => steerable; }
    public bool Motorized { get => motorized; }

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
        
        if (forwardAxle)
        {
            steerable = true;
            motorized = true;
        }

        else
        {
            steerable= false;
            motorized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Wheel collider's world pose values and use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        rotation *= Quaternion.Euler(0, 0, 0);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;
    }
}
