using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    #region Variables

    [Tooltip("Target object for the camera.")]
    [SerializeField] private Transform target;

    #region Orbital rotation
    
    // Stores the distance between player's car and camera
    private float distanceToPlayer;

    [Header("Orbital rotation", order = 0)]
    // Stores mouse sensitivity
    [SerializeField] private MouseSensitivity mouseSensitivity;

    // Stores camera rotation values
    private CameraRotation camRotation;

    // Stores minimum and maximum camera angle for clamp
    [SerializeField] private CameraAngle camAngle;

    #endregion

    #region Camera reset

    // Stores camera's start rotation, for camera resetting after a camera movement's idle time.
    private Vector3 camStartRotation;

    [Header("Camera reset", order = 1)]

    [Tooltip("How much time (in seconds) must pass before camera resets its position.")]
    [SerializeField] private float maxIdleTimer;

    private float idleTimer;
    private float timeCount = 0;

    #endregion

    #endregion

    #region Methods

    #region Unity Methods

    private void Awake()
    {
        // Initializes distance of camera from player
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camStartRotation = new Vector3(camAngle.minAngle, 0, 0);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            // If any movement with mouse is performed, all timers are resetted
            idleTimer = 0;
            timeCount = 0;

            // Changes camera Pitch/Yaw values depending of how players move their mouse
            camRotation.yaw += (Input.GetAxis("Mouse X") * mouseSensitivity.horizontalSensitivity) * Time.deltaTime;
            camRotation.pitch += (Input.GetAxis("Mouse Y") * mouseSensitivity.verticalSensitivity) * Time.deltaTime;

            // Clamps Pitch (X axis rotation) to not let it be unmanaged
            camRotation.pitch = Mathf.Clamp(camRotation.pitch, camAngle.minAngle, camAngle.maxAngle);
        }

        else
        {
            // If no movement with mouse is performed, timer starts
            idleTimer += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (idleTimer < maxIdleTimer)
        {
            // Rotates the camera using Pitch and Yaw values
            transform.localEulerAngles = new Vector3(camRotation.pitch, camRotation.yaw, 0);
        }

        else
        {
            // Starts a Coroutine that performs camera resetting, allowing to the rest of the code to be executed only after camera resetting is finished
            StartCoroutine(CameraReset(timeCount));

            // Sets camera's pitch and yaw to current rotation values, to avoid camera snaps
            camRotation.pitch = transform.localEulerAngles.x;
            camRotation.yaw = transform.localEulerAngles.y;
        }

        // Sets camera position (after rotation) to always look to the player
        transform.position = target.position - transform.forward * distanceToPlayer;
    }

    #endregion

    #region Personal Methods

    private IEnumerator CameraReset(float time)
    {
        transform.localEulerAngles = new Vector3
                (
                Mathf.LerpAngle(transform.localEulerAngles.x, camStartRotation.x, timeCount),
                Mathf.LerpAngle(transform.localEulerAngles.y, camStartRotation.y, timeCount),
                0
                )
                ;

        timeCount += Time.deltaTime;

        yield return new WaitForSeconds(time);
    }

    #endregion

    #endregion
}
