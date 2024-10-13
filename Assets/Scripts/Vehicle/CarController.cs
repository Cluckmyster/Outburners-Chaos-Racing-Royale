using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using TMPro;

public class CarController : NetworkBehaviour
{
    public AudioClip idle;
    public AudioClip moving;
    private AudioSource audioSource;
    bool playing;

    [Header("Car attributes", order = 0)]
    public float motorTorque;               // Was: 20000
    public float brakeTorque;               // Was: 20000
    public float maxSpeed;                  // Was: 100
    public float steeringRange;             // Was: 30
    public float steeringRangeAtMaxSpeed;   // Was: 10

    [SerializeField] private float motorAcceleration;   // (NOT IMPLEMENTED) Used to determine car acceleration
    [SerializeField] private float motorRearAcceleration;   // (NOT IMPLEMENTED) Used to determine car acceleration during reverse gear
    
    public float currentSpeed;

    private bool reverseGear;

    WheelControl[] wheels;
    Rigidbody rigidBody;

    //For setting CoM from inspector
    public Transform centreMass;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass = centreMass.localPosition;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            Vector3 pos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(pos.x, pos.y + 2, pos.z);
        }

        //Debug.Log(rigidBody.velocity);
    }

    void FixedUpdate()
    {
        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        // Sets variable to false if car is forward gearing, true if car is reverse gearing
        reverseGear = (vInput >= 0) ? false : true;

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        currentSpeed = forwardSpeed;
        
        if (reverseGear)
        {
            currentSpeed = -forwardSpeed;
        }

        // Clamps the variable's value (not car's actual speed) to max speed value
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

        // Calculate how close the car is to top speed as a number from zero to one (even if in reverse gear)
        float speedFactor = (!reverseGear) ? Mathf.InverseLerp(0, maxSpeed, forwardSpeed) : Mathf.InverseLerp(0, -maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // Åcand to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction as the car's velocity
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        if (vInput == 0)
        {
            isAccelerating = false;

            if (audioSource.clip == moving)
            {
                audioSource.clip = idle;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == idle)
            {
                audioSource.clip = moving;
                audioSource.Play();
            }
        }

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.Steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.Motorized)
                {
                    if (!reverseGear)
                    {
                        wheel.WheelCollider.motorTorque = vInput * currentMotorTorque * motorAcceleration;
                    }

                    else
                    {
                        wheel.WheelCollider.motorTorque = vInput * currentMotorTorque * motorRearAcceleration;
                    }
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }
}
