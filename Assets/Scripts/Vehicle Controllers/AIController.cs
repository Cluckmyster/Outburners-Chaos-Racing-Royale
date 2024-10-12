using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    public float motorTorque = 20000;
    public float brakeTorque = 20000;
    public float maxSpeed = 100;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;

    //Variables for tracking targets
    public List<GameObject> resetPoints;
    public List<GameObject> otherCars;
    public List<GameObject> playerCars;
    public List<GameObject> AICars;
    public Transform target;
    public bool targetRight;
    public bool targetAhead;
    public int totalCars;

    private int retry;
    private int totalResetPoints;

    private bool ramming = false;
    private bool resetting = false;
    public float currentSpeed;

    WheelControl[] wheels;
    Rigidbody rigidBody;

    public GameObject bumper;

    //For setting CoM from inspector
    public Transform centreMass;

    //Awake comes before Start
    private void Awake()
    {
        //Find Reset Points in scene
        resetPoints = GameObject.FindGameObjectsWithTag("ResetPoint").ToList();
        totalResetPoints = resetPoints.Count;

        //Find other cars in scene
        AICars = GameObject.FindGameObjectsWithTag("AICar").ToList();
        playerCars = GameObject.FindGameObjectsWithTag("Player").ToList();
        otherCars = playerCars.Concat(AICars).ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject car in otherCars.ToList())
        {
            if (car == this.gameObject)
            {
                //Remove this car from list
                otherCars.Remove(car);
            }
            else if (car.transform.parent != null)
            {
                //Remove children
                otherCars.Remove(car);
            }
        }
        totalCars = otherCars.Count;

        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass = centreMass.localPosition;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();


        //Find a target on start
        if (target == null)
        {
            int random = Random.Range(0, totalCars);
            target = otherCars[random].transform;
            if (!target.gameObject.GetComponent<Hitpoints>().isActiveAndEnabled || Vector3.Distance(target.transform.position, gameObject.transform.position) < 10.0f)
            {
                target = null;
                retry += 1;
                if (retry > totalCars)
                {
                    retry = 0;
                    Reset();
                }
            }
        }
    }

    //Update is called once per frame
    void Update()
    {
        //Find target if none OR if target becomes invalid
        if (target == null)
        {
            int random = Random.Range(0, totalCars);
            target = otherCars[random].transform;
            if (!target.gameObject.GetComponent<Hitpoints>().isActiveAndEnabled || Vector3.Distance(target.transform.position, gameObject.transform.position) < 10.0f)
            {
                target = null;
                retry += 1;
                if (retry > totalCars)
                {
                    retry = 0;
                    Reset();
                }
            }
        }

        if (target != null)
        {
            if (!resetting)
            {
                if (!target.gameObject.GetComponent<Hitpoints>().enabled)
                {
                    Reset();
                }
            }
        }
    }

    // FixedUpdate is called once per frame and best for physics stuff
    void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(target.transform.position, gameObject.transform.position);
        Vector3 directionToTarget = (target.transform.position - gameObject.transform.position).normalized;
        float dot = Vector3.Dot(gameObject.transform.forward, directionToTarget);
        //1 if target ahead
        //-1 if target behind
        float vInput = 0f;
        if (dot > 0f || distanceToTarget > 4f)
        {
            vInput = 1f;
        }
        else
        {
            vInput = -1f;
        }

        float angleToTarget = Vector3.SignedAngle(gameObject.transform.forward, directionToTarget, Vector3.up);
        //1 if target to the right
        //-1 if target to the left
        float hInput = 0f;
        if (angleToTarget > 0f)
        {
            hInput = 1f;
        }
        else
        {
            hInput = -1f;
        }

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(gameObject.transform.forward, rigidBody.velocity);
        currentSpeed = forwardSpeed;
        if (currentSpeed < 0)
        {
            currentSpeed = -forwardSpeed;
        }

        // Calculate how close the car is to top speed as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // cand to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction as the car's velocity
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        if (vInput == 0)
        {
            isAccelerating = false;
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
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    public void Reset()
    {
        StartCoroutine(ResetTimer());

        int point = Random.Range(0, totalResetPoints);
        target = resetPoints[point].transform;
    }

    IEnumerator ResetTimer()
    {
        resetting = true;
        yield return new WaitForSeconds(3);
        resetting = false;
        target = null;
    }
}

